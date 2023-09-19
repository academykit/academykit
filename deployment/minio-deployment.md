# Deployment guide for Minio server in linux server

MinIO is an object storage solution that provides an Amazon Web Services S3-compatible API and supports all core S3 features. MinIO is built to deploy anywhere - public or private cloud, bare metal infrastructure, orchestrated environments, and edge infrastructure.

| Environment Variable | Values              |
| -------------------- | ------------------- |
| MINIO_ROOT_USER      | Minio root user     |
| MINIO_ROOT_PASSWORD  | Minio root password |

1.  Run following commands

```bash'
docker compose -f docker-compose-minio.yaml build
docker compose -f docker-compose-minio.yaml up -d
```

2. Go the your domain or IP address on browser or localhost to verify the site is up and running well.
