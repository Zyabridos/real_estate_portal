output "stack" {
  description = "Stack id (blue/green)"
  value       = var.stack_id
}

output "load_balancer_owner_stack" {
  value = var.load_balancer_owner_stack
}

output "load_balancer_target_stack" {
  value = var.load_balancer_target_stack
}

# k3s public IPs
output "k3s_server_public_ips" {
  description = "Public IPv4 of BLUE k3s servers."
  value       = hcloud_server.k3s_server[*].ipv4_address
}

output "k3s_worker_public_ips" {
  description = "Public IPv4 of BLUE k3s workers."
  value       = hcloud_server.k3s_worker[*].ipv4_address
}

# k3s private IPs
output "k3s_server_private_ips" {
  description = "Private IPv4 of BLUE k3s servers."
  value       = hcloud_server_network.k3s_server[*].ip
}

output "k3s_worker_private_ips" {
  description = "Private IPv4 of BLUE k3s workers."
  value       = hcloud_server_network.k3s_worker[*].ip
}

# GREEN IPs
output "k3s_green_server_public_ips" {
  value = hcloud_server.k3s_server_green[*].ipv4_address
}

output "k3s_green_worker_public_ips" {
  value = hcloud_server.k3s_worker_green[*].ipv4_address
}

output "k3s_green_server_private_ips" {
  value = hcloud_server_network.k3s_server_green[*].ip
}

output "k3s_green_worker_private_ips" {
  value = hcloud_server_network.k3s_worker_green[*].ip
}

# LB outputs (owner-only)
output "load_balancer_id" {
  value = try(hcloud_load_balancer.shared[0].id, null)
}

output "load_balancer_name" {
  value = try(hcloud_load_balancer.shared[0].name, null)
}

output "load_balancer_ipv4" {
  value = try(hcloud_load_balancer.shared[0].ipv4, null)
}

output "k3s_lb_public_ip" {
  description = "Public IPv4 of the shared Load Balancer."
  value       = try(hcloud_load_balancer.shared[0].ipv4, null)
}

output "ansible_inventory_ini" {
  value = <<EOT
[k3s_server]
%{for i, s in hcloud_server.k3s_server~}
${s.ipv4_address} ansible_user=root private_ipv4=${try(hcloud_server_network.k3s_server[i].ip, "")}
%{endfor~}

[k3s_workers]
%{for i, s in hcloud_server.k3s_worker~}
${s.ipv4_address} ansible_user=root private_ipv4=${try(hcloud_server_network.k3s_worker[i].ip, "")}
%{endfor~}

[k3s_server_green]
%{for i, s in hcloud_server.k3s_server_green~}
${s.ipv4_address} ansible_user=root private_ipv4=${try(hcloud_server_network.k3s_server_green[i].ip, "")}
%{endfor~}

[k3s_workers_green]
%{for i, s in hcloud_server.k3s_worker_green~}
${s.ipv4_address} ansible_user=root private_ipv4=${try(hcloud_server_network.k3s_worker_green[i].ip, "")}
%{endfor~}
EOT
}