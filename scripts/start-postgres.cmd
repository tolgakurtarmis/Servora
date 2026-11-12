@echo off
setlocal
cd /d "%~dp0.."

docker compose up -d
if errorlevel 1 exit /b 1

echo Waiting for Postgres...
set /a attempts=0

:wait_loop
set /a attempts+=1
docker compose exec -T postgres pg_isready -U postgres >nul 2>&1
if not errorlevel 1 (
  echo Postgres is ready.
  exit /b 0
)

if %attempts% geq 30 (
  echo Postgres did not become ready in time.
  exit /b 1
)

timeout /t 1 /nobreak >nul
goto wait_loop
