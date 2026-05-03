The Chirp! URL: https://chirpitu.live

## Deployment

There are two ways to deploy Chirp with Docker Swarm:

1. **Existing database** — deploys a swarm droplet that connects to the existing managed PostgreSQL database.
2. **New database** — deploys a swarm droplet and creates a new managed PostgreSQL database.

Both methods require Terraform and deploy the full monitoring stack (Grafana, Prometheus, Loki, Alloy).

### Prerequisites

- [Terraform](https://developer.hashicorp.com/terraform/downloads) (>= 1.3.0)
- `jq` installed locally (`sudo apt install jq`) (only needed for Method 1)
- Access to the team's DigitalOcean account

### Step 1: Generate SSH keys

```bash
mkdir -p ssh_key
ssh-keygen -t ed25519 -f ssh_key/terraform -N ""
```

This creates `ssh_key/terraform` (private) and `ssh_key/terraform.pub` (public). Do not commit these to git.

### Step 2: Create a secrets file

Create a file called `secrets` in the project root. Do not commit it to git.

**For Method 1 (existing database):**

```bash
export TF_VAR_do_token=<your-digitalocean-api-token>
export TF_VAR_postgres_psw=<the-database-password>
```

**For Method 2 (new database):**

```bash
export TF_VAR_do_token=<your-digitalocean-api-token>
export TF_VAR_ssh_public_key=../ssh_key/terraform.pub
export TF_VAR_ssh_private_key=../ssh_key/terraform
```

| Variable | Where to find it |
|---|---|
| `TF_VAR_do_token` | DigitalOcean console > API > Tokens > Generate New Token (read + write) |
| `TF_VAR_postgres_psw` | Ask a team member or find it in the DigitalOcean database connection details |

---

## Method 1: Deploy to existing database

This uses `chirp_swarm_cluster.tf` in the project root. The droplet connects to the existing managed PostgreSQL cluster.

### Step 3: Create a Reserved IP

1. Go to DigitalOcean console > **Networking** > **Reserved IPs**
2. Click **Assign Reserved IP** and select region `fra1`
3. Copy the IP address

### Step 4: Deploy

```bash
source secrets
terraform init
terraform apply -var="reserved_ip=<your-reserved-ip>"
```

Terraform will:
1. Upload your SSH key to DigitalOcean
2. Create a droplet running Docker
3. Copy all config files (stack, Prometheus, Grafana, Loki, Alloy)
4. Initialize a Docker Swarm cluster
5. Deploy all services via `docker stack deploy`
6. Whitelist the droplet on the managed database firewall
7. Assign the reserved IP to the droplet

### Tearing down

```bash
terraform destroy
```

This destroys the droplet but keeps the reserved IP (it is managed outside Terraform).

---

## Method 2: Deploy with a new database

This uses the `terraform/` subdirectory. It creates both a new managed PostgreSQL cluster and a swarm droplet. The database connection string is automatically injected into the stack.

### Step 3: Deploy

```bash
source secrets
cd terraform
terraform init
terraform apply
```

Terraform will:
1. Upload your SSH key to DigitalOcean
2. Create a new managed PostgreSQL cluster and database
3. Create a droplet running Docker
4. Copy all config files with the database connection string injected
5. Initialize a Docker Swarm cluster
6. Deploy all services via `docker stack deploy`
7. Configure the database firewall to allow the droplet
8. Create and assign a reserved IP to the droplet

### Tearing down

```bash
cd terraform
terraform destroy
```

This destroys the droplet, the database, and the reserved IP.

---

## Accessing the server

```bash
ssh -i ssh_key/terraform root@<server-ip>
```

## Useful commands on the server

```bash
docker service ls                      # list all services and their status
docker service logs chirp_chirpserver  # view app logs
docker service ps chirp_chirpserver    # see which nodes run the app
```
