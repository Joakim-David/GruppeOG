The Chirp! URL: http://142.93.169.145:7273

## Deploying with Terraform and Docker Swarm

### Prerequisites

- [Terraform](https://developer.hashicorp.com/terraform/downloads) (>= 1.3.0)
- `jq` installed locally (`sudo apt install jq`)
- Access to the team's DigitalOcean account

### Step 1: Generate SSH keys

```bash
mkdir -p ssh_key
ssh-keygen -t ed25519 -f ssh_key/terraform -N ""
```

This creates `ssh_key/terraform` (private) and `ssh_key/terraform.pub` (public). Do not commit these to git.

### Step 2: Create a secrets file

Create a file called `secrets` in the project root:

```bash
export TF_VAR_do_token=<your-digitalocean-api-token>
export TF_VAR_postgres_psw=<the-database-password>
```

| Variable | Where to find it |
|---|---|
| `TF_VAR_do_token` | DigitalOcean console > API > Tokens > Generate New Token (read + write) |
| `TF_VAR_postgres_psw` | Ask a team member or find it in the DigitalOcean database connection details |

Do not commit the `secrets` file to git.

### Step 3: Create a Reserved IP

1. Go to DigitalOcean console > **Networking** > **Reserved IPs**
2. Click **Assign Reserved IP** and select region `fra1`
3. Copy the IP address
4. Pass it when running apply:
   ```bash
   terraform apply -var="reserved_ip=<your-reserved-ip>"
   ```

### Step 4: Deploy

```bash
source secrets
terraform init
terraform apply
```

Terraform will:
1. Upload your SSH key to DigitalOcean
2. Create a droplet running Docker
3. Copy all config files (stack, Prometheus, Grafana, Loki, Alloy)
4. Initialize a Docker Swarm cluster
5. Deploy all services via `docker stack deploy`
6. Whitelist the droplet on the managed database
7. Assign the reserved IP to the droplet

### Accessing the server

```bash
ssh -i ssh_key/terraform root@<your-reserved-ip>
```

### Useful commands on the server

```bash
docker service ls                      # list all services and their status
docker service logs chirp_chirpserver  # view app logs
docker service ps chirp_chirpserver    # see which nodes run the app
```

### Tearing down

```bash
terraform destroy
```

This destroys the droplet but keeps the reserved IP (it is managed outside Terraform).
