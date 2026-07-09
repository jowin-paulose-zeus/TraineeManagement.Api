This project was made as part of my training to learn dotnet.
I followed the daily steps provided in the "Three_Day_DotNet_API_InMemory_Training_Plan.pdf"
3rd July and 6th July 2026 Task
# TraineeMangement.Api

## Technology Used
-ASP.Net Web Core
-C#
-.Net
-Swagger
-MySQL

## API List
-GET/api/Health

-POST/api/Auth/register
-POST/api/Auth/login

-GET/api/trainee/Trainee
-POST/api/trainee/Trainee
-PUT/api/trainee/Trainee
-GET/api/trainee/Trainee/id
-DELETE/api/trainee/Trainee/Id
-GET/api/trainee/Trainee/search

## Features Completed

Phase 1
Day 1 (3rd July)
    -Swagger UI for API testing
    -In-memory data storage using List<Trainee>
    -Health Check endpoint (GET)
    -Get all Trainees (GET)
    -Get Trainee by Id (Get/{id})
    -Create new trainee (POST)
Day 2 (6th July)
    -Update Trainee details (PUT)
    -Delete Trainee (Delete)
    -Request and response DTOs
    -Input Validation
    -Service Layer Implementation
Day 3 (7th July)
    -Added EntityFrameworkCore
    -Added Async functions
    -Added a search function

Phase 2
Day 1 (8th July)
    -Added data persistence using MySQL
    -Migrated data and Schema from local MySQL
    -Tested all APIs with SQL Implementation
Day 2 (9th July)
    -Added user log in
    -Password Hashing
    -Jwt token generation


## How to Run
-Clone Repository
-Create Database in local MySQL
-Update appsettings.json as 
    "connectionStrings": {
    "DefaultConnection": "server=localhost;port=3306;database=YourDatabaseName;user=YourUserName;password=YourPassword;"
  }
-Install necessary packages like
    -dotnet add package MySql.EntityFrameworkCore
    -dotnet add Microsoft.EntityFrameworkCore.Tools
-Dotnet Restore ( Restores Nuget packages )
-Migrate MySQL and update by using these command
    -dotnet ef migrations add InitialCreate
    -dotnet ef database update
-Dotnet Run

## Improvements Planned
-Secure APIs
-Make list APIs scalable
