#!/usr/bin/env bash
set -euo pipefail
umask 077

container="${1:?Usage: backup-postgres.sh container database user output.dump}"
database="${2:?Database is required}"
db_user="${3:?User is required}"
output="${4:?Output file is required}"

if [[ -e "$output" ]]; then
  echo "Output already exists." >&2
  exit 1
fi

temporary="$(mktemp "${output}.tmp.XXXXXX")"
trap 'rm -f "$temporary"' EXIT
docker exec "$container" pg_dump -U "$db_user" -d "$database"   --format=custom --no-owner --no-acl > "$temporary"
test -s "$temporary"
mv "$temporary" "$output"
trap - EXIT
