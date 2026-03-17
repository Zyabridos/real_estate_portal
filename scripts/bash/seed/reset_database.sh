#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

MONGO_CONTAINER="${MONGO_CONTAINER:-realestate_mongodb}"
MONGO_DB="${MONGO_DB:-realestate_dev}"
MONGO_AUTH_DB="${MONGO_AUTH_DB:-admin}"

if [[ -z "${MONGO_USERNAME:-}" ]]; then
  read -rp "Enter Mongo username: " MONGO_USERNAME
fi

if [[ -z "${MONGO_PASSWORD:-}" ]]; then
  read -rsp "Enter Mongo password for ${MONGO_USERNAME}: " MONGO_PASSWORD
  echo
fi

require_cmd() {
  command -v "$1" >/dev/null 2>&1 || { error "Missing required command: $1"; exit 1; }
}

require_cmd docker

neutral "Resetting Mongo seed state"
neutral "Container: ${MONGO_CONTAINER}"
neutral "Database: ${MONGO_DB}"
neutral "Auth DB: ${MONGO_AUTH_DB}"
neutral "User: ${MONGO_USERNAME}"

docker exec -i "${MONGO_CONTAINER}" mongosh \
  --username "${MONGO_USERNAME}" \
  --password "${MONGO_PASSWORD}" \
  --authenticationDatabase "${MONGO_AUTH_DB}" \
  "${MONGO_DB}" \
  --quiet <<'EOF'
const collectionsToClear = ["leads", "properties", "brokers", "agencies"];
const counterKeys = ["leads", "properties", "brokers", "agencies"];

try {
  for (const name of collectionsToClear) {
    const result = db.getCollection(name).deleteMany({});
    print(`[seed-reset] cleared ${name}: ${result.deletedCount}`);
  }

  const countersDeleteResult = db.getCollection("counters").deleteMany({
    _id: { $in: counterKeys }
  });
  print(`[seed-reset] counters removed: ${countersDeleteResult.deletedCount}`);

} catch (err) {
  print(`[seed-reset] ERROR: ${err.message}`);
  quit(1);
}
EOF

success "[seed-reset] Database reset completed"