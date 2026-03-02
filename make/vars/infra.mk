TF 					  ?= terraform
ANSIBLE 			  ?= ansible
ANSIBLE_MAIN_PLAYBOOK ?= playbooks/site.yml
ANSIBLE_BASE_VARS = \
  env=$(ENV) \
  stack_id=$(STACK) \
  image_tag=$(IMAGE_TAG) \
  api_image_tag=$(IMAGE_TAG) \
  frontend_image_tag=$(IMAGE_TAG) \
  cms_image_tag=$(IMAGE_TAG)

ANSIBLE_LIMIT 		?= all
EXTRA_VARS    		?=
VAULT_PASS_FILE 	?= vault-password
ANSIBLE_CONFIG_FILE ?= $(ANSIBLE_DIR)/ansible.cfg

SSH_KEYSCAN      ?= ssh-keyscan
SSH_KEYGEN       ?= ssh-keygen
KNOWN_HOSTS_FILE ?= $(HOME)/.ssh/known_hosts

INFRA_SSH_USER ?= root
INFRA_SSH_HOST ?=
