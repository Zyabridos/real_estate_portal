resource "hcloud_network" "k3s" {
  count    = var.k8s_enabled ? 1 : 0
  name     = "${local.base_name}-${var.env}-${var.stack_id}-k3s-net"
  ip_range = var.k3s_network_ip_range

  labels = merge(local.common_labels, { role = "k3s-net" })
}

resource "hcloud_network_subnet" "k3s" {
  count        = var.k8s_enabled ? 1 : 0
  type         = "cloud"
  network_id   = hcloud_network.k3s[0].id
  network_zone = var.k3s_network_zone
  ip_range     = var.k3s_subnet_ip_range
}

resource "hcloud_server_network" "k3s_server" {
  count     = var.k8s_enabled ? var.k3s_server_count : 0
  server_id = hcloud_server.k3s_server[count.index].id
  subnet_id = hcloud_network_subnet.k3s[0].id
}

resource "hcloud_server_network" "k3s_agent" {
  count     = var.k8s_enabled ? var.k3s_workers_count : 0
  server_id = hcloud_server.k3s_agent[count.index].id
  subnet_id = hcloud_network_subnet.k3s[0].id
}
