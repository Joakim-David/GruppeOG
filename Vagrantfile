# -*- mode: ruby -*-
# vi: set ft=ruby :

Vagrant.configure("2") do |config|
  config.vm.box = 'digital_ocean'
  config.vm.box_url = "https://github.com/devopsgroup-io/vagrant-digitalocean/raw/master/box/digital_ocean.box"
  config.ssh.private_key_path = '~/.ssh/id_ed25519'
  config.vm.synced_folder ".", "/vagrant", type: "rsync"

  config.vm.define "chirpserver", primary: true do |server|

    server.vm.provider :digital_ocean do |provider|
      provider.ssh_key_name = ENV["SSH_KEY_NAME"]
      provider.token = ENV["DIGITAL_OCEAN_TOKEN"]
      provider.image = 'ubuntu-22-04-x64'
      provider.region = 'fra1'
      provider.size = 's-1vcpu-1gb'
    end

    server.vm.hostname = "chirpserver"

    server.vm.provision "shell", inline: <<-SHELL

      echo "Waiting for cloud-init to finish..."
      sudo cloud-init status --wait

      echo "================================================================="
      echo "=                Installing Docker                              ="
      echo "================================================================="

      sudo apt-get update -y
      sudo apt-get install -y ca-certificates curl gnupg

      # Add Docker GPG key (correct way for 22.04)
      sudo install -m 0755 -d /etc/apt/keyrings
      curl -fsSL https://download.docker.com/linux/ubuntu/gpg | \
        sudo gpg --dearmor -o /etc/apt/keyrings/docker.gpg
      sudo chmod a+r /etc/apt/keyrings/docker.gpg

      # Add Docker repository
      echo \
        "deb [arch=$(dpkg --print-architecture) signed-by=/etc/apt/keyrings/docker.gpg] \
        https://download.docker.com/linux/ubuntu \
        $(. /etc/os-release && echo $VERSION_CODENAME) stable" | \
        sudo tee /etc/apt/sources.list.d/docker.list > /dev/null

      sudo apt-get update -y

      sudo apt-get install -y docker-ce docker-ce-cli containerd.io

      sudo systemctl enable docker
      sudo systemctl start docker

      echo "================================================================="
      echo "=                Building Docker Image                          ="
      echo "================================================================="

      cd /vagrant
      sudo docker build -t chirp-app .

      echo "================================================================="
      echo "=                Running Docker Container                       ="
      echo "================================================================="

      sudo docker run -d \
        --name chirp \
        --restart unless-stopped \
        -p 7273:7273 \
        -v /home/vagrant/chirp-data:/app/data \
        bennyboomblaster/minitwitimage:latest

      echo "================================================================="
      echo "=                            DONE                               ="
      echo "================================================================="

      THIS_IP=$(hostname -I | awk '{print $1}')
      echo "Navigate in your browser to:"
      echo "http://${THIS_IP}:7273"
      echo "================================================================="

      sudo docker ps

    SHELL
  end
end