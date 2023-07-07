# Deployment guide for standalone linux server

This guide is for the deployment of this codebase on client's linux environments using docker with their own SSL certificate.

## Prerequisites

1. Docker with docker compose installed on the machine with version 20
2. Access to ECR to pull the image of this codebase
3. Permission on the server with sudo access

## Deployment Steps

1. Get the certificates from client and put inside the `/nginx/ssl.cert` and `./nginx/ssl.key` name should be matched else you have to update the `./nginx/Dockerfile` and `./nginx/nginx.conf` to match the .cert and .key file name.
2. Update the docker-compose.yml file with appropriate environments variables. Following are required configuration

| Environment Variable                    | Values                                                  |
| --------------------------------------- | ------------------------------------------------------- |
| ConnectionStrings\_\_DefaultConnection  | MySQL connection string for EF Core for main APIs       |
| JWT\_\_DurationInMinutes                | Expiration duration of JWT token in minutes             |
| ConnectionStrings\_\_Hangfireconnection | MySQL connection string for EF Core for Hangfire service |
| Hangfire\_\_User                        | Username for the hangfire dashboard                     |
| Hangfire\_\_Password                    | Password for the hangfire dashboard                     |
| AppUrls\_\_App                          | Domain name for the application                         |

1. Run following commands

   ```bash'
   docker compose build
   docker compose up -d
   ```

2. Go the your domain or IP address on browser or localhost to verify the site is up and running well.
