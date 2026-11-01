#!/usr/bin/env bash
set -euo pipefail

API_URL="${API_URL:-http://localhost:5005}"
LOGIN="${DEMO_LOGIN:-demo}"
PASSWORD="${DEMO_PASSWORD:-OrderDemo#2026!}"

login_status="$(curl --silent --show-error --max-time 15 \
    --output /tmp/order-auth-response.json --write-out '%{http_code}' \
    -H 'Content-Type: application/json' \
    -d "$(jq -nc --arg login "$LOGIN" --arg password "$PASSWORD" '{login:$login,password:$password}')" \
    "$API_URL/api/user/auth")"

if [[ "$login_status" != "200" ]]; then
    echo "Demo user authentication failed with HTTP $login_status:" >&2
    cat /tmp/order-auth-response.json >&2
    exit 1
fi

login_response="$(cat /tmp/order-auth-response.json)"
token="$(jq -r '.token // .Token // empty' <<< "$login_response")"
if [[ -z "$token" ]]; then
    echo "Authentication response did not contain a JWT token." >&2
    echo "$login_response" >&2
    exit 1
fi

order_payload='{
  "clientId": "11111111111111111111111111111111",
  "userId": "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
  "items": [
    {
      "productId": "22222222222222222222222222222222",
      "sellValue": 250.00,
      "quantity": 1
    }
  ]
}'

status="$(curl --silent --show-error --max-time 20 --output /tmp/order-response.json --write-out '%{http_code}' \
    -X POST "$API_URL/api/order" \
    -H 'Content-Type: application/json' \
    -H "Authorization: Bearer $token" \
    -d "$order_payload")"

if [[ "$status" != "201" ]]; then
    echo "Creating an order failed with HTTP $status:" >&2
    cat /tmp/order-response.json >&2
    exit 1
fi

echo "End-to-end API validation passed: demo login succeeded and an order was created."
