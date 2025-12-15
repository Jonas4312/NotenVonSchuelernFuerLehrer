# Build Stage
FROM node:22-alpine AS build

WORKDIR /app

# Dependencies installieren
COPY package*.json ./
RUN npm ci

# Source kopieren und bauen
COPY . .
RUN npm run build

# Production Stage - nginx zum Ausliefern
FROM nginx:alpine AS production

# Nginx Konfiguration für SPA
COPY nginx.conf /etc/nginx/conf.d/default.conf

# Build-Artefakte kopieren
COPY --from=build /app/dist /usr/share/nginx/html

EXPOSE 80

CMD ["nginx", "-g", "daemon off;"]
