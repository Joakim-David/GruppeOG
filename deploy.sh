source ~/.bash_profile

cd /minitwit

docker pull $DOCKER_USERNAME/minitwitimage:latest
docker restart $DOCKER_USERNAME/minitwitimage:latest