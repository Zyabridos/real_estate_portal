locals {
  base_name = "real-estate-hub"
  
  lb_owner = var.stack_id == var.lb_owner_stack
  shared_lb_name = "${local.base_name}-${var.env}-${var.lb_owner_stack}-lb"
  
  shared_lb_labels = {
    project = local.base_name
    env     = var.env
    role    = "public-entry"
  }
  server_label_selector = "project=${local.base_name},env=${var.env},role=web"
}

resource "hcloud_load_balancer" "shared_prod" {
  count              = local.lb_owner ? 1 : 0
  name               = local.shared_lb_name
  load_balancer_type = var.load_balancer_type
  location           = var.location
  
  labels = local.shared_lb_labels

  algorithm {
    type = var.load_balancer_algorithm
  }

  lifecycle {
    prevent_destroy = true
  }
}

data "hcloud_load_balancer" "shared_prod" {
  count = local.lb_owner ? 0 : 1
  name  = local.shared_lb_name
}

locals {
  lb_id = (
    local.lb_owner
    ? hcloud_load_balancer.shared_prod[0].id
    : data.hcloud_load_balancer.shared_prod[0].id
  )
}

resource "hcloud_load_balancer_target" "all_web" {
  count            = local.lb_owner ? 1 : 0
  load_balancer_id = local.lb_id
  type             = "label_selector"
  label_selector   = local.server_label_selector
}

# HTTP 80 -> server:80
resource "hcloud_load_balancer_service" "http" {
  count            = local.lb_owner ? 1 : 0
  load_balancer_id = local.lb_id
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

# 443 passthrough -> server:443 (TLS on Caddy)
# remember that Health-check is on HTTP /lb-health на :80
resource "hcloud_load_balancer_service" "https_tcp" {
  count            = local.lb_owner ? 1 : 0
  load_balancer_id = local.lb_id
  protocol         = "tcp"
  listen_port      = 443
  destination_port = 443

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
