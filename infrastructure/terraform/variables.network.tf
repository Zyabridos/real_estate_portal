variable "k3s_network_ip_range" {
  description = "Main private network CIDR for k3s nodes."
  type        = string
  default     = "10.50.0.0/16"
}

variable "k3s_subnet_ip_range" {
  description = "Subnet CIDR inside main network."
  type        = string
  default     = "10.50.1.0/24"
}

variable "k3s_network_zone" {
  description = "Hetzner network zone for the k3s private subnet (e.g. eu-central)."
  type        = string
  default     = "eu-central"
}

variable "enable_green_stack" {
  description = "When enabled, Terraform creates a second isolated private network/subnet and a separate set of nodes."
  type        = bool
  default     = true
}

variable "k3s_network_ip_range_green" {
  description = "Private network CIDR for GREEN k3s nodes."
  type        = string
  default     = "10.51.0.0/16"
}

variable "k3s_subnet_ip_range_green" {
  description = "Private subnet CIDR for GREEN k3s nodes."
  type        = string
  default     = "10.51.1.0/24"
}

variable "k3s_network_zone_green" {
  description = "Hetzner network zone for GREEN private subnet (e.g. eu-central)."
  type        = string
  default     = "eu-central"
}