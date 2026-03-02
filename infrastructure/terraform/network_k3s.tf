# --- BLUE network
resource "hcloud_network" "k3s" {
  count    = (var.k8s_enabled && var.stack_id == "blue") ? 1 : 0
  name     = "${var.base_name}-${var.env}-blue-k3s-net"
  ip_range = var.k3s_network_ip_range

  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    { role = "k3s-net" }
  )
}

resource "hcloud_network_subnet" "k3s" {
  count        = (var.k8s_enabled && var.stack_id == "blue") ? 1 : 0
  type         = "cloud"
  network_id   = hcloud_network.k3s[0].id
  network_zone = var.k3s_network_zone
  ip_range     = var.k3s_subnet_ip_range
}

resource "hcloud_server_network" "k3s_server" {
  count     = (var.k8s_enabled && var.stack_id == "blue") ? var.k3s_server_count : 0
  server_id = hcloud_server.k3s_server[count.index].id
  subnet_id = hcloud_network_subnet.k3s[0].id
}

resource "hcloud_server_network" "k3s_worker" {
  count     = (var.k8s_enabled && var.stack_id == "blue") ? var.k3s_workers_count : 0
  server_id = hcloud_server.k3s_worker[count.index].id
  subnet_id = hcloud_network_subnet.k3s[0].id
}

# --- GREEN network
resource "hcloud_network" "k3s_green" {
  count    = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? 1 : 0
  name     = "${var.base_name}-${var.env}-green-k3s-net"
  ip_range = var.k3s_network_ip_range_green

  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    { role = "k3s-net" }
  )
}

resource "hcloud_network_subnet" "k3s_green" {
  count        = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? 1 : 0
  type         = "cloud"
  network_id   = hcloud_network.k3s_green[0].id
  network_zone = var.k3s_network_zone_green
  ip_range     = var.k3s_subnet_ip_range_green
}

resource "hcloud_server_network" "k3s_server_green" {
  count     = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? var.k3s_server_count : 0
  server_id = hcloud_server.k3s_server_green[count.index].id
  subnet_id = hcloud_network_subnet.k3s_green[0].id
}

resource "hcloud_server_network" "k3s_worker_green" {
  count     = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? var.k3s_workers_count_green : 0
  server_id = hcloud_server.k3s_worker_green[count.index].id
  subnet_id = hcloud_network_subnet.k3s_green[0].id
}