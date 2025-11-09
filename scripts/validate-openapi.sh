#!/bin/bash
set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${YELLOW}Starting Functions app to generate OpenAPI spec...${NC}"

# Start the Functions app in the background
cd "$(dirname "$0")/../Networth.Functions"
dotnet run &
FUNC_PID=$!

# Wait for Functions app to start
echo "Waiting for Functions app to start..."
sleep 10

# Download the OpenAPI spec
echo -e "${YELLOW}Downloading OpenAPI spec...${NC}"
SPEC_FILE="../openapi.json"

# Try different common OpenAPI endpoints
ENDPOINTS=(
    "http://localhost:7071/api/openapi/v3.json"
    "http://localhost:7071/api/openapi/v2.json"
    "http://localhost:7071/api/swagger.json"
    "http://localhost:7071/swagger/v1/swagger.json"
)

SUCCESS=false
for endpoint in "${ENDPOINTS[@]}"; do
    echo "Trying endpoint: $endpoint"
    if curl -s -f "$endpoint" -o "$SPEC_FILE"; then
        echo -e "${GREEN}Successfully downloaded from $endpoint${NC}"
        SUCCESS=true
        break
    fi
done

if [ "$SUCCESS" = false ]; then
    echo -e "${RED}Failed to download OpenAPI spec from any endpoint${NC}"
    kill $FUNC_PID 2>/dev/null || true
    exit 1
fi

# Stop the Functions app
echo -e "${YELLOW}Stopping Functions app...${NC}"
kill $FUNC_PID 2>/dev/null || true
wait $FUNC_PID 2>/dev/null || true

echo -e "${GREEN}Note: Scalar API documentation is available at http://localhost:7071/api/docs when the app is running${NC}"

# Validate with Spectral
echo -e "${YELLOW}Validating OpenAPI spec with Spectral...${NC}"
cd "$(dirname "$0")/.."

if ! command -v spectral &> /dev/null; then
    echo -e "${YELLOW}Spectral not found. Installing...${NC}"
    npm install -g @stoplight/spectral-cli
fi

spectral lint "$SPEC_FILE" || {
    echo -e "${RED}OpenAPI validation failed!${NC}"
    exit 1
}

echo -e "${GREEN}OpenAPI spec is valid!${NC}"
