const $ = (id) => document.getElementById(id);

// Llamada a la API: lanza un Error con el mensaje de ProblemDetails si falla
async function api(url, options = {}) {
    const response = await fetch(url, {
        headers: { 'Content-Type': 'application/json' },
        ...options,
    });
    if (!response.ok) {
        const problem = await response.json().catch(() => ({}));
        throw new Error(problem.detail || problem.title || `Error ${response.status}`);
    }
    return response.json();
}

function showMessage(text, type = 'ok') {
    const box = $('message');
    box.textContent = text;
    box.className = `message ${type}`;
    box.hidden = false;
    setTimeout(() => (box.hidden = true), 4000);
}

// Crea una fila con textContent (evita inyección de HTML)
function row(...cells) {
    const tr = document.createElement('tr');
    for (const value of cells) {
        const td = document.createElement('td');
        td.textContent = value;
        tr.appendChild(td);
    }
    return tr;
}

function fillTable(tbodyId, rows, emptyText) {
    const tbody = $(tbodyId);
    tbody.replaceChildren(...rows);
    if (rows.length === 0) {
        const tr = row(emptyText);
        tr.firstChild.colSpan = tbody.closest('table').querySelectorAll('th').length;
        tbody.appendChild(tr);
    }
}

function fillSelect(select, species) {
    const current = select.value;
    select.replaceChildren(
        ...species.map((s) => new Option(`${s.name} (${s.powerLevel})`, s.id))
    );
    if (species.some((s) => s.id === current)) select.value = current;
}

async function loadSpecies() {
    const species = await api('/api/species');
    fillTable('species-body', species.map((s) => row(s.name, s.powerLevel, s.specialAbility)), 'Aún no hay especies');
    fillSelect($('fighter-a'), species);
    fillSelect($('fighter-b'), species);
    if (species.length > 1 && $('fighter-a').value === $('fighter-b').value) {
        $('fighter-b').selectedIndex = 1;
    }
}

async function loadRanking() {
    const ranking = await api('/api/ranking');
    fillTable('ranking-body', ranking.map((r) => row(r.position, r.name, r.powerLevel, r.wins)), 'Sin datos');
}

async function loadBattles() {
    const battles = await api('/api/battles');
    fillTable(
        'battles-body',
        battles.map((b) => row(
            new Date(b.foughtAt).toLocaleString('es-BO'),
            `${b.fighterAName} vs ${b.fighterBName}`,
            b.winnerName
        )),
        'Aún no hay combates'
    );
}

async function refreshAll() {
    try {
        await Promise.all([loadSpecies(), loadRanking(), loadBattles()]);
    } catch (error) {
        showMessage(error.message, 'error');
    }
}

function showBattle(battle) {
    $('battle-result').textContent =
        `${battle.fighterAName} vs ${battle.fighterBName}: gana ${battle.winnerName}`;
}

$('species-form').addEventListener('submit', async (event) => {
    event.preventDefault();
    const form = event.target;
    const data = new FormData(form);
    try {
        const created = await api('/api/species', {
            method: 'POST',
            body: JSON.stringify({
                name: data.get('name'),
                powerLevel: Number(data.get('powerLevel')),
                specialAbility: data.get('specialAbility'),
            }),
        });
        form.reset();
        showMessage(`Especie "${created.name}" registrada`);
        await refreshAll();
    } catch (error) {
        showMessage(error.message, 'error');
    }
});

$('fight-btn').addEventListener('click', async () => {
    try {
        const battle = await api('/api/battles', {
            method: 'POST',
            body: JSON.stringify({ fighterAId: $('fighter-a').value, fighterBId: $('fighter-b').value }),
        });
        showBattle(battle);
        await refreshAll();
    } catch (error) {
        showMessage(error.message, 'error');
    }
});

$('random-btn').addEventListener('click', async () => {
    try {
        const battle = await api('/api/battles/random', { method: 'POST' });
        showBattle(battle);
        await refreshAll();
    } catch (error) {
        showMessage(error.message, 'error');
    }
});

refreshAll();