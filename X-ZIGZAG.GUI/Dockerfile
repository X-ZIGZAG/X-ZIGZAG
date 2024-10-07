FROM node:20.16.0-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
ARG API_URL
RUN sed -i "s|%%API_URL%%|${API_URL}|g" src/environments/environment.ts
RUN npm run build

FROM node:20.16.0-alpine
WORKDIR /app
COPY --from=build /app/dist ./dist
COPY --from=build /app/package*.json ./
RUN npm ci --only=production
EXPOSE 4000
CMD ["node", "dist/x-zigzag-web-page/server/server.mjs"]
