output "server_ip" {
  value = digitalocean_droplet.swarm.ipv4_address
}

output "reserved_ip" {
  value = digitalocean_reserved_ip.chirp.ip_address
}

output "website" {
  value = "http://${digitalocean_reserved_ip.chirp.ip_address}:7273"
}

output "grafana" {
  value = "http://${digitalocean_reserved_ip.chirp.ip_address}:3000"
}

output "prometheus" {
  value = "http://${digitalocean_reserved_ip.chirp.ip_address}:9090"
}

output "database_host" {
  value = digitalocean_database_cluster.postgres.private_host
}

output "database_port" {
  value = digitalocean_database_cluster.postgres.port
}

output "ssh_command" {
  value = "ssh root@${digitalocean_droplet.swarm.ipv4_address}"
}
