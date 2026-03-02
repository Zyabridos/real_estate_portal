FROM node:20-alpine AS dev
WORKDIR /app

RUN apk add --no-cache curl

COPY package*.json ./
RUN npm ci

COPY . .

# Not-root user
RUN chown -R node:node /app
RUN apk add --no-cache curl
USER node

EXPOSE 3000

HEALTHCHECK --interval=10s --timeout=5s --start-period=20s --retries=8 \
  CMD curl -fsS http://localhost:3000/ > /dev/null || exit 1

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "3000"]
