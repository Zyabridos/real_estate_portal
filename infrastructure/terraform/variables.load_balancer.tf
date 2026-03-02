variable "load_balancer_type" {
  description = "Hetzner Load Balancer type (e.g., lb11, lb21)."
  type        = string
  default     = "lb11"
}

variable "load_balancer_algorithm" {
  description = "Load balancer algorithm: round_robin or least_connections."
  type        = string
  default     = "least_connections"

  validation {
    condition     = contains(["round_robin", "least_connections"], var.load_balancer_algorithm)
    error_message = "load_balancer_algorithm must be 'round_robin' or 'least_connections'."
  }
}

variable "load_balancer_owner_stack" {
  description = "Which stack owns (creates) the shared Load Balancer and its services."
  type        = string
  default     = "blue"

  validation {
    condition     = contains(["blue", "green"], var.load_balancer_owner_stack)
    error_message = "load_balancer_owner_stack must be either 'blue' or 'green'."
  }
}

variable "load_balancer_target_stack" {
  description = "Which stack receives production traffic via the shared Load Balancer (blue|green)."
  type        = string
  default     = "blue"

  validation {
    condition     = contains(["blue", "green"], var.load_balancer_target_stack)
    error_message = "load_balancer_target_stack must be either 'blue' or 'green'."
  }
}