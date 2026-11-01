#!/usr/bin/env bash
set -euo pipefail

SQLCMD="$(command -v sqlcmd || find /opt -type f -name sqlcmd 2>/dev/null | head -n 1)"
if [[ -z "${SQLCMD}" ]]; then
    echo "sqlcmd was not found in the mssql-tools image." >&2
    exit 1
fi

echo "Waiting for SQL Server at ${MSSQL_HOST:-sqlserver}..."
for attempt in $(seq 1 60); do
    if "${SQLCMD}" -S "${MSSQL_HOST:-sqlserver},1433" -U sa -P "${MSSQL_SA_PASSWORD}" -C -b -Q "SELECT 1" >/dev/null 2>&1; then
        break
    fi
    if [[ "${attempt}" -eq 60 ]]; then
        echo "SQL Server did not become ready in time." >&2
        exit 1
    fi
    sleep 5
done

echo "Applying database schema..."
"${SQLCMD}" -S "${MSSQL_HOST:-sqlserver},1433" -U sa -P "${MSSQL_SA_PASSWORD}" -C -b -i /schema/schema.sql

if compgen -G "/scripts/seeds/*.sql" > /dev/null; then
    for seed_file in /scripts/seeds/*.sql; do
        echo "Applying seed file: ${seed_file}"
        "${SQLCMD}" -S "${MSSQL_HOST:-sqlserver},1433" -U sa -P "${MSSQL_SA_PASSWORD}" -C -b -i "${seed_file}"
    done
fi

echo "Database initialization completed."
