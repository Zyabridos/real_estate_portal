output "location" {
  description = "Hetzner location for created resources."
  value       = var.location
}

output "public_ip" {
  description = "Public IPv4 address of the main server."
  value       = hcloud_server.real-estate_hub_prod.ipv4_address
}

output "server_name" {
  description = "Name of the main server."
  value       = hcloud_server.real-estate_hub_prod.name
}

output "ansible_inventory_ini" {
  description = "Inventory-like output (INI format) for quick Ansible usage."
  value       = <<EOT
[realestate_prod]
${hcloud_server.real-estate_hub_prod.ipv4_address} ansible_user=root
EOT
}
