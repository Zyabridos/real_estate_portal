env      = "prod"
stack_id = "green"

server_type = "cx23"
image       = "ubuntu-24.04"
location    = "hel1"

ssh_key_name = "realestate-prod-ssh-key"

legacy_enabled = false

# LB is still owned by blue, but green can exist as a stack in parallel.
load_balancer_owner_stack  = "blue"
load_balancer_target_stack = "blue"

k8s_enabled       = true
k3s_server_count  = 1
k3s_workers_count = 1
k3s_api_port      = 6443

# This is TEST-NET-1 (documentation range), so you won't be able to SSH with it.
ssh_allowed_cidrs = ["192.0.2.0/24"]

k3s_network_ip_range = "10.50.0.0/16"
k3s_subnet_ip_range  = "10.50.1.0/24"
k3s_network_zone     = "eu-central"
