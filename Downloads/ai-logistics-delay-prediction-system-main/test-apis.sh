#!/bin/bash

# API Test Script for Logistics Backend
# Tests all major endpoints

BASE_URL="http://localhost:5075/api"
TIMESTAMP=$(date +"%Y-%m-%d %H:%M:%S")

echo "=========================================="
echo "🧪 API Test Report"
echo "Timestamp: $TIMESTAMP"
echo "Base URL: $BASE_URL"
echo "=========================================="
echo ""

# Color codes
GREEN='\033[0;32m'
RED='\033[0;31m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Test counters
TOTAL_TESTS=0
PASSED_TESTS=0
FAILED_TESTS=0

# Function to test endpoint
test_endpoint() {
    local method=$1
    local endpoint=$2
    local data=$3
    local description=$4
    
    TOTAL_TESTS=$((TOTAL_TESTS + 1))
    
    echo "Test #$TOTAL_TESTS: $description"
    echo "Method: $method | Endpoint: $endpoint"
    
    if [ "$method" == "POST" ] || [ "$method" == "PUT" ]; then
        http_code=$(curl -s -w "%{http_code}" -o /tmp/response.json -X "$method" \
            -H "Content-Type: application/json" \
            -d "$data" \
            "$BASE_URL$endpoint")
    else
        http_code=$(curl -s -w "%{http_code}" -o /tmp/response.json -X "$method" \
            "$BASE_URL$endpoint")
    fi
    
    echo "Response Code: $http_code"
    
    if [[ "$http_code" =~ ^[23] ]]; then
        echo -e "${GREEN}✅ PASS${NC}"
        PASSED_TESTS=$((PASSED_TESTS + 1))
    else
        echo -e "${RED}❌ FAIL${NC}"
        FAILED_TESTS=$((FAILED_TESTS + 1))
    fi
    
    if [ -f /tmp/response.json ] && [ -s /tmp/response.json ]; then
        echo "Response: $(cat /tmp/response.json | head -c 200)..."
        rm -f /tmp/response.json
    fi
    echo ""
}

# Shipment Tests
echo "=========================================="
echo "📦 Shipment Endpoint Tests"
echo "=========================================="

test_endpoint "GET" "/shipment" "" "Get all shipments"

test_endpoint "POST" "/shipment" \
    '{"origin":"New York","destination":"Los Angeles","carrier":"FedEx","trackingNumber":"FDX123456789","estimatedDeliveryDateUtc":"2026-04-25T15:30:00Z","status":"Pending"}' \
    "Create new shipment"

test_endpoint "GET" "/shipment/1" "" "Get shipment by ID (1)"

test_endpoint "GET" "/shipment/search?query=FDX" "" "Search shipments"

test_endpoint "GET" "/shipment/filter/status?status=Pending" "" "Filter by status"

# Tracking Tests
echo "=========================================="
echo "📍 Tracking Endpoint Tests"
echo "=========================================="

test_endpoint "GET" "/tracking/shipment/1" "" "Get tracking history for shipment 1"

test_endpoint "POST" "/tracking" \
    '{"shipmentId":1,"location":"Memphis TN","timestamp":"2026-04-23T12:00:00Z","status":"InTransit"}' \
    "Create tracking log"

# Alert Tests
echo "=========================================="
echo "🚨 Alert Endpoint Tests"
echo "=========================================="

test_endpoint "GET" "/alert" "" "Get all alerts"

test_endpoint "POST" "/alert" \
    '{"shipmentId":1,"type":"DelayWarning","message":"Potential delay detected","severity":"High"}' \
    "Create alert"

# Dashboard Tests
echo "=========================================="
echo "📊 Dashboard Endpoint Tests"
echo "=========================================="

test_endpoint "GET" "/dashboard/statistics" "" "Get dashboard statistics"

test_endpoint "GET" "/dashboard/at-risk-shipments" "" "Get at-risk shipments"

# Error Handling Tests
echo "=========================================="
echo "⚠️ Error Handling Tests"
echo "=========================================="

test_endpoint "GET" "/shipment/99999" "" "Get non-existent shipment (404)"

test_endpoint "POST" "/shipment" \
    '{"origin":"","destination":"","carrier":"","trackingNumber":"","estimatedDeliveryDateUtc":"","status":""}' \
    "Invalid shipment data"

# Summary
echo "=========================================="
echo "📋 Test Summary"
echo "=========================================="
echo "Total Tests: $TOTAL_TESTS"
echo -e "${GREEN}Passed: $PASSED_TESTS${NC}"
echo -e "${RED}Failed: $FAILED_TESTS${NC}"

PASS_RATE=$((PASSED_TESTS * 100 / TOTAL_TESTS))
echo "Pass Rate: $PASS_RATE%"

if [ $FAILED_TESTS -eq 0 ]; then
    echo -e "${GREEN}✅ All tests passed!${NC}"
    exit 0
else
    echo -e "${YELLOW}⚠️ Some tests failed${NC}"
    exit 1
fi
