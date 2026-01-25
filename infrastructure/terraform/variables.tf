variable "hcloud_token" {
  description = "Hetzner Cloud API token"
  type        = string
  sensitive   = true
}

variable "env" {
  description = "Environment name (dev/stage/prod)"
  type        = string
}

variable "stack_id" {
  description = "Blue/green stack id (blue|green). Used for names/labels."
  type        = string

  validation {
    condition     = contains(["blue", "green"], var.stack_id)
    error_message = "stack_id must be either 'blue' or 'green'."
  }
}

variable "ssh_key_name" {
  description = "Existing SSH key name in Hetzner Cloud"
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

variable "load_balancer_type" {
  description = "Hetzner load balancer type (lb11/lb21/...)"
  type        = string
  default     = "lb11"
}

variable "load_balancer_algorithm" {
  description = "Load balancer algorithm: round_robin or least_connections"
  type        = string
  default     = "round_robin"

  validation {
    condition     = contains(["round_robin", "least_connections"], var.load_balancer_algorithm)
    error_message = "load_balancer_algorithm must be 'round_robin' or 'least_connections'."
  }
}

variable "load_balancer_certificate_id" {
  description = "Optional: Hetzner certificate ID for TLS termination on LB (enables HTTPS service)."
  type        = number
  default     = null
}

