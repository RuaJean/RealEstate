#!/usr/bin/env bash
set -euo pipefail

# Exporta la base de datos Mongo a un archivo .archive.gz con timestamp.
# Variables:
#   MONGO_URI (opcional)  - URI de conexión. Ej: mongodb://localhost:27017 o mongodb://user:pass@localhost:27017/?authSource=admin
#   MONGO_DB  (opcional)  - Nombre de la base. Por defecto RealEstateDb

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
MIG_DIR="$REPO_ROOT/tools/migrations"
DUMP_DIR="$MIG_DIR/dump"
mkdir -p "$DUMP_DIR"

: "${MONGO_URI:=mongodb://localhost:27017}"
: "${MONGO_DB:=RealEstateDb}"

STAMP="$(date +%Y%m%d_%H%M%S)"
ARCHIVE="$DUMP_DIR/${MONGO_DB}_${STAMP}.archive.gz"

if ! command -v mongodump >/dev/null 2>&1; then
  echo "ERROR: mongodump no está instalado. Instálalo con: brew install mongodb-database-tools" >&2
  exit 1
fi

echo "Exportando base '$MONGO_DB' desde '$MONGO_URI' -> $ARCHIVE"
mongodump --uri="$MONGO_URI" --db="$MONGO_DB" --archive="$ARCHIVE" --gzip

# Actualiza puntero a 'latest'
ln -sfn "$(basename "$ARCHIVE")" "$DUMP_DIR/latest.archive.gz"

echo "Export completado: $ARCHIVE"


