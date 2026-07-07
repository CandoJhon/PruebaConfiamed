# Prueba Técnica — Gestión de Ítems de Trabajo y Distribución

> Código desarrollado como parte de una prueba técnica de evaluación.
> Uso restringido a fines de evaluación del candidato; no se autoriza su
> uso en producción, distribución ni reutilización comercial sin acuerdo
> previo por escrito con el autor.

## Arquitectura

Solución compuesta por **dos microservicios independientes**, cada uno
en arquitectura de N capas (Domain / Infrastructure / Application / Api):

```
PruebaTecnica.sln
src/
  GestionUsuarios/
    GestionUsuarios.Domain          -> entidad Usuario, reglas de saturación
    GestionUsuarios.Infrastructure  -> DbContext (EF Core + SQLite), repositorio
    GestionUsuarios.Application     -> DTOs, servicio de negocio
    GestionUsuarios.Api             -> controllers REST, Program.cs
  ItemsTrabajo/
    ItemsTrabajo.Domain             -> entidad ItemTrabajo, enums
    ItemsTrabajo.Infrastructure     -> DbContext (EF Core + SQLite), repositorio
    ItemsTrabajo.Application        -> DTOs, DistribucionService (algoritmo), cliente HTTP
    ItemsTrabajo.Api                -> controllers REST, Program.cs
```

**Comunicación entre microservicios:** `ItemsTrabajo.Api` llama vía REST
(HTTP simple) a `GestionUsuarios.Api` para (a) leer el estado actual de
carga de cada usuario antes de asignar un ítem, y (b) notificarle cuando
asigna o completa un ítem, para que actualice sus contadores. Esto separa
responsabilidades: Ítems de Trabajo es dueño del catálogo de ítems y del
algoritmo de asignación; Gestión de Usuarios es dueño del estado/contadores
por usuario.

**Persistencia:** ambos microservicios usan EF Core con proveedor
**SQLite** (`Microsoft.EntityFrameworkCore.Sqlite`). Se eligió SQLite por ser
una base de datos real y persistente en disco, sin necesidad de instalar ni
configurar un motor externo — ideal para el alcance de esta prueba técnica.

