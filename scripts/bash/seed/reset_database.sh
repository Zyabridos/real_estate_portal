#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../../lib/config.sh"
source "${SCRIPT_DIR}/../../lib/log.sh"

MONGO_CONTAINER="${MONGO_CONTAINER:-realestate_mongodb}"
MONGO_DB="${MONGO_DB:-realestate_dev}"
MONGO_AUTH_DB="${MONGO_AUTH_DB:-admin}"
RESET_USERS="${RESET_USERS:-false}"

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
neutral "Reset users: ${RESET_USERS}"

docker exec -i "${MONGO_CONTAINER}" mongosh \
  --username "${MONGO_USERNAME}" \
  --password "${MONGO_PASSWORD}" \
  --authenticationDatabase "${MONGO_AUTH_DB}" \
  "${MONGO_DB}" \
  --quiet <<EOF
const resetUsers = "${RESET_USERS}" === "true";

const collectionsToClear = [
  "leads",
  "properties",
  "brokers",
  "agencies"
];

const counterKeys = [
  "leads",
  "properties",
  "brokers",
  "agencies"
];

if (resetUsers) {
  collectionsToClear.push("users");
  counterKeys.push("users");
}

try {
  for (const name of collectionsToClear) {
    const exists = db.getCollectionNames().includes(name);

    if (!exists) {
      print(\`[seed-reset] skipped \${name}: collection not found\`);
      continue;
    }

    const result = db.getCollection(name).deleteMany({});
    print(\`[seed-reset] cleared \${name}: \${result.deletedCount}\`);
  }

  const countersExists = db.getCollectionNames().includes("counters");

  if (countersExists) {
    const countersDeleteResult = db.getCollection("counters").deleteMany({
      _id: { $in: counterKeys }
    });
    print(\`[seed-reset] counters removed: \${countersDeleteResult.deletedCount}\`);
  } else {
    print("[seed-reset] skipped counters: collection not found");
  }
} catch (err) {
  print(\`[seed-reset] ERROR: \${err.message}\`);
  quit(1);
}
EOF

seed_env="${SCRIPT_DIR}/../../.seed/seed.env"
if [[ -f "${seed_env}" ]]; then
  rm -f "${seed_env}"
  success "[seed-reset] Removed ${seed_env}"
fi

if [[ "${RESET_USERS}" == "true" ]]; then
  warn "[seed-reset] Users were removed. Restart backend before running seed so AdminUserSeeder can recreate the admin account."
fi

success "[seed-reset] Database reset completed"