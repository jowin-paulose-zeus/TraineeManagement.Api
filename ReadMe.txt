This project was made as part of my training to learn dotnet.

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

-GET/api/Trainee
-POST/api/Trainee
-PUT/api/Trainee/id
-GET/api/Trainee/id
-DELETE/api/Trainee/Id

-GET/api/Mentor
-POST/api/Mentor
-PUT/api/Mentor/id
-GET/api/Mentor/id
-DELETE/api/Mentor/id

-GET/api/LearningTask
-POST/api/LearningTask
-PUT/api/LearningTask/id
-GET/api/LearningTask/id
-DELETE/api/LearningTask/id

-GET/api/TaskAssignment
-POST/api/TaskAssignment
-PUT/api/TaskAssignment/id
-GET/api/TaskAssignment/id

-GET/api/Submission
-POST/api/Submission
-GET/api/Submission/id

-GET/api/Review
-POST/api/Review
-GET/api/Review/id

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
Day 3 (10th July)
    -Protected APIs JWT token Authorization
    -Added pagination, search and filter to GET Trainees
    -Configured Cors for phase 3 react
    -Added structured logging
Day 4 (13th July)
    -Created Mentor module with Get,Get by Id, Post, Put and Delete
    -Created Learning Task module with Get,Get by Id, Post, Put and Delete
Day 5 (14th July)
    -Created Task Assignment Module with Post, Get, Get by Id and Put Api
    -Created Submission Module with Post, Get and Get by Id
    -Created Review Module with Post and Get, Get by Id
    -Global Exception Handling Middleware implemented

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

## Login and JWT Authorization for testing
Admin login is 
username = "admin"
password = "admin@123"
After logging in, in the response body you will find the JWT token labeled as token.
Copy the token and click on Authorize button. Paste the token in the value field and press Authorize.
All APIs should be available for testing.

##Security Checklist
-User Authentication
-Password Hashing
-JWT Token Authorization
-Global Exception Handling

## Improvements Planned
-Adding React Frontend
