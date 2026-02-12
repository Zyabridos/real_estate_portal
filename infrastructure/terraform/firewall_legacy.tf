# TODO: remove when migration to k3s completed
resource "hcloud_firewall" "real_estate_hub_fw" {
  name   = "${local.base_name}-${var.env}-${var.stack_id}-legacy-fw"
  labels = merge(local.common_labels, { role = "legacy-fw" })

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
    source_ips = ["0.0.0.0/0", "::/0"]
  }

  rule {
    direction  = "in"
    protocol   = "tcp"
    port       = "443"
    source_ips = ["0.0.0.0/0", "::/0"]
  }
}
