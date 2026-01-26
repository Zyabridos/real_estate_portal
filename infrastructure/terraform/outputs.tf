output "stack" {
  description = "Stack id (blue/green)"
  value       = var.stack_id
}

output "server_name" {
  description = "Server name"
  value       = hcloud_server.real_estate_hub.name
}

output "public_ip" {
  description = "Public IPv4"
  value       = hcloud_server.real_estate_hub.ipv4_address
}

output "ansible_inventory_ini" {
  value = <<EOT
[realestate_${var.env}_${var.stack_id}]
${hcloud_server.real_estate_hub.ipv4_address} ansible_user=root
EOT
}

output "load_balancer_id" {
  value = local.lb_id
}

output "load_balancer_name" {
  value = local.shared_lb_name
}

# IPv4 LB (в обоих workspaces)
output "load_balancer_ipv4" {
  value = (
    local.lb_owner
    ? hcloud_load_balancer.shared_prod[0].ipv4
    : data.hcloud_load_balancer.shared_prod[0].ipv4
  )
}

output "public_entrypoint" {
  value = "http://${(
    local.lb_owner
    ? hcloud_load_balancer.shared_prod[0].ipv4
    : data.hcloud_load_balancer.shared_prod[0].ipv4
  )}"
}
