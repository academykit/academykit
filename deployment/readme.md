# Database Server

## Install mysql server

```bash'
    sudo apt update
    sudo apt install mysql-server
    sudo systemctl start mysql.service
```

## start mysql service

```bash'
    sudo systemctl enable mysql.service
    sudo systemctl start mysql.service
    # check if mysql is running ro not
    sudo systemctl status mysql.service
```

### Output should look like

```
● mysql.service - MySQL Community Server
     Loaded: loaded (/lib/systemd/system/mysql.service; enabled Vendor
     preset: enabled)
     Active: active (running) since Tue 2020-04-21 12:56:48 UTC; 6min ago
     Main PID: 10382 (mysqld)
     Status: "Server is operational"
      Tasks: 39 (limit: 1137)
     Memory: 370.0M
     CGroup: /system.slice/mysql.service
             └─10382 /usr/sbin/mysqld

```

## Configure mysql server

```bash'
sudo mysql

CREATE USER 'user'@'%' IDENTIFIED BY 'password';

## "%" should be replaced by ip address of main backend
CREATE DATABASE database_name;

GRANT CREATE, ALTER, DROP, INSERT, UPDATE, INDEX, DELETE, SELECT, REFERENCES, RELOAD on database_name.* TO 'user'@'%';
```

    >Note: % in mysql query enables database connection for all host. please add ip address of server for security purpose.

# Application Server

### Install docker

```bash
sudo apt-get update
curl https://desktop.docker.com/linux/main/amd64/docker-desktop-4.17.0-amd64.deb?utm_source=docker&utm_medium=webreferral&utm_campaign=docs-driven-download-linux-amd64
sudo apt-get install ./docker-desktop-<version>-<arch>.deb
systemctl --user start docker-desktop
docker compose version
docker version
```

Follow **[Application deployment guide](./application-deployment.md)** for application server

# minio server

Follow **[Minio deployment guide](./minio-deployment.md)** for minio server
