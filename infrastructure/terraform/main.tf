provider "hcloud" {
  token = var.hcloud_token
}

locals {
  base_name   = "real-estate-hub"
  name_prefix = "${local.base_name}-${var.env}-${var.stack_id}"

  common_labels = {
    project = "real-estate-hub"
    env     = var.env
    stack   = var.stack_id
  }
}

# Use an existing SSH key (recommended for blue/green; avoids uniqueness conflicts)
data "hcloud_ssh_key" "main" {
  name = var.ssh_key_name
}

resource "hcloud_firewall" "real_estate_hub_fw" {
  name   = "${local.name_prefix}-fw"
  labels = local.common_labels

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "22"
    source_ips = ["0.0.0.0/0", "::/0"]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "80"
    source_ips = ["0.0.0.0/0", "::/0"]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "443"
    source_ips = ["0.0.0.0/0", "::/0"]
  }
}

resource "hcloud_server" "real_estate_hub" {
  name        = "${local.name_prefix}-server"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels   = local.common_labels
  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.real_estate_hub_fw.id]
}
