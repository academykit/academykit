# Deployment guide for Certificate url

| Environment Variable | Values               |
| -------------------- | -------------------- |
| APP_URL              | Main Application url |

1.  Run following commands

```bash'
docker compose -f docker-compose-certificate.yaml build
docker compose -f docker-compose-certificate.yaml up -d
```

2. Go the your domain or IP address on browser or localhost to verify the site is up and running well.
