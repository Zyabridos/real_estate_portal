provider "hcloud" {
  token = var.hcloud_token
}

data "hcloud_ssh_key" "main" {
  name = var.ssh_key_name
}

# --- BLUE servers
resource "hcloud_server" "k3s_server" {
  count = (var.k8s_enabled && var.stack_id == "blue") ? var.k3s_server_count : 0

  name        = "${var.base_name}-${var.env}-${var.stack_id}-k3s-server-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    {
      role = "k3s-server"
      k8s  = "true"
    }
  )

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw[0].id]
}

resource "hcloud_server" "k3s_worker" {
  count = (var.k8s_enabled && var.stack_id == "blue") ? var.k3s_workers_count : 0

  name        = "${var.base_name}-${var.env}-${var.stack_id}-k3s-worker-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    {
      role = "k3s-worker"
      k8s  = "true"
    }
  )

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw[0].id]
}

# --- GREEN servers
resource "hcloud_server" "k3s_server_green" {
  count = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? var.k3s_server_count : 0

  name        = "${var.base_name}-${var.env}-${var.stack_id}-k3s-server-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    {
      role = "k3s-server"
      k8s  = "true"
    }
  )

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw_green[0].id]
}

resource "hcloud_server" "k3s_worker_green" {
  count = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? var.k3s_workers_count_green : 0

  name        = "${var.base_name}-${var.env}-${var.stack_id}-k3s-worker-${count.index + 1}"
  server_type = var.server_type
  image       = var.image
  location    = var.location

  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    {
      role = "k3s-worker"
      k8s  = "true"
    }
  )

  ssh_keys = [data.hcloud_ssh_key.main.id]

  public_net {
    ipv4_enabled = true
    ipv6_enabled = false
  }

  firewall_ids = [hcloud_firewall.k3s_fw_green[0].id]
}