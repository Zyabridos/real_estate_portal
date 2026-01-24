terraform {
  required_version = ">= 1.5.0"

  required_providers {
    hcloud = {
      source  = "hetznercloud/hcloud"
      version = "~> 1.48"
    }
  }
}

provider "hcloud" {
  token = var.hcloud_token
}

locals {
  common_labels = {
    project = "real-estate-hub"
    env     = var.env
  }
}

resource "hcloud_ssh_key" "main" {
  name       = var.ssh_key_name
  public_key = var.ssh_public_key
}

resource "hcloud_firewall" "real-estate_hub_fw" {
  name = "real-estate-hub-fw"

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

resource "hcloud_server" "real-estate_hub_prod" {
  name        = "real-estate-hub-prod"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels   = local.common_labels
  ssh_keys = [hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.real-estate_hub_fw.id]
}
