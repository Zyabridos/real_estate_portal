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
  description = "Number of k3s agents nodes."
  type        = number
  default     = 1

  validation {
    condition     = var.k3s_workers_count >= 0
    error_message = "k3s_workers_count must be >= 0."
  }
}

variable "k3s_workers_count_green" {
  description = "Number of GREEN k3s workers (pre-prod)."
  type        = number
  default     = 0

  validation {
    condition     = var.k3s_workers_count_green >= 0
    error_message = "k3s_workers_count_green must be >= 0."
  }
}

variable "k3s_api_port" {
  description = "Kubernetes API port for k3s."
  type        = number
  default     = 6443
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
  description = "Hetzner region for resources (e.g., hel1, nbg1, fsn1)."
  type        = string
  default     = "hel1"
}