#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
source "${SCRIPT_DIR}/../lib/log.sh"

echo ""
highlight "=================================================="
success "   Real Estate Project — Available Commands"
highlight "=================================================="
echo ""

warn "Local development:"
info "  dev-backend             Run backend locally (dotnet watch)"
info "  dev-frontend            Run frontend locally (Vite)"
info "  dev-cms                 Run Sanity Studio locally"
info "  dev                     Run backend + frontend + cms"
echo ""

warn "API / Ports:"
info "  ping-api                Check backend health endpoint"
info "  ping-properties         Fetch properties list"
info "  ping-property ID=<id>   Fetch property by id"
info "  smoke-api               Run basic API smoke checks"
echo ""

warn "Reset / bootstrap (local):"
info "  backend-full-rebuild    Full backend reset (NuGet, bin/obj, rebuild & run)"
info "  frontend-clean-install  Full frontend reset (node_modules reinstall)"
echo ""

warn "Testing:"
info "  test-back               Run backend test suite"
echo ""

warn "Docker commands:"
info "  build                   Build Docker images"
info "  up                      Start all services (foreground)"
info "  up-d                    Start all services (background)"
info "  down                    Stop and remove all containers"
info "  rebuild                 Stop, rebuild and restart all services"
echo ""

warn "Docker service management:"
info "  restart                 Restart core services"
info "  restart-backend         Restart backend container"
info "  restart-frontend        Restart frontend container"
info "  restart-db              Restart mongodb container"
info "  restart-with-cms        Restart core services + cms"
echo ""

warn "Shell inside containers:"
info "  sh-backend              Shell into backend container"
info "  sh-frontend             Shell into frontend container"
echo ""

warn "Cleanup:"
info "  prune                   Remove unused Docker resources"
error "  clean                   Full Docker cleanup (DANGER)"
echo ""

highlight "Usage:"
info "  make <command>"
echo ""
