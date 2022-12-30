@echo off
setlocal

set startup_project=.\src\Sestio.Usuarios.Startup
set migrations_project=.\src\Sestio.Usuarios.Startup.Migrations

set command=%1
set tail=%*
call set tail=%%tail:*%1=%%

COPY %startup_project%\appsettings*.json %migrations_project%
dotnet ef %command% -p %migrations_project% %tail%
