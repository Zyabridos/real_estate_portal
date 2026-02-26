provider "hcloud" {
  token = var.hcloud_token
}

locals {
  base_name   = "real-estate-hub"
  name_prefix = "${local.base_name}-${var.env}-${var.stack_id}"

  common_labels = {
    project = local.base_name
    env     = var.env
    stack   = var.stack_id
  }
}

data "hcloud_ssh_key" "main" {
  name = var.ssh_key_name
}

resource "hcloud_server" "k3s_server" {
  count = var.k8s_enabled ? var.k3s_server_count : 0

  name        = "${local.name_prefix}-k3s-server-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(local.common_labels, {
    role = "k3s-server"
    k8s  = "true"
  })

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw[0].id]
}

resource "hcloud_server" "k3s_worker" {
  count = var.k8s_enabled ? var.k3s_workers_count : 0

  name        = "${local.name_prefix}-k3s-worker-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(local.common_labels, {
    role = "k3s-worker"
    k8s  = "true"
  })

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw[0].id]
}