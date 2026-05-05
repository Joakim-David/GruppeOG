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
# LOCALS
#####################################

locals {
  connection_string = "Host=${digitalocean_database_cluster.postgres.private_host};Port=${digitalocean_database_cluster.postgres.port};Database=${digitalocean_database_db.chirp.name};Username=${digitalocean_database_cluster.postgres.user};Password=${digitalocean_database_cluster.postgres.password};SSL Mode=Require"
}

#####################################
# SSH KEY
#####################################

resource "digitalocean_ssh_key" "default" {
  name       = "chirp-full-deploy-key"
  public_key = file(var.ssh_public_key)
}

#####################################
# MANAGED POSTGRESQL DATABASE
#####################################

resource "digitalocean_database_cluster" "postgres" {
  name       = "chirp-postgres"
  engine     = "pg"
  version    = var.db_engine_version
  size       = var.db_size
  region     = var.region
  node_count = var.db_node_count
}

resource "digitalocean_database_db" "chirp" {
  cluster_id = digitalocean_database_cluster.postgres.id
  name       = "chirp"
}

resource "digitalocean_database_firewall" "postgres" {
  cluster_id = digitalocean_database_cluster.postgres.id

  rule {
    type  = "droplet"
    value = digitalocean_droplet.swarm.id
  }
}

#####################################
# SWARM DROPLET
#####################################

resource "digitalocean_droplet" "swarm" {
  name   = "chirp-swarm-full"
  region = var.region
  size   = var.droplet_size
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
  # COPY STACK + CONFIG FILES
  #####################################

  provisioner "file" {
    content     = templatefile("${path.module}/templates/chirp_stack.yml.tpl", {
      connection_string = local.connection_string
    })
    destination = "/root/chirp_stack.yml"
  }

  provisioner "file" {
    source      = "${path.module}/../monitoring/prometheus.yml"
    destination = "/root/prometheus.yml"
  }

  provisioner "file" {
    source      = "${path.module}/../alloy/config.alloy"
    destination = "/root/config.alloy"
  }

  provisioner "file" {
    source      = "${path.module}/../loki/loki-config.yml"
    destination = "/root/loki-config.yml"
  }

  provisioner "file" {
    source      = "${path.module}/../grafana/datasource.yml"
    destination = "/root/datasource.yml"
  }

  provisioner "file" {
    source      = "${path.module}/../grafana/dashboards"
    destination = "/root/dashboards"
  }

  provisioner "file" {
    source      = "${path.module}/../latest.txt"
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
      "chmod 666 /root/latest.txt",

      # initialize swarm
      "docker swarm init --advertise-addr ${self.ipv4_address}",

      # deploy stack
      "docker stack deploy -c /root/chirp_stack.yml chirp"
    ]
  }
}

#####################################
# RESERVED IP
#####################################

resource "digitalocean_reserved_ip" "chirp" {
  region = var.region
}

resource "digitalocean_reserved_ip_assignment" "chirp" {
  ip_address = digitalocean_reserved_ip.chirp.ip_address
  droplet_id = digitalocean_droplet.swarm.id
}
