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

# Legacy outputs (keep for now, used by old ansible/scripts)
output "server_name" {
  description = "Legacy server name (docker-compose path)."
  value       = hcloud_server.real_estate_hub.name
}

output "public_ip" {
  description = "Legacy server public IPv4 (docker-compose path)."
  value       = hcloud_server.real_estate_hub.ipv4_address
}

# k3s public IPs
output "k3s_server_public_ips" {
  description = "Public IPv4 addresses of k3s server (control-plane) nodes."
  value       = hcloud_server.k3s_server[*].ipv4_address
}

output "k3s_agent_public_ips" {
  description = "Public IPv4 addresses of k3s agent (worker) nodes."
  value       = hcloud_server.k3s_agent[*].ipv4_address
}

# k3s private IPs (from hcloud_server_network attachments)
output "k3s_server_private_ips" {
  description = "Private IPv4 addresses of k3s server nodes (Hetzner private network)."
  value       = hcloud_server_network.k3s_server[*].ip
}

output "k3s_agent_private_ips" {
  description = "Private IPv4 addresses of k3s agent nodes (Hetzner private network)."
  value       = hcloud_server_network.k3s_agent[*].ip
}

output "ansible_inventory_hosts" {
  description = "Structured hosts data for Ansible."
  value = {
    k3s_server = [
      for i, s in hcloud_server.k3s_server : {
        name         = s.name
        public_ipv4  = s.ipv4_address
        private_ipv4 = try(hcloud_server_network.k3s_server[i].ip, null)
      }
    ]
    k3s_agent = [
      for i, s in hcloud_server.k3s_agent : {
        name         = s.name
        public_ipv4  = s.ipv4_address
        private_ipv4 = try(hcloud_server_network.k3s_agent[i].ip, null)
      }
    ]
  }
}

output "ansible_inventory_ini" {
  description = "INI-style inventory snippet for Ansible (k3s + legacy)."
  value       = <<EOT
[k3s_server]
%{for ip in hcloud_server.k3s_server[*].ipv4_address~}
${ip} ansible_user=root
%{endfor~}

[k3s_agent]
%{for ip in hcloud_server.k3s_agent[*].ipv4_address~}
${ip} ansible_user=root
%{endfor~}

[legacy_web]
${hcloud_server.real_estate_hub.ipv4_address} ansible_user=root
EOT
}

output "load_balancer_id" {
  value = local.lb_id
}

output "load_balancer_name" {
  value = local.shared_lb_name
}

output "load_balancer_ipv4" {
  value = (
    local.lb_owner
    ? hcloud_load_balancer.shared_prod[0].ipv4
    : data.hcloud_load_balancer.shared_prod[0].ipv4
  )
}

output "k3s_lb_public_ip" {
  description = "Public IPv4 of the shared Load Balancer (entrypoint for k8s ingress)."
  value = (
    local.lb_owner
    ? hcloud_load_balancer.shared_prod[0].ipv4
    : data.hcloud_load_balancer.shared_prod[0].ipv4
  )
}
