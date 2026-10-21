using System;
using System.Collections.Generic;
using System.Text;

namespace TorneoGalactico.Domain
{
    public static class BattleRules
    {
        public static Species DetermineWinner(Species fighterA, Species fighterB)
        {
            ArgumentNullException.ThrowIfNull(fighterA);
            ArgumentNullException.ThrowIfNull(fighterB);

            if (fighterA.Id == fighterB.Id)
                throw new DomainException("Una especie no puede combatir contra sí misma.");

            // Regla 1: gana el de mayor nivel de poder
            if (fighterA.PowerLevel != fighterB.PowerLevel)
                return fighterA.PowerLevel > fighterB.PowerLevel ? fighterA : fighterB;

            // Regla 2: en empate, gana el nombre alfabéticamente primero (sin distinguir mayúsculas)
            var byName = string.Compare(fighterA.Name, fighterB.Name, StringComparison.OrdinalIgnoreCase);

            // Desempate final determinista (no debería ocurrir porque los nombres son únicos)
            if (byName == 0)
                byName = string.Compare(fighterA.Name, fighterB.Name, StringComparison.Ordinal);

            return byName <= 0 ? fighterA : fighterB;
        }
    }
}
