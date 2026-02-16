resource "hcloud_firewall" "k3s_fw" {
  name   = "${local.base_name}-${var.env}-${var.stack_id}-k3s-fw"
  labels = merge(local.common_labels, { role = "k3s-fw" })

  # SSH
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
    source_ips = ["0.0.0.0/0", "::/0"] # everyone (duuugh, but things r easly to forget when one is not working with it regullary)
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "443"
    source_ips = ["0.0.0.0/0", "::/0"]
  }

  # k3s API
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = tostring(var.k3s_api_port)
    source_ips = concat(var.ssh_allowed_cidrs, [var.k3s_subnet_ip_range]) # [ admin IP ranges, cluster nodes/components - allows them talk to the API ])
  }

  # workers -> servers, internal only - nodes must reach the server, but it should not be reachable from outside the cluster subnet.
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "9345"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Flannel VXLAN, internal only. Required when using Flannel VXLAN backend.
  rule {
    direction  = "in"
    protocol   = "udp"
    port       = "8472"
    source_ips = [var.k3s_subnet_ip_range]
  }

  # Kubelet API, internal only - used by control-plane / metrics / logs.
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "10250"
    source_ips = [var.k3s_subnet_ip_range]
  }
}


# --- GREEN
resource "hcloud_firewall" "k3s_fw_green" {
  name   = "${local.base_name}-${var.env}-green-k3s-fw"
  labels = merge(local.green_common_labels, { role = "k3s-fw" })

  # SSH
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

  # k3s API
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = tostring(var.k3s_api_port)
    source_ips = concat(var.ssh_allowed_cidrs, [var.k3s_subnet_ip_range_green])
  }

  # k3s supervisor (workers -> servers)
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "9345"
    source_ips = [var.k3s_subnet_ip_range_green]
  }

  # Flannel VXLAN
  rule {
    direction  = "in"
    protocol   = "udp"
    port       = "8472"
    source_ips = [var.k3s_subnet_ip_range_green]
  }

  # Kubelet
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "10250"
    source_ips = [var.k3s_subnet_ip_range_green]
  }
}
