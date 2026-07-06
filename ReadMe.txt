This project was made as part of my training to learn dotnet.
I followed the daily steps provided in the "Three_Day_DotNet_API_InMemory_Training_Plan.pdf"

# TraineeMangement.Api

## Technology Used
-ASP.Net Web Core
-C#
-.Net
-Swagger

## Features Completed
Day 1 of dotnet project (3rd July)
-Swagger UI for API testing
-In-memory data storage using List<Trainee>
-Health Check endpoint (GET)
-Get all Trainees (GET)
-Get Trainee by Id (Get/{id})
-Create new trainee (POST)
Day 2 of dotnet project (6th July)
-Update Trainee details (PUT)
-Delete Trainee (Delete)
-Request and response DTOs
-Input Validation
-Service Layer Implementation


## How to Run
-Clone Repository
-Dotnet Restore ( Restores Nuget packages )
-Dotnet Run

## Challenges Faced
I faced difficulties in installing Nuget packages as we couldn't access the packages on https://api.nuget.org/v3/index.json.
After asking some seniors I came to know we had to access the nuget packages through AWS. Setting that up took time.

## Improvements Planned
-Integrating Microsoft.EntityFrameworkCore.InMemory 
-Adding search API
