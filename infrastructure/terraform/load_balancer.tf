resource "hcloud_load_balancer" "real_estate_hub" {
  name               = "${local.name_prefix}-lb"
  load_balancer_type = var.load_balancer_type
  location           = var.location

  labels = local.common_labels

  algorithm {
    type = var.load_balancer_algorithm
  }

  lifecycle {
    prevent_destroy = true
  }
}

# choose server by label
resource "hcloud_load_balancer_target" "real_estate_hub" {
  load_balancer_id = hcloud_load_balancer.real_estate_hub.id
  type             = "label_selector"
  label_selector   = "project=${local.base_name},env=${var.env},stack=${var.stack_id}"
}

# HTTP (80) -> :80
resource "hcloud_load_balancer_service" "http" {
  load_balancer_id = hcloud_load_balancer.real_estate_hub.id
  protocol         = "http"
  listen_port      = 80
  destination_port = 80

  health_check {
    protocol = "http"
    port     = 80
    interval = 10
    timeout  = 5
    retries  = 3

    http {
      path         = "/lb-health"
      status_codes = ["200"]
    }
  }
}

# HTTPS (443) with TLS termination on LB (опционально).
resource "hcloud_load_balancer_service" "https" {
  count            = var.load_balancer_certificate_id == null ? 0 : 1
  load_balancer_id = hcloud_load_balancer.real_estate_hub.id
  protocol         = "https"
  listen_port      = 443
  destination_port = 80

  http {
    certificates  = [var.load_balancer_certificate_id]
    redirect_http = true
  }

  health_check {
    protocol = "http"
    port     = 80
    interval = 10
    timeout  = 5
    retries  = 3

    http {
      path         = "/lb-health"
      status_codes = ["200"]
    }
  }
}
