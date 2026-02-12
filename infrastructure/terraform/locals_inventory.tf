locals {
  # stable group names for Ansible
  inventory_group_server = "k3s_server"
  inventory_group_agent  = "k3s_agent"

  # Common host vars (can be extended later)
  ansible_user = "root"

  # Host objects (public/private)
  k3s_server_hosts = [
    for i, s in hcloud_server.k3s_server : {
      name         = s.name
      public_ipv4  = s.ipv4_address
      private_ipv4 = try(hcloud_server_network.k3s_server[i].ip, null)
      ansible_user = local.ansible_user
      role         = "k3s-server"
    }
  ]

  k3s_agent_hosts = [
    for i, s in hcloud_server.k3s_agent : {
      name         = s.name
      public_ipv4  = s.ipv4_address
      private_ipv4 = try(hcloud_server_network.k3s_agent[i].ip, null)
      ansible_user = local.ansible_user
      role         = "k3s-agent"
    }
  ]

  # Optional legacy host (docker-compose path)
  legacy_web_hosts = [
    {
      name         = hcloud_server.real_estate_hub.name
      public_ipv4  = hcloud_server.real_estate_hub.ipv4_address
      private_ipv4 = null
      ansible_user = local.ansible_user
      role         = "web"
    }
  ]

  # A single structured object you can output or render into templates later
  ansible_inventory_struct = {
    env      = var.env
    stack_id = var.stack_id

    groups = {
      (local.inventory_group_server) = local.k3s_server_hosts
      (local.inventory_group_agent)  = local.k3s_agent_hosts
      legacy_web                     = local.legacy_web_hosts
    }
  }
}
