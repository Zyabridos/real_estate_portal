locals {
  base_name = "real-estate-hub"

  lb_owner       = var.stack_id == var.load_balancer_owner_stack
  shared_lb_name = "${local.base_name}-${var.env}-${var.load_balancer_owner_stack}-lb"

  shared_lb_labels = {
    project = local.base_name
    env     = var.env
    role    = "public-entry"
  }

  # IMPORTANT:
  # - must include stack=... to avoid routing to BOTH blue+green
  # - if k8s_enabled => target k8s nodes (k8s=true), else legacy web (role=web)
  lb_target_kind        = var.k8s_enabled ? "k8s=true" : "role=web"
  server_label_selector = "project=${local.base_name},env=${var.env},stack=${var.load_balancer_target_stack},${local.lb_target_kind}"
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

resource "hcloud_load_balancer_target" "targets" {
  count            = local.lb_owner ? 1 : 0
  load_balancer_id = local.lb_id
  type             = "label_selector"
  label_selector   = local.server_label_selector
}

# HTTP 80 -> target:80
resource "hcloud_load_balancer_service" "http" {
  count            = local.lb_owner ? 1 : 0
  load_balancer_id = local.lb_id
  protocol         = "http"
  listen_port      = 80
  destination_port = 80

  # For k8s we don't want to depend on a specific path existing yet,
  # so use TCP health-check (port open == healthy).
  health_check {
    protocol = "tcp"
    port     = 80
    interval = 10
    timeout  = 5
    retries  = 3
  }
}

# 443 passthrough -> target:443 (TLS at ingress / node)
resource "hcloud_load_balancer_service" "https_tcp" {
  count            = local.lb_owner ? 1 : 0
  load_balancer_id = local.lb_id
  protocol         = "tcp"
  listen_port      = 443
  destination_port = 443

  health_check {
    protocol = "tcp"
    port     = 443
    interval = 10
    timeout  = 5
    retries  = 3
  }
}
