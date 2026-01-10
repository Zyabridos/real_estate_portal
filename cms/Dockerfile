FROM node:20-alpine AS dev
WORKDIR /app

COPY package*.json ./
RUN npm ci || npm install

COPY . .

EXPOSE 3333

CMD ["npm", "run", "dev", "--", "--host", "0.0.0.0", "--port", "3333"]
