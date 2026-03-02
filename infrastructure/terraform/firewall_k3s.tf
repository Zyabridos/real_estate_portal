resource "hcloud_firewall" "k3s_fw" {
  count = (var.k8s_enabled && var.stack_id == "blue") ? 1 : 0
  name  = "${var.base_name}-${var.env}-blue-k3s-fw"
  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    { role = "k3s-fw" }
  )

  # SSH
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "22"
    source_ips = var.ssh_allowed_cidrs
  }

  # Private subnet (BLUE)
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

  # k3s supervisor (workers -> servers)
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

  # Kubelet
  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "10250"
    source_ips = [var.k3s_subnet_ip_range]
  }
}

# --- GREEN
resource "hcloud_firewall" "k3s_fw_green" {
  count = (var.k8s_enabled && var.enable_green_stack && var.stack_id == "green") ? 1 : 0
  name  = "${var.base_name}-${var.env}-green-k3s-fw"
  labels = merge(
    {
      project = var.base_name
      env     = var.env
      stack   = var.stack_id
    },
    { role = "k3s-fw" }
  )

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "22"
    source_ips = var.ssh_allowed_cidrs
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "80"
    source_ips = [var.k3s_subnet_ip_range_green]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "443"
    source_ips = [var.k3s_subnet_ip_range_green]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = tostring(var.k3s_api_port)
    source_ips = concat(var.ssh_allowed_cidrs, [var.k3s_subnet_ip_range_green])
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "9345"
    source_ips = [var.k3s_subnet_ip_range_green]
  }

  rule {
    direction  = "in"
    protocol   = "udp"
    port       = "8472"
    source_ips = [var.k3s_subnet_ip_range_green]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "10250"
    source_ips = [var.k3s_subnet_ip_range_green]
  }
}