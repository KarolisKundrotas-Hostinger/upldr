# Upldr - A simple server allowing to upload files

A simple server allowing to upload files. Designed to be used on Docker and Kubernetes.

## Usage

```bash
curl --data-binary '@filetoupload.txt' -X POST -H 'X-Key: changeme' -H 'X-File-Name: fileonserver.txt' -H 'Content-Type: application/octet-stream' http://upldr/upload -v
```

## Environment variables

| Variable | Description | Default value |
| -------- | ----------- | ------------- |
| `DOTNET_ENVIRONMENT` | Environment name | `Production` |
| `DOTNET_URLS` | Urls to listen to | `http://+:80` |
| `DOTNET_UploadPath` | Path where to store uploaded files | `/app/uploads` |
| `DOTNET_Key` | Key used to authenticate | `changeme` |
