data "hcloud_network" "blue_net" {
  count = (
    var.load_balancer_owner_stack == var.stack_id &&
    var.load_balancer_target_stack == "blue" &&
    var.stack_id != "blue"
  ) ? 1 : 0

  name = "${var.base_name}-${var.env}-blue-k3s-net"
}

data "hcloud_network" "green_net" {
  count = (
    var.load_balancer_owner_stack == var.stack_id &&
    var.load_balancer_target_stack == "green" &&
    var.stack_id != "green"
  ) ? 1 : 0

  name = "${var.base_name}-${var.env}-green-k3s-net"
}

resource "hcloud_load_balancer" "shared" {
  count              = (var.load_balancer_owner_stack == var.stack_id) ? 1 : 0
  name               = "${var.base_name}-${var.env}-lb"
  load_balancer_type = var.load_balancer_type
  location           = var.location

  algorithm {
    type = var.load_balancer_algorithm
  }

  lifecycle {
    prevent_destroy = true
  }

  labels = {
    project = var.base_name
    env     = var.env
    role    = "public-entry"
  }
}

# Attach LB to ONE private network (ACTIVE stack)
resource "hcloud_load_balancer_network" "lb_blue" {
  count                   = (var.load_balancer_owner_stack == var.stack_id && var.load_balancer_target_stack == "blue") ? 1 : 0
  load_balancer_id        = hcloud_load_balancer.shared[0].id
  network_id              = (var.stack_id == "blue") ? hcloud_network.k3s[0].id : data.hcloud_network.blue_net[0].id
  enable_public_interface = true
}

resource "hcloud_load_balancer_network" "lb_green" {
  count                   = (var.load_balancer_owner_stack == var.stack_id && var.load_balancer_target_stack == "green") ? 1 : 0
  load_balancer_id        = hcloud_load_balancer.shared[0].id
  network_id              = (var.stack_id == "green") ? hcloud_network.k3s_green[0].id : data.hcloud_network.green_net[0].id
  enable_public_interface = true
}

resource "hcloud_load_balancer_target" "targets" {
  count            = (var.load_balancer_owner_stack == var.stack_id) ? 1 : 0
  load_balancer_id = hcloud_load_balancer.shared[0].id
  type             = "label_selector"
  label_selector   = "project=${var.base_name},env=${var.env},stack=${var.load_balancer_target_stack},k8s=true,role=k3s-worker"
  use_private_ip   = true

  depends_on = [
    hcloud_load_balancer_network.lb_blue,
    hcloud_load_balancer_network.lb_green
  ]
}

# Expose HTTP (TCP/80) on the shared Hetzner Load Balancer.
resource "hcloud_load_balancer_service" "http" {
  count            = (var.load_balancer_owner_stack == var.stack_id) ? 1 : 0
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
  count            = (var.load_balancer_owner_stack == var.stack_id) ? 1 : 0
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