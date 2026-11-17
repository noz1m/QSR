#!/usr/bin/env bash
set -euo pipefail

container="${1:?Usage: verify-backup.sh container user backup.dump}"
db_user="${2:?User is required}"
backup="${3:?Backup file is required}"
test -f "$backup"

verification_db="qsr_restore_${RANDOM}_${RANDOM}"
docker exec "$container" createdb -U "$db_user" "$verification_db"
trap 'docker exec "$container" dropdb -U "$db_user" "$verification_db"' EXIT
docker exec -i "$container" pg_restore -U "$db_user" -d "$verification_db"   --exit-on-error --single-transaction --no-owner --no-acl < "$backup"
docker exec "$container" psql -U "$db_user" -d "$verification_db"   --set=ON_ERROR_STOP=1 -c 'SELECT COUNT(*) AS orders FROM orders;'
echo "Backup restored successfully to the isolated verification database."
