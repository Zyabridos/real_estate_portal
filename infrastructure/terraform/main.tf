provider "hcloud" {
  token = var.hcloud_token
}

locals {
  name_prefix = "${local.base_name}-${var.env}-${var.stack_id}"

  common_labels = {
    project = local.base_name
    env     = var.env
    stack   = var.stack_id
    # role set per resource (web / k3s-server / k3s-agent)
  }
}

data "hcloud_ssh_key" "main" {
  name = var.ssh_key_name
}

# Legacy - TODO: deprecate when k8s works properly
resource "hcloud_server" "real_estate_hub" {
  name        = "${local.name_prefix}-server"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels   = merge(local.common_labels, { role = "web" })
  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.real_estate_hub_fw.id]
}

# K3S: servers
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

  firewall_ids = [hcloud_firewall.k3s_fw.id]
}

# K3S: agents (workers)
resource "hcloud_server" "k3s_agent" {
  count = var.k8s_enabled ? var.k3s_workers_count : 0

  name        = "${local.name_prefix}-k3s-agent-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(local.common_labels, {
    role = "k3s-agent"
    k8s  = "true"
  })

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw.id]
}
