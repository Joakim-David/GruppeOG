variable "do_token" {
  description = "DigitalOcean API token"
  sensitive   = true
}

variable "region" {
  description = "DigitalOcean region for all resources"
  default     = "fra1"
}

variable "ssh_public_key" {
  description = "Path to SSH public key for droplet access"
}

variable "ssh_private_key" {
  description = "Path to SSH private key for provisioning"
}

variable "db_engine_version" {
  description = "PostgreSQL major version"
  default     = "17"
}

variable "db_size" {
  description = "DigitalOcean managed database size slug"
  default     = "db-s-1vcpu-1gb"
}

variable "db_node_count" {
  description = "Number of database nodes"
  default     = 1
}

variable "droplet_size" {
  description = "DigitalOcean droplet size slug"
  default     = "s-1vcpu-1gb"
}
