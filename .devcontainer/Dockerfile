FROM mcr.microsoft.com/devcontainers/dotnet:6.0

# Fix APT keyring issues + tools
RUN apt-get update \
 && apt-get install -y curl gnupg software-properties-common apt-transport-https \
 && curl -fsSL https://packages.microsoft.com/keys/microsoft.asc | gpg --dearmor > /usr/share/keyrings/microsoft.gpg

# SQL Server CLI Tools
RUN curl -sSL https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/prod.list \
    | tee /etc/apt/sources.list.d/mssql-release.list \
 && apt-get update \
 && ACCEPT_EULA=Y apt-get install -y mssql-tools unixodbc-dev

ENV PATH="$PATH:/opt/mssql-tools/bin"

# Angular CLI 16 (global)
RUN npm install -g @angular/cli@16

# Create shared Docker network if not exists
CMD bash -c "docker network inspect sumisan-net || docker network create sumisan-net; exec bash"
