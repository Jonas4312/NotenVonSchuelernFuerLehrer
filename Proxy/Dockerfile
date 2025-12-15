# Stage 1: Zertifikatsgenerierung
FROM alpine:3.20 AS certgen

RUN apk add --no-cache openssl

# Build-Argument für Zertifikatspasswort
ARG CERT_PASSWORD=changeit

# Selbstsigniertes Zertifikat erstellen mit korrekten Berechtigungen
# UID 1654 = app user im YARP chiseled image
RUN openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
    -keyout /tmp/key.pem \
    -out /tmp/cert.pem \
    -subj "/CN=localhost/O=NotenVonSchuelernFuerLehrer/C=DE" \
    -addext "subjectAltName=DNS:localhost,DNS:yarp,IP:127.0.0.1" && \
    openssl pkcs12 -export -out /tmp/cert.pfx \
    -inkey /tmp/key.pem \
    -in /tmp/cert.pem \
    -password "pass:${CERT_PASSWORD}" && \
    chmod 644 /tmp/cert.pfx && \
    chown 1654:1654 /tmp/cert.pfx && \
    rm -f /tmp/key.pem /tmp/cert.pem

# Stage 2: Finales Image mit YARP
FROM mcr.microsoft.com/dotnet/nightly/yarp:2.3-preview

WORKDIR /app

# Zertifikat kopieren (Berechtigungen werden aus certgen übernommen)
COPY --from=certgen /tmp/cert.pfx /app/cert.pfx

# Konfiguration kopieren
COPY yarp.json /etc/yarp.config

# Umgebungsvariablen für Kestrel HTTPS
ENV ASPNETCORE_URLS="https://+:5001;http://+:5000"
ENV Kestrel__Certificates__Default__Path="/app/cert.pfx"
ENV Kestrel__Certificates__Default__Password="changeit"

# Port 5001: HTTPS (default exponiert)
# Port 5000: HTTP (intern für weitere Reverse Proxies)
EXPOSE 5000
EXPOSE 5001
