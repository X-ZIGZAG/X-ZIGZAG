FROM node:20.16.0
WORKDIR /app
COPY . .
RUN npm install
ARG API_URL
RUN sed -i "s|%%API_URL%%|${API_URL}|g" src/environments/environment.ts
RUN npm run build
EXPOSE 4000
CMD ["node", "dist/x-zigzag-web-page/server/server.mjs"]