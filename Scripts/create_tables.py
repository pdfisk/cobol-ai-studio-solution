#!/usr/bin/env python3
"""
Create tables for COBOL and Python source files in PostgreSQL database 'cobol_studio'.
Usage: python create_tables.py
"""

import psycopg2
from psycopg2.extensions import ISOLATION_LEVEL_AUTOCOMMIT

DB_NAME = "cobol_studio"
DB_USER = "admin"
DB_PASSWORD = "chichi"
DB_HOST = "localhost"
DB_PORT = 5432


def create_database_if_not_exists(conn_params: dict) -> None:
    """Create the database if it does not exist (connects to 'postgres' first)."""
    conn_params_create = conn_params.copy()
    conn_params_create["dbname"] = "postgres"
    conn = psycopg2.connect(**conn_params_create)
    conn.set_isolation_level(ISOLATION_LEVEL_AUTOCOMMIT)
    cur = conn.cursor()
    cur.execute(
        "SELECT 1 FROM pg_database WHERE datname = %s",
        (DB_NAME,),
    )
    if not cur.fetchone():
        cur.execute(f'CREATE DATABASE "{DB_NAME}"')
        print(f"Created database '{DB_NAME}'.")
    else:
        print(f"Database '{DB_NAME}' already exists.")
    cur.close()
    conn.close()


def create_tables(conn) -> None:
    """Create cobol_source_files and python_source_files tables."""
    cur = conn.cursor()

    cur.execute("""
        CREATE TABLE IF NOT EXISTS cobol_source_files (
            id SERIAL PRIMARY KEY,
            file_name VARCHAR(512) NOT NULL,
            content TEXT NOT NULL,
            created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
            updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
            UNIQUE(file_name)
        );
    """)
    print("Table 'cobol_source_files' is ready.")

    cur.execute("""
        CREATE TABLE IF NOT EXISTS python_source_files (
            id SERIAL PRIMARY KEY,
            file_name VARCHAR(512) NOT NULL,
            content TEXT NOT NULL,
            created_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
            updated_at TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
            UNIQUE(file_name)
        );
    """)
    print("Table 'python_source_files' is ready.")

    conn.commit()
    cur.close()


def main() -> None:
    conn_params = {
        "dbname": DB_NAME,
        "user": DB_USER,
        "password": DB_PASSWORD,
        "host": DB_HOST,
        "port": DB_PORT,
    }
    create_database_if_not_exists(conn_params)
    conn = psycopg2.connect(**conn_params)
    try:
        create_tables(conn)
    finally:
        conn.close()
    print("Done.")


if __name__ == "__main__":
    main()
