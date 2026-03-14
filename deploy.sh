#!/bin/bash
set -e  # Exit on any error

echo "Pulling latest image..."
docker pull bennyboomblaster/minitwitimage:latest

echo "Stopping old container..."
docker stop chirp || true

echo "Removing old container..."
docker rm chirp || true

echo "Starting new container..."
docker run -d \
    --name chirp \
    --restart unless-stopped \
    -p 7273:7273 \
    -e "ConnectionStrings__DefaultConnection=Host=db-postgresql-fra1-72367-do-user-33600044-0.e.db.ondigitalocean.com;Port=25060;Database=chirp;Username=doadmin;Password=${POSTGRES_PSW}" \
    -v /data/latest.txt:/app/data/latest.txt \
    bennyboomblaster/minitwitimage:latest

echo "Deployment complete!"
docker ps | grep chirp
