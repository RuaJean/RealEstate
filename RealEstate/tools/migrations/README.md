## Migraciones de datos para MongoDB

Scripts para exportar/importar la base de datos de este proyecto usando las MongoDB Database Tools (`mongodump`/`mongorestore`).

### Requisitos

- macOS/Linux con bash
- MongoDB Database Tools:
  - Instalar en macOS: `brew install mongodb-database-tools`
  - Verificar: `mongodump --version` y `mongorestore --version`

### Variables de entorno

- `MONGO_URI`: URI de conexión. Ejemplos:
  - Local sin auth: `mongodb://localhost:27017`
  - Docker compose con auth por defecto: `mongodb://root:example@localhost:27017/?authSource=admin`
- `MONGO_DB`: Nombre de la base (por defecto `RealEstateDb`).
- `DROP`: Al importar, si `true` elimina colecciones antes de restaurar (default: `true`).

### Exportar datos actuales

```bash
cd tools/migrations
# Ejemplo con docker-compose por defecto (usuario root/example)
MONGO_URI="mongodb://root:example@localhost:27017/?authSource=admin" MONGO_DB=RealEstateDb ./export.sh
# o simplemente (si usas mongod local sin auth)
./export.sh
```

Esto generará un archivo `dump/<DBNAME>_YYYYmmdd_HHMMSS.archive.gz` y actualizará el enlace `dump/latest.archive.gz`.

### Importar en otro entorno

```bash
cd tools/migrations
# Importar al mismo nombre de DB que trae el archivo
./import.sh dump/latest.archive.gz

# Importar mapeando a otra DB destino
MONGO_DB=RealEstateDb ./import.sh dump/latest.archive.gz

# Importar en servidor con auth
MONGO_URI="mongodb://root:example@localhost:27017/?authSource=admin" ./import.sh
```

Por defecto se aplica `--drop`. Si no quieres borrar colecciones destino:

```bash
DROP=false ./import.sh dump/latest.archive.gz
```

### Índices y consistencia

- Este proyecto crea índices al iniciar a través de `MongoDbContext.EnsureIndexes()`. Aun así, `mongorestore` restaurará índices desde el dump.
- Si importas en una DB vacía y ejecutas la API, los índices se (re)crearán automáticamente.

### Seed alternativo (datos sintéticos)

Si quieres poblar con datos de ejemplo (no reales), puedes usar la consola de seed:

```bash
dotnet run --project tools/SmokeConsole/SmokeConsole.csproj
```

Usa la configuración de `src/RealEstate.Api/appsettings.json` para conectarse a MongoDB.


