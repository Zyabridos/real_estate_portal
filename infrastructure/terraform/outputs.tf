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

output "load_balancer_name" {
  description = "Load balancer name"
  value       = hcloud_load_balancer.real_estate_hub.name
}

output "load_balancer_ipv4" {
  description = "Load balancer public IPv4"
  value       = hcloud_load_balancer.real_estate_hub.ipv4
}

output "public_entrypoint" {
  description = "Suggested public entrypoint"
  value       = "http://${hcloud_load_balancer.real_estate_hub.ipv4}"
}
