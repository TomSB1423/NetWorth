# Build stage
FROM node:20-alpine AS build
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
FROM nginx:1.27-alpine AS runtime
COPY --from=build /app/build /usr/share/nginx/html
# Provide a default nginx config
COPY nginx.conf /etc/nginx/conf.d/default.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
