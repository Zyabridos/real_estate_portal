resource "hcloud_firewall" "k3s_fw" {
  name   = "${local.base_name}-${var.env}-${var.stack_id}-k3s-fw"
  labels = merge(local.common_labels, { role = "k3s-fw" })

  # SSH (admin only)
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "22"
    source_ips = var.ssh_allowed_cidrs
  }

  # Public ingress
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

  # k3s API (allow from admin CIDRs + private subnet)
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = tostring(var.k3s_api_port)
    source_ips = concat(var.ssh_allowed_cidrs, [var.k3s_subnet_ip_range])
  }

  # k3s supervisor (agents -> servers), internal only
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "9345"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Flannel VXLAN (if used), internal only
  rule {
    direction  = "in"
    protocol   = "udp"
    port       = "8472"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Kubelet, internal only
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "10250"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Embedded etcd (if you go HA later), internal only
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "2379-2380"
    source_ips = [var.k3s_subnet_ip_range]
  }
}
