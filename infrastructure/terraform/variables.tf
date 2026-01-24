variable "hcloud_token" {
  description = "Hetzner Cloud API token"
  type        = string
  sensitive   = true
}

variable "ssh_key_name" {
  description = "Name of SSH key in Hetzner Cloud"
  type        = string
}

variable "server_type" {
  description = "Hetzner server type"
  type        = string
  default     = "cx23"
}

variable "image" {
  description = "Image name (ubuntu-24.04, etc.)"
  type        = string
  default     = "ubuntu-24.04"
}

variable "location" {
  description = "Hetzner location"
  type        = string
  default     = "hel1"
}

variable "env" {
  description = "Environment name (dev/stage/prod)."
  type        = string
  default     = "prod"
}

variable "ssh_public_key" {
  description = "Public SSH key in OpenSSH format (~/.ssh/id_of_public_key.pub)."
  type        = string
  sensitive   = true
}
