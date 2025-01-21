#!/bin/bash
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -Q "CREATE DATABASE BackendManagement"
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -d BackendManagement -i /scripts/InitialSchema.sql
/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Passw0rd -d BackendManagement -i /scripts/SeedData.sql 