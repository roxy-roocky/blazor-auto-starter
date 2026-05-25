#!/bin/bash

# Installer nvm
wget -qO- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.4/install.sh | bash

# Charger nvm
export NVM_DIR="$HOME/.nvm"
[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"

# Installer et utiliser Node LTS
nvm install --lts
nvm use --lts

# Ajouter au PATH de manière persistante
echo 'export NVM_DIR="$HOME/.nvm"' >> ~/.bashrc
echo '[ -s "$NVM_DIR/nvm.sh" ] && \. "$NVM_DIR/nvm.sh"' >> ~/.bashrc

dotnet tool install -g dotnet-ef

# Ajouter dotnet tools au PATH
echo 'export PATH="$PATH:/root/.dotnet/tools"' >> ~/.bashrc
