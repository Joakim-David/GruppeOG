terraform {
  required_version = ">= 1.3.0"

  required_providers {
    digitalocean = {
      source  = "digitalocean/digitalocean"
      version = "~> 2.0"
    }
  }
}

provider "digitalocean" {
  token = var.do_token
}

#####################################
# VARIABLES
#####################################

variable "do_token" {
  sensitive = true
}

variable "region" {
  default = "fra1"
}

variable "ssh_public_key" {
  default = "ssh_key/terraform.pub"
}

variable "ssh_private_key" {
  default = "ssh_key/terraform"
}

variable "postgres_psw" {}

variable "db_cluster_id" {
  description = "DigitalOcean managed database cluster ID"
  default     = "f9289e58-03ef-4e82-8523-c3ca3dfd873d"
}

variable "reserved_ip" {
  description = "Pre-existing DigitalOcean reserved IP address"
  default     = "209.38.190.12"
}

#####################################
# SSH KEY
#####################################

resource "digitalocean_ssh_key" "default" {
  name       = "terraform-key"
  public_key = file(var.ssh_public_key)
}

#####################################
# SINGLE NODE SWARM MANAGER
#####################################

resource "digitalocean_droplet" "swarm" {
  name   = "chirp-swarm"
  region = var.region
  size   = "s-1vcpu-1gb"
  image  = "docker-20-04"

  ssh_keys = [
    digitalocean_ssh_key.default.fingerprint
  ]

  connection {
    type        = "ssh"
    user        = "root"
    host        = self.ipv4_address
    private_key = file(var.ssh_private_key)
    timeout     = "3m"
  }

  #####################################
  # COPY STACK FILES
  #####################################

  provisioner "file" {
    source      = "chirp_stack.yml"
    destination = "/root/chirp_stack.yml"
  }

#provisioner "file" {
#  source      = "stack/nginx/default.conf"
#  destination = "/root/nginx/default.conf"
#}

  provisioner "file" {
    source      = "monitoring/prometheus.yml"
    destination = "/root/prometheus.yml"
  }

  provisioner "file" {
    source      = "alloy/config.alloy"
    destination = "/root/config.alloy"
  }

  provisioner "file" {
    source      = "loki/loki-config.yml"
    destination = "/root/loki-config.yml"
  }

  provisioner "file" {
    source      = "grafana/datasource.yml"
    destination = "/root/datasource.yml"
  }

  provisioner "file" {
    source      = "grafana/dashboards"
    destination = "/root/dashboards"
  }

  provisioner "file" {
    source      = "latest.txt"
    destination = "/root/latest.txt"
  }

    #####################################
    # SETUP + DEPLOY
    #####################################

    provisioner "remote-exec" {
    inline = [
        # firewall
        "ufw allow 22",
        "ufw allow 80",
        "ufw allow 3000",
        "ufw allow 3100",
        "ufw allow 4000",
        "ufw allow 7273",
        "ufw allow 9090",
        "ufw allow 9095",
        "ufw allow 2377/tcp",
        "ufw allow 7946",
        "ufw allow 4789/udp",
        "ufw default allow routed",
        "ufw --force enable",

        # prepare folders
        "mkdir -p /root/monitoring",
        "mkdir -p /root/alloy",
        "mkdir -p /root/loki",
        "mkdir -p /root/grafana/dashboards",

        # move files into expected locations
        "mv /root/prometheus.yml /root/monitoring/prometheus.yml",
        "mv /root/config.alloy /root/alloy/config.alloy",
        "mv /root/loki-config.yml /root/loki/loki-config.yml",
        "mv /root/datasource.yml /root/grafana/datasource.yml",
        "mv /root/dashboards/* /root/grafana/dashboards/",

        # initialize swarm
        "docker swarm init --advertise-addr ${self.ipv4_address}",

        # deploy stack with postgres password
        "POSTGRES_PSW='${var.postgres_psw}' docker stack deploy -c /root/chirp_stack.yml chirp"
    ]
    }
}

#####################################
# DATABASE FIREWALL (append-only)
#####################################

resource "null_resource" "db_whitelist" {
  triggers = {
    droplet_id = digitalocean_droplet.swarm.id
  }

  provisioner "local-exec" {
    command = <<-EOT
      CURRENT=$(curl -s -X GET \
        -H "Authorization: Bearer ${var.do_token}" \
        "https://api.digitalocean.com/v2/databases/${var.db_cluster_id}/firewall" \
        | jq '.rules')

      NEW_RULES=$(echo "$CURRENT" | jq '. + [{"type": "droplet", "value": "${digitalocean_droplet.swarm.id}"}]')

      curl -s -X PUT \
        -H "Authorization: Bearer ${var.do_token}" \
        -H "Content-Type: application/json" \
        -d "{\"rules\": $NEW_RULES}" \
        "https://api.digitalocean.com/v2/databases/${var.db_cluster_id}/firewall"
    EOT
  }
}

#####################################
# RESERVED (FLOATING) IP
#####################################

resource "digitalocean_reserved_ip_assignment" "chirp" {
  ip_address = var.reserved_ip
  droplet_id = digitalocean_droplet.swarm.id
}

#####################################
# OUTPUTS
#####################################

output "server_ip" {
  value = digitalocean_droplet.swarm.ipv4_address
}

output "reserved_ip" {
  value = var.reserved_ip
}

output "website" {
  value = "http://${var.reserved_ip}"
}

output "grafana" {
  value = "http://${var.reserved_ip}:3000"
}

output "prometheus" {
  value = "http://${var.reserved_ip}:9090"
}