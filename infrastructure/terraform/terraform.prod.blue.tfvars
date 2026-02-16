env      = "prod"
stack_id = "blue"

server_type = "cx23"
image       = "ubuntu-24.04"
location    = "hel1"

ssh_key_name = "realestate-prod-ssh-key"

# освобождаем слот
legacy_enabled = false

# Blue
k8s_enabled       = true
k3s_server_count  = 1
k3s_workers_count = 1

# Green pre-prod
enable_green_stack      = true
k3s_workers_count_green = 0


load_balancer_owner_stack  = "blue"
load_balancer_target_stack = "blue"
# 
# k3s_server_count   = 1
# k3s_workers_count  = 0
# enable_green_stack = true
# k3s_api_port       = 6443
# 
# # This is TEST-NET-1 (documentation range), so you won't be able to SSH with it.
# ssh_allowed_cidrs = ["84.52.243.200/32"]
# 
# k3s_network_ip_range = "10.50.0.0/16"
# k3s_subnet_ip_range  = "10.50.1.0/24"
# k3s_network_zone     = "eu-central"
