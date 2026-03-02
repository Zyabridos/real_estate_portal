variable "hcloud_token" {
  description = "Hetzner Cloud API token used by Terraform to manage resources."
  type        = string
  sensitive   = true
}

variable "base_name" {
  description = "Base project name used in resource names and labels."
  type        = string
  default     = "real-estate-hub"
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

variable "ssh_allowed_cidrs" {
  description = "List of CIDR blocks allowed to SSH into servers (tcp/22). Use this to restrict admin access to known IPs."
  type        = list(string)

  validation {
    condition     = alltrue([for c in var.ssh_allowed_cidrs : can(cidrhost(c, 0))])
    error_message = "ssh_allowed_cidrs must contain valid CIDR blocks (e.g., 1.2.3.4/32)."
  }
}