Torneo Galáctico

Demo para gestionar el Gran Torneo Galáctico registro de especies, combates

y ranking por victorias. API REST en ASP.NET Core (.NET 10) con interfaz web.


Funcionalidades

Registrar y listar especies (nombre, nivel de poder, habilidad especial).

Combate entre dos especies gana el mayor nivel de poder en empate,

el nombre alfabéticamente primero.


Combate aleatorio entre dos especies distintas.

Historial de combates y ranking ordenado por victorias.

Requisitos

Opción A Docker y Docker Compose.

Ejecución rápida (Docker)

```bash

docker compose up --build

```

Abrir httplocalhost8080


Ejecución local

```bash

dotnet run --project srcTorneoGalactico.Web

```

Por defecto usa almacenamiento en memoria.


Tests

```bash

dotnet test

```



API

Método	Ruta	Descripción

POST	apispecies	Registrar especie

GET	apispecies	Listar especies

GET	apispecies{id}	Obtener una especie

POST	apibattles	Combate entre dos especies

POST	apibattlesrandom	Combate aleatorio

GET	apibattles	Historial de combates

GET	apiranking	Ranking por victorias

Los errores se devuelven como ProblemDetails (400, 404, 409).



Arquitectura

```

src

TorneoGalactico.Domain Entidades y regla del combate

TorneoGalactico.Application Casos de uso, DTOs, interfaces de repositorio

TorneoGalactico.Infrastructure Repositorios en memoria y EF Core + PostgreSQL

TorneoGalactico.Web API, manejo de errores e interfaz web

tests

TorneoGalactico.UnitTests

TorneoGalactico.IntegrationTests

```



Decisiones de diseño

Capas con dependencias hacia el dominio la regla del combate no depende de

base de datos ni de HTTP, y se prueba de forma aislada.



Ranking calculado a partir de los combates registrados, no almacenado

una sola fuente de verdad.



Persistencia (memoria) mediante interfaces de

repositorio y configuración.



Nombres únicos sin distinguir mayúsculas, reforzado con un índice único en la base.

Supuestos

La comparación alfabética es ordinal.


El nivel de poder es un entero mayor o igual a 0.

Una especie no puede combatir contra sí misma.

En el ranking, el empate de victorias se ordena por nombre.


