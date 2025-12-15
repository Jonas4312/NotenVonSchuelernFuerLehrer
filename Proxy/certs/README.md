# SSL-Zertifikate für YARP Reverse Proxy

## Automatische Zertifikatsgenerierung

Das YARP Docker Image generiert beim Build automatisch ein selbstsigniertes Zertifikat.
Das Passwort kann über das Build-Argument `CERT_PASSWORD` gesetzt werden (Standard: `changeit`).

## Eigenes Zertifikat verwenden

Für **Produktion** ein echtes Zertifikat hier ablegen:

1. Erstelle eine PFX-Datei (PKCS#12) mit deinem Zertifikat + Private Key
2. Benenne die Datei `cert.pfx`
3. Lege sie in dieses Verzeichnis
4. Bearbeite `docker-compose.yaml` und kommentiere die Volume-Zeilen ein:
   ```yaml
   volumes:
     - ./Proxy/certs/cert.pfx:/app/certs/cert.pfx:ro
   ```
5. Setze das Zertifikatspasswort in einer `.env` Datei:
   ```
   CERT_PASSWORD=dein-passwort
   ```
6. Führe `docker compose up --build` aus

### PFX aus PEM-Dateien erstellen

Falls du separate `.crt` und `.key` Dateien hast:

```bash
openssl pkcs12 -export -out cert.pfx -inkey privkey.pem -in fullchain.pem -password pass:dein-passwort
```

### Let's Encrypt Zertifikate

Mit certbot generierte Zertifikate konvertieren:

```bash
openssl pkcs12 -export \
  -out cert.pfx \
  -inkey /etc/letsencrypt/live/deine-domain.de/privkey.pem \
  -in /etc/letsencrypt/live/deine-domain.de/fullchain.pem \
  -password pass:dein-passwort
```

## Ports

- **5001** (HTTPS): Öffentlich exponiert
- **5000** (HTTP): Verfügbar für interne Reverse Proxies (in docker-compose.yaml auskommentiert)
