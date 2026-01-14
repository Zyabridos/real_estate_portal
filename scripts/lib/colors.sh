#!/usr/bin/env bash

# Reset
RESET="\033[0m"

# Styles
BOLD="\033[1m"
DIM="\033[2m"

# Colors
RED="\033[1;31m"
GREEN="\033[1;32m"
YELLOW="\033[1;33m"
BLUE="\033[1;34m"
PURPLE="\033[1;35m"
LIGHT_BLUE="\033[1;36m"

# Helpers
info() {
  echo -e "${LIGHT_BLUE}$*${RESET}"
}

success() {
  echo -e "${GREEN}$*${RESET}"
}

warn() {
  echo -e "${YELLOW}$*${RESET}"
}

error() {
  echo -e "${RED}$*${RESET}" >&2
}

highlight() {
  echo -e "${PURPLE}$*${RESET}"
}
