locals {
  is_lb_owner = var.load_balancer_owner_stack == var.stack_id

  lb_name = "${local.base_name}-${var.env}-lb"

  # IMPORTANT: should match ONLY ONE stack at a time (blue OR green)
  lb_label_selector = "project=${local.base_name},env=${var.env},stack=${var.load_balancer_target_stack},k8s=true"

  green_network_name = "${local.base_name}-${var.env}-green-k3s-net"
}

resource "hcloud_load_balancer" "shared" {
  count              = local.is_lb_owner ? 1 : 0
  name               = local.lb_name
  load_balancer_type = var.load_balancer_type
  location           = var.location

  algorithm {
    type = var.load_balancer_algorithm
  }

  lifecycle {
    prevent_destroy = true
  }

  labels = {
    project = local.base_name
    env     = var.env
    role    = "public-entry"
  }
}

# Needed when LB targets GREEN: the GREEN network is created in prod-green workspace,
# but LB is owned by prod-blue. So we look it up by name.
data "hcloud_network" "k3s_green" {
  count = local.is_lb_owner && var.load_balancer_target_stack == "green" ? 1 : 0
  name  = local.green_network_name
}

resource "hcloud_load_balancer_network" "lb_blue" {
  count                  = local.is_lb_owner && var.load_balancer_target_stack == "blue" ? 1 : 0
  load_balancer_id       = hcloud_load_balancer.shared[0].id
  network_id             = hcloud_network.k3s[0].id
  enable_public_interface = true
}

resource "hcloud_load_balancer_network" "lb_green" {
  count                  = local.is_lb_owner && var.load_balancer_target_stack == "green" ? 1 : 0
  load_balancer_id       = hcloud_load_balancer.shared[0].id
  network_id             = data.hcloud_network.k3s_green[0].id
  enable_public_interface = true
}

resource "hcloud_load_balancer_target" "targets" {
  count            = local.is_lb_owner ? 1 : 0
  load_balancer_id = hcloud_load_balancer.shared[0].id
  type             = "label_selector"
  label_selector   = local.lb_label_selector

  # LB -> nodes over private IP
  use_private_ip = true

  depends_on = [
    hcloud_load_balancer_network.lb_blue,
    hcloud_load_balancer_network.lb_green
  ]
}

resource "hcloud_load_balancer_service" "http" {
  count            = local.is_lb_owner ? 1 : 0
  load_balancer_id = hcloud_load_balancer.shared[0].id
  protocol         = "tcp"
  listen_port      = 80
  destination_port = 80

  health_check {
    protocol = "tcp"
    port     = 80
    interval = 10
    timeout  = 5
    retries  = 3
  }
}

resource "hcloud_load_balancer_service" "https_tcp" {
  count            = local.is_lb_owner ? 1 : 0
  load_balancer_id = hcloud_load_balancer.shared[0].id
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