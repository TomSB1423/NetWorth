# Build stage
FROM node:24-alpine@sha256:2867d550cf9d8bb50059a0fff528741f11a84d985c732e60e19e8e75c7239c43 AS build
WORKDIR /app

# Install deps first
COPY package.json package-lock.json* yarn.lock* pnpm-lock.yaml* ./
# Install all deps for build (react-scripts needs them)
RUN npm ci || npm install

# Copy source and build
COPY . .
# Allow API base URL injection at build time (CRA reads REACT_APP_* envs)
ARG REACT_APP_API_BASE_URL
ENV REACT_APP_API_BASE_URL=${REACT_APP_API_BASE_URL}
RUN npm run build

# Runtime stage (nginx)
FROM nginx:1.29-alpine@sha256:b3c656d55d7ad751196f21b7fd2e8d4da9cb430e32f646adcf92441b72f82b14 AS runtime
COPY --from=build /app/build /usr/share/nginx/html
# Provide a default nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
