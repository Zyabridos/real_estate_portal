variable "hcloud_token" {
  description = "Hetzner Cloud API token used by Terraform to manage resources."
  type        = string
  sensitive   = true
}

variable "env" {
  description = "Deployment environment (dev/stage/prod). Used for naming and labels."
  type        = string
  default     = "prod"
}

variable "stack_id" {
  description = "Blue/green stack identifier (blue|green). Used for resource names and labels."
  type        = string

  validation {
    condition     = contains(["blue", "green"], var.stack_id)
    error_message = "stack_id must be either 'blue' or 'green'."
  }
}

variable "ssh_key_name" {
  description = "Name of an existing SSH key in Hetzner Cloud to attach to provisioned servers."
  type        = string
}

variable "server_type" {
  description = "Hetzner Cloud server type for VMs (e.g., cx23)."
  type        = string
  default     = "cx23"
}

variable "image" {
  description = "Base OS image name for servers (e.g., ubuntu-24.04)."
  type        = string
  default     = "ubuntu-24.04"
}

variable "location" {
  description = "Hetzner location/region for resources (e.g., hel1, nbg1, fsn1)."
  type        = string
  default     = "hel1"
}

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

variable "load_balancer_certificate_id" {
  description = "Optional Hetzner certificate ID to enable TLS termination on the Load Balancer."
  type        = number
  default     = null
}

variable "load_balancer_owner_stack" {
  description = "Which stack owns (creates) the shared Load Balancer and its services. Usually 'blue'."
  type        = string
  default     = "blue"

  validation {
    condition     = contains(["blue", "green"], var.load_balancer_owner_stack)
    error_message = "load_balancer_owner_stack must be either 'blue' or 'green'"
  }
}

variable "load_balancer_target_stack" {
  description = "Which stack receives production traffic via the shared Load Balancer (blue|green)."
  type        = string
  default     = "blue"

  validation {
    condition     = contains(["blue", "green"], var.load_balancer_target_stack)
    error_message = "load_balancer_target_stack must be either 'blue' or 'green'"
  }
}

# k3s / k8s
variable "k8s_enabled" {
  description = "Enable provisioning of k3s-ready nodes (k3s server/agent groups) and related outputs."
  type        = bool
  default     = true
}

variable "k3s_server_count" {
  description = "Number of k3s server (control-plane) nodes."
  type        = number
  default     = 1

  validation {
    condition     = var.k3s_server_count >= 0
    error_message = "k3s_server_count must be >= 0."
  }
}

variable "k3s_workers_count" {
  description = "Number of k3s agents nodes"
  type        = number
  default     = 1

  validation {
    condition     = var.k3s_workers_count >= 0
    error_message = "k3s_workers_count must be >= 0."
  }
}

variable "k3s_api_port" {
  description = "Kubernetes API port for k3s"
  type        = number
  default     = 6443
}

variable "ssh_allowed_cidrs" {
  description = "CIDR allowlist for SSH access to servers (tcp/22)"
  type        = list(string)

  validation {
    condition     = alltrue([for c in var.ssh_allowed_cidrs : can(cidrhost(c, 0))])
    error_message = "ssh_allowed_cidrs must contain valid CIDR blocks (e.g., 1.2.3.4/32)."
  }
}

# for internal traffic and private IP outputs
variable "k3s_network_ip_range" {
  description = "Main private network CIDR for k3s nodes"
  type        = string
  default     = "10.50.0.0/16" # 10.50.0.0 – 10.50.255.255
}

variable "k3s_subnet_ip_range" {
  description = "Subnet CIDR inside main network"
  type        = string
  default     = "10.50.1.0/24" # 10.50.1.0 – 10.50.1.255
}

variable "k3s_network_zone" {
  description = "Hetzner network zone for the k3s private subnet"
  type        = string
  default     = "eu-central"
}