El esquema se crea automáticamente al arrancar cada Api mediante
`Database.EnsureCreated()` (ver bloque al final de cada `Program.cs`), sin
necesidad de manejar migraciones para este ejercicio. La cadena de conexión
vive en `appsettings.json -> ConnectionStrings`, por ejemplo:
```json
"ConnectionStrings": {
  "GestionUsuariosDb": "Data Source=gestionusuarios.db"
}
```
El archivo `.db` resultante aparece en el directorio de trabajo del proceso
en ejecución (con `dotnet run` desde la carpeta del proyecto, ahí mismo; con
`F5` en Visual Studio, normalmente en `bin/Debug/net8.0/`). Los datos
**persisten** entre reinicios del backend — a diferencia de InMemory, no se
pierden al detener el proceso. Para inspeccionarlos se puede usar
[DB Browser for SQLite](https://sqlitebrowser.org), una extensión de SQLite
para VS Code, o el CLI `sqlite3`.

Cambiar a SQL Server/PostgreSQL en el futuro solo requiere reemplazar
`UseSqlite(...)` por `UseSqlServer(...)` / `UseNpgsql(...)` en cada
`Program.cs` (la interfaz de repositorio no cambia).

## Asunciones documentadas

El enunciado deja algunos puntos abiertos a interpretación; estas son las
decisiones tomadas y su justificación:

1. **Regla de saturación** se evalúa solo sobre ítems de alta relevancia
   **pendientes** (no completados) — un usuario deja de estar saturado a
   medida que completa sus ítems de alta relevancia.
2. **Prioridad urgencia vs. relevancia:** al asignar un único ítem, ambas
   reglas convergen en el mismo criterio (usuario no saturado con menos
   pendientes). La prioridad "los ítems relevantes se asignan primero" se
   aplica en el **endpoint de asignación por lote** (`POST /api/itemstrabajo/lote`),
   donde el orden de procesamiento es: urgentes → alta relevancia → baja
   relevancia → fecha de entrega más próxima. Se documenta este supuesto
   explícitamente en `DistribucionService.cs`.
3. **Desempate** entre usuarios con igual cantidad de pendientes: orden
   alfabético por nombre de usuario, para tener un resultado determinístico
   y testeable.
4. Los usuarios no se "crean" formalmente desde este sistema (ya existen en
   otra parte); `GestionUsuarios.Api` registra automáticamente la referencia
   por nombre de usuario la primera vez que se le asigna un ítem (además,
   se puede registrar explícitamente vía `POST /api/usuarios`).
5. Los archivos `.db` de SQLite se excluyen del control de versiones
   (`.gitignore`), ya que son datos generados en tiempo de ejecución, no
   parte del código fuente.

## Cómo ejecutar

Requiere .NET SDK 8 (o 9, cambiando `TargetFramework` en los `.csproj`).

**Opción A — dos terminales:**
```bash
# Terminal 1
cd src/GestionUsuarios/GestionUsuarios.Api
dotnet run
# Disponible en http://localhost:5100/swagger (ver nota de puertos abajo)

# Terminal 2
cd src/ItemsTrabajo/ItemsTrabajo.Api
dotnet run
# Disponible en http://localhost:5200/swagger
```

**Opción B — Visual Studio 2022:** clic derecho sobre la solución →
"Configurar proyectos de inicio..." → "Varios proyectos de inicio" → poner
**Inicio** en `GestionUsuarios.Api` e `ItemsTrabajo.Api` (los demás en
Ninguno) → `F5`. Arrancan ambos a la vez, cada uno abre su Swagger.

`ItemsTrabajo.Api` está configurado para llamar a `GestionUsuarios.Api` en
`http://localhost:5100` (ver `appsettings.json -> Servicios:GestionUsuariosBaseUrl`).
Debe iniciarse **primero** `GestionUsuarios.Api`.

> **Nota sobre puertos:** `launchSettings.json` fija 5100/5200, pero según
> cómo se ejecute (IDE vs terminal, perfil tomado) el puerto real asignado
> puede diferir (por ejemplo, 5000). Verifica el puerto real en la consola
> al arrancar, y si no coincide con `GestionUsuariosBaseUrl`, ajusta ese
> valor en `ItemsTrabajo.Api/appsettings.json` (o fuerza el puerto con
> `dotnet run --urls "http://localhost:5100"`).

> **Importante — crear usuarios antes de asignar ítems:** el algoritmo de
> distribución solo elige entre usuarios **ya existentes** en
> `GestionUsuarios.Api` (no inventa usuarios nuevos). Antes de crear el
> primer ítem de trabajo, registra al menos dos usuarios vía Swagger:
> `POST /api/usuarios` con `{ "nombreUsuario": "ana" }`, y lo mismo para
> `"carlos"`. Si no hay usuarios registrados, `POST /api/itemstrabajo`
> responde con conflicto ("no hay usuarios disponibles").

## Endpoints principales

**GestionUsuarios.Api**
- `GET /api/usuarios` — lista usuarios y contadores
- `GET /api/usuarios/{nombreUsuario}`
- `POST /api/usuarios/{nombreUsuario}/asignar-item` `{ "esAltaRelevancia": bool }`
- `POST /api/usuarios/{nombreUsuario}/completar-item` `{ "esAltaRelevancia": bool }`

**ItemsTrabajo.Api**
- `POST /api/itemstrabajo` `{ "titulo", "fechaEntrega", "relevancia": "Alta"|"Baja" }` — crea y asigna un ítem
- `POST /api/itemstrabajo/lote` — arreglo de ítems, se asignan respetando prioridad
- `GET /api/itemstrabajo`
- `GET /api/itemstrabajo/usuario/{nombreUsuario}`
- `POST /api/itemstrabajo/{id}/completar`

## Pendiente / posibles mejoras (fuera de alcance de la prueba)

- Pruebas unitarias e integración para `DistribucionService` y ambos controllers.
- Resiliencia en las llamadas HTTP entre microservicios (retry/circuit breaker con Polly).
- Autenticación entre servicios (API key o JWT de servicio a servicio).
