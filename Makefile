include make/_vars.mk
include make/_help.mk

include make/debug.mk
include make/dev.mk
include make/docker.mk
include make/infrastructure.mk
include make/k8s.mk
include make/pings.mk
include make/seeds.mk

SHELL := /bin/bash
.ONESHELL:
.SHELLFLAGS := -eu -o pipefail -c
MAKEFLAGS += --no-builtin-rules
.DEFAULT_GOAL := help
