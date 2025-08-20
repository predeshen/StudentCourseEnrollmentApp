@echo off
echo Starting Student Course Enrollment App...
echo.

echo Starting API...
start "API" cmd /k "cd StudentCourseEnrollmentApp.API && dotnet run"

echo Starting Blazor App...
start "Blazor" cmd /k "cd StudentCourseEnrollmentApp.UI && dotnet run"

echo.
echo Both applications are starting...
echo API will be available at: http://localhost:7001
echo Blazor App will be available at: http://localhost:7003
echo.
echo Press any key to close this window...
pause >nul
