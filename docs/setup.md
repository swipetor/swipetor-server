
### Prerequisites

#### Components

- .NET 8
- PostgreSQL (main db)
- InfluxDB (metrics)
- Nignx (for local proxying to swipetor-ui and swipetor-server)
- ffmpeg, MP4Box, yt-dlp CLIs (for video processing)
- - Search their installation on your platform.
- SMTP server — try `MailHog` for dev env

#### Cloud Services

- [Firebase cloud app](https://firebase.google.com/) — for push notifications - free
- [CloudFlare R2](https://www.cloudflare.com/developer-platform/r2/) — file storage - good free tier & no egress fees
- [Google Recaptcha](https://www.google.com/recaptcha/admin) — free


### Configuration file
- Rename `appsettings.Development.sample.json` to `appsettings.Development.json`
- - Ensure the crendentials are valid for Components & Cloud Services
- Ensure `PersistKeysDir` path exists and writable by app's running user
- Ensure `AppConfig.Site` values resolve to your nginx proxy

### Nginx configuration

- Sample config for the local nginx server for `local.swipetor.com` is below.
- Generate your self-signed keys.
- Nginx should point to webpack (from swipetor-ui) except `/public` dir should point to this server.

```
server {
  listen 8443 ssl http2;
  listen [::]:8443 ssl http2;
  server_name local.swipetor.com lan.swipetor.com;

  access_log /var/log/nginx/swipetor-app.https.access.log;
  error_log /var/log/nginx/swipetor-app.https.error.log;

  ssl_certificate /Users/ata/Documents/certs/local.swipetor.com/site.crt;
  ssl_certificate_key /Users/ata/Documents/certs/local.swipetor.com/site.key;

  client_max_body_size 200M;

  location ~ ^/public/(?!(build)) {
     root /Users/ata/projects/swpapp/SwipetorApp/wwwroot;
     autoindex on;

     # First attempt to serve request as file, then
     # as directory, then fall back to displaying a 404.
     try_files $uri $uri/ =404;
  }

  location / {
    try_files $uri @webpack_dev_server;
  }

  # Pass requests for / to the nodejs server:
  location @webpack_dev_server {
    client_max_body_size 200M;
    proxy_read_timeout 600s;
    proxy_pass https://localhost:8010;
    proxy_http_version 1.1;
    proxy_set_header   Upgrade $http_upgrade;
    proxy_set_header   Connection keep-alive;
    proxy_set_header   Host $host;
    proxy_cache_bypass $http_upgrade;
    proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
    proxy_set_header   X-Forwarded-Proto $scheme;
    proxy_set_header X-Real-IP $remote_addr;
    proxy_set_header X-NginX-Proxy true;
    proxy_ssl_session_reuse off;
    proxy_redirect off;
  }
}
```

### Firebase Cloud Messaging

- Download `firebase-admin.json` and place into App_Data of the main project.
- - This json has keys: `type, project_id, private_key_id, private_key, client_email, client_id, auth_uri, token_uri, auth_provider_x509_cert_url, client_x509_cert_url, universe_domain`
- Also fill in the app's other corresponding credentials in the appsettings.Development.json.

### Database
- Ensure the project compiles as a .net project
- Update database with `dotnet ef database update`

### CloudFlare R2 Setup

R2 needs CloudFlare workers to serve requests.  
See `r2-worker` directory. Update `wrangler.toml` and run `make deploy`. You will need to do `npm install` first.
Also, your worker hostname should match in the `appsettings` config json.

### Run
- Ensure all packages are downloaded with `nuget restore`
- `SwipetorAppTest` test project should run fine, try running in the IDE.
- Run the project like a normal .net project.

See [swipetor-ui Setup](https://github.com/atas/swipetor-ui/blob/main/docs/setup.md) for other components.