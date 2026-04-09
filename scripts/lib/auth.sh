#!/usr/bin/env bash
set -euo pipefail

require_auth_tools() {
  command -v curl >/dev/null 2>&1 || { echo "Missing required command: curl" >&2; exit 1; }
  command -v python3 >/dev/null 2>&1 || { echo "Missing required command: python3" >&2; exit 1; }
}

seed_login() {
  require_auth_tools

  local backend_url="${BACKEND_URL:-http://localhost:5055}"
  local email="${SEED_AUTH_EMAIL:-${AUTH_SEED_ADMIN_EMAIL:-}}"
  local password="${SEED_AUTH_PASSWORD:-${AUTH_SEED_ADMIN_PASSWORD:-}}"

  if [[ -z "${email}" || -z "${password}" ]]; then
    echo "[seed] Missing SEED_AUTH_EMAIL/SEED_AUTH_PASSWORD or AUTH_SEED_ADMIN_EMAIL/AUTH_SEED_ADMIN_PASSWORD" >&2
    exit 1
  fi

  local payload
  payload="$(SEED_LOGIN_EMAIL="${email}" SEED_LOGIN_PASSWORD="${password}" python3 -c '
import json
import os

print(json.dumps({
    "email": os.environ["SEED_LOGIN_EMAIL"],
    "password": os.environ["SEED_LOGIN_PASSWORD"]
}))
')"

  local resp status body
  resp="$(curl -sS -i -X POST "${backend_url}/api/auth/login" \
    -H "Content-Type: application/json" \
    -d "${payload}")"

  status="$(printf '%s' "${resp}" | head -n 1 | awk '{print $2}')"
  body="$(printf '%s' "${resp}" | awk 'BEGIN{p=0} /^\r?$/{p=1;next} {if(p) print}')"

  if [[ ! "${status}" =~ ^2 ]]; then
    echo "[seed] Login failed: ${backend_url}/api/auth/login (status=${status})" >&2
    echo "[seed] Email: ${email}" >&2
    echo "[seed] Response body:" >&2
    printf '%s\n' "${body}" | head -n 80 >&2
    exit 1
  fi

  local token
  token="$(printf '%s' "${body}" | python3 -c '
import json
import sys

data = json.load(sys.stdin)
token = data.get("accessToken") or data.get("token") or ""
print(token)
')"

  if [[ -z "${token}" ]]; then
    echo "[seed] Login succeeded but token is missing in response." >&2
    printf '%s\n' "${body}" >&2
    exit 1
  fi

  printf '%s' "${token}"
}