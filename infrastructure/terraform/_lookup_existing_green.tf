data "hcloud_network" "existing_green" {
  name = "real-estate-hub-prod-green-k3s-net"
}

data "hcloud_firewall" "existing_green" {
  name = "real-estate-hub-prod-green-k3s-fw"
}

data "hcloud_server" "existing_green_server" {
  name = "real-estate-hub-prod-green-k3s-server-1"
}