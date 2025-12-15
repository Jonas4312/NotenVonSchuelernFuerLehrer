#!/bin/bash

CERT_PATH="/etc/yarp/certs/cert.pfx"
CERT_PASSWORD="${CERT_PASSWORD:-changeit}"

# Prüfen ob externes Zertifikat vorhanden
if [ -f "$CERT_PATH" ]; then
    echo "Externes Zertifikat gefunden: $CERT_PATH"
else
    echo "Kein externes Zertifikat gefunden. Generiere selbstsigniertes Zertifikat..."
    
    # Selbstsigniertes Zertifikat erstellen
    openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
        -keyout /tmp/key.pem \
        -out /tmp/cert.pem \
        -subj "/CN=localhost/O=NotenVonSchuelernFuerLehrer/C=DE" \
        -addext "subjectAltName=DNS:localhost,DNS:yarp,IP:127.0.0.1"
    
    # In PFX konvertieren
    openssl pkcs12 -export -out "$CERT_PATH" \
        -inkey /tmp/key.pem \
        -in /tmp/cert.pem \
        -password "pass:$CERT_PASSWORD"
    
    # Temporäre Dateien entfernen
    rm -f /tmp/key.pem /tmp/cert.pem
    
    echo "Selbstsigniertes Zertifikat erstellt: $CERT_PATH"
fi

# YARP mit HTTPS und HTTP starten
# Port 5001: HTTPS (nach außen exponiert)
# Port 5000: HTTP (intern für weitere Reverse Proxies, default nicht exponiert)
exec dotnet /app/Yarp.ReverseProxy.dll \
    --urls "https://+:5001;http://+:5000" \
    --Kestrel:Certificates:Default:Path="$CERT_PATH" \
    --Kestrel:Certificates:Default:Password="$CERT_PASSWORD"
