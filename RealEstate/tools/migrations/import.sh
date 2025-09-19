#!/usr/bin/env bash
set -euo pipefail

# Restaura una base de datos desde un archivo .archive.gz generado por export.sh
# Uso:
#   ./import.sh [ruta_al_archive]
# Variables:
#   MONGO_URI (opcional)  - URI de conexión destino
#   MONGO_DB  (opcional)  - Nombre de la base destino (si no se define, se usa la del archivo)
#   DROP      (opcional)  - true/false para ejecutar --drop (default: true)

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "$SCRIPT_DIR/../.." && pwd)"
MIG_DIR="$REPO_ROOT/tools/migrations"
DUMP_DIR="$MIG_DIR/dump"

: "${MONGO_URI:=mongodb://localhost:27017}"
: "${DROP:=true}"

ARCHIVE_PATH="${1:-$DUMP_DIR/latest.archive.gz}"
if [ ! -f "$ARCHIVE_PATH" ]; then
  echo "ERROR: No se encontró el archivo '$ARCHIVE_PATH'" >&2
  exit 1
fi

if ! command -v mongorestore >/dev/null 2>&1; then
  echo "ERROR: mongorestore no está instalado. Instálalo con: brew install mongodb-database-tools" >&2
  exit 1
fi

# Inferir nombre de DB origen desde el nombre del archivo: <dbname>_YYYYmmdd_HHMMSS.archive.gz
FILENAME="$(basename "$ARCHIVE_PATH")"
SRC_DB_GUESS="${FILENAME%%_*}"

if [[ "${MONGO_DB:-}" == "" ]]; then
  # Si no se especifica MONGO_DB, restauramos con el nombre original del archivo
  echo "Importando DB '$SRC_DB_GUESS' -> mismo nombre en '$MONGO_URI' desde '$ARCHIVE_PATH'"
  mongorestore \
    $( [ "$DROP" = "true" ] && printf %s "--drop" ) \
    --uri="$MONGO_URI" \
    --archive="$ARCHIVE_PATH" \
    --gzip
else
  # Mapear nombres de DB: origen -> destino
  echo "Importando DB '$SRC_DB_GUESS' -> '$MONGO_DB' en '$MONGO_URI' desde '$ARCHIVE_PATH'"
  mongorestore \
    $( [ "$DROP" = "true" ] && printf %s "--drop" ) \
    --uri="$MONGO_URI" \
    --nsFrom="${SRC_DB_GUESS}.*" \
    --nsTo="${MONGO_DB}.*" \
    --archive="$ARCHIVE_PATH" \
    --gzip
fi

echo "Import completado."


