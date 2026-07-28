#!/bin/bash
# PostgreSQL container başlarken ContactDb ve ReportDb veritabanlarını oluşturur.
# Migration'lar uygulama başlarken EF Core tarafından çalıştırılır.

set -e

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" <<-EOSQL
    SELECT 'CREATE DATABASE "ContactDb"'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'ContactDb')\gexec

    SELECT 'CREATE DATABASE "ReportDb"'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'ReportDb')\gexec
EOSQL

echo "✅ ContactDb ve ReportDb veritabanları hazır."
