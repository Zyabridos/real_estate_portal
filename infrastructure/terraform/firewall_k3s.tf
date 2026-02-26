resource "hcloud_firewall" "k3s_fw" {
  count  = var.k8s_enabled ? 1 : 0
  name   = "${local.base_name}-${var.env}-${var.stack_id}-k3s-fw"
  labels = merge(local.common_labels, { role = "k3s-fw" })

  # SSH (admin)
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "22"
    source_ips = var.ssh_allowed_cidrs
  }

  # HTTP/HTTPS from private subnet (LB -> nodes)
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "80"
    source_ips = [var.k3s_subnet_ip_range]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "443"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # k3s API
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = tostring(var.k3s_api_port)
    source_ips = concat(var.ssh_allowed_cidrs, [var.k3s_subnet_ip_range])
  }

  # k3s supervisor (agents -> servers)
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "9345"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Flannel VXLAN
  rule {
    direction  = "in"
    protocol   = "udp"
    port       = "8472"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Kubelet API
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "10250"
    source_ips = [var.k3s_subnet_ip_range]
  }
}