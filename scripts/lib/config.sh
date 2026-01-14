#!/usr/bin/env bash
set -euo pipefail

# Default ports (might be overridden by env though)
BACKEND_PORT="${BACKEND_PORT:-5055}"
FRONTEND_PORT="${FRONTEND_PORT:-3000}"
MONGO_PORT="${MONGO_PORT:-27017}"

BACKEND_URL="${BACKEND_URL:-http://localhost:${BACKEND_PORT}}"
FRONTEND_URL="${FRONTEND_URL:-http://localhost:${FRONTEND_PORT}}"
