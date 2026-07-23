# TraineeManagement.Api

A RESTful ASP.NET Core Web API developed as part of my .NET Backend Training. The project demonstrates modern backend development practices including authentication, CRUD operations, file handling, distributed caching, asynchronous messaging, background processing, inter-service communication, and Dockerized deployment.

---

# Technology Stack

- ASP.NET Core Web API (.NET 10)
- C#
- Entity Framework Core
- MySQL
- Swagger / OpenAPI
- JWT Authentication
- Redis
- RabbitMQ
- .NET Worker Service
- IHttpClientFactory
- Docker
- Docker Compose
- AWS CodeArtifact

---

# Solution Architecture

The solution consists of three projects:

## TraineeManagement.Api

Main REST API responsible for:

- Authentication
- CRUD Operations
- File Upload & Download
- Redis Cache
- RabbitMQ Publisher
- Health Checks

---

## SubmissionProcessor.Worker

Background Worker responsible for:

- RabbitMQ Consumer
- Submission Processing
- Retry Handling
- Processing Status Updates
- Idempotent Message Processing

---

## TrainingDirectory.Api

Internal service used for:

- Trainee Summary
- Assignment Summary

Demonstrates service-to-service communication using IHttpClientFactory.

---

# API Endpoints

## Health

| Method | Endpoint |
|---------|----------|
| GET | `/api/Health` |
| GET | `/health/live` |
| GET | `/health/ready` |

---

## Authentication

| Method | Endpoint |
|---------|----------|
| POST | `/api/Auth/register` |
| POST | `/api/Auth/login` |

---

## Trainee

| Method | Endpoint |
|---------|----------|
| GET | `/api/Trainee` |
| POST | `/api/Trainee` |
| GET | `/api/Trainee/{id}` |
| PUT | `/api/Trainee/{id}` |
| DELETE | `/api/Trainee/{id}` |

Supports:

- Search
- Pagination
- Filtering

---

## Mentor

| Method | Endpoint |
|---------|----------|
| GET | `/api/Mentor` |
| POST | `/api/Mentor` |
| GET | `/api/Mentor/{id}` |
| PUT | `/api/Mentor/{id}` |
| DELETE | `/api/Mentor/{id}` |

---

## Learning Task

| Method | Endpoint |
|---------|----------|
| GET | `/api/LearningTask` |
| POST | `/api/LearningTask` |
| GET | `/api/LearningTask/{id}` |
| PUT | `/api/LearningTask/{id}` |
| DELETE | `/api/LearningTask/{id}` |

---

## Task Assignment

| Method | Endpoint |
|---------|----------|
| GET | `/api/TaskAssignment` |
| POST | `/api/TaskAssignment` |
| GET | `/api/TaskAssignment/{id}` |
| PUT | `/api/TaskAssignment/{id}` |

---

## Submission

| Method | Endpoint |
|---------|----------|
| GET | `/api/Submission` |
| POST | `/api/Submission` |
| GET | `/api/Submission/{id}` |
| GET | `/api/Submissions/{id}/summary` |

---

## Submission Files

| Method | Endpoint |
|---------|----------|
| POST | `/api/Submissions/{submissionId}/files` |
| GET | `/api/SubmissionFiles/{id}` |
| GET | `/api/SubmissionFiles/{id}/download` |
| DELETE | `/api/SubmissionFiles/{id}` |

---

## Review

| Method | Endpoint |
|---------|----------|
| GET | `/api/Review` |
| POST | `/api/Review` |
| GET | `/api/Review/{id}` |

---

## Processing Jobs

| Method | Endpoint |
|---------|----------|
| GET | `/api/ProcessingJobs/{id}` |
| POST | `/api/ProcessingJobs/{id}/retry` |

---

# Features Implemented

## Phase 1

### Day 1

- Swagger UI
- In-memory data storage
- Health Check API
- Get All Trainees
- Get Trainee by Id
- Create Trainee

### Day 2

- Update Trainee
- Delete Trainee
- DTOs
- Input Validation
- Service Layer

### Day 3

- Entity Framework Core
- Async APIs
- Search Functionality

---

## Phase 2

### Day 1

- MySQL Integration
- Entity Framework Migrations
- Persistent Database

### Day 2

- User Registration
- User Login
- Password Hashing
- JWT Generation

### Day 3

- JWT Authorization
- Search
- Pagination
- Filtering
- CORS
- Structured Logging

### Day 4

- Mentor Module
- Learning Task Module

### Day 5

- Task Assignment Module
- Submission Module
- Review Module
- Global Exception Middleware

---

## Phase 3

### Day 1

- File Storage Abstraction
- Local File Storage
- Upload API
- Download API
- Delete API
- Metadata Storage
- File Validation
- Upload Limits
- Server-generated File Names
- Secure File Handling

---

### Day 2

- Redis Configuration
- Cache-Aside Pattern
- TTL
- Cache Invalidation
- Graceful Cache Failure
- MySQL Fallback

Example Keys

```
trainee:{id}
submission-summary:{id}
```

---

### Day 3

- Durable Queue
- Persistent Messages
- Versioned Message Contract
- Publisher
- Consumer

---

### Day 4

- Worker Service
- Processing Jobs
- Retry Handling
- Idempotency
- Duplicate Message Detection
- Processing Status Tracking

Status Flow

```
Queued
↓

Processing
↓

Completed
```

or

```
Queued
↓

Processing
↓

Failed
```

---

### Day 5

- TrainingDirectory API
- Typed HttpClient
- IHttpClientFactory
- Timeout Configuration
- Retry Policy
- Standard Resilience Handler

---

### Day 6

- Structured Logging
- Correlation IDs
- Health Checks
- End-to-End Workflow Monitoring

---

### Day 6

Dockerized:

- TraineeManagement.Api
- SubmissionProcessor.Worker
- TrainingDirectory.Api

Docker Compose includes:

- MySQL
- Redis
- RabbitMQ
- API
- Worker
- TrainingDirectory

Implemented:

- Multi-stage Builds
- Health Checks
- Persistent Volumes
- Environment Variables
- Restart Policies
- Service Discovery

---

# Security Features

- JWT Authentication
- Password Hashing
- Authorization
- Secure File Upload
- File Validation
- Global Exception Handling
- Structured Logging
- Input Validation

---

# Performance Features

- Redis Distributed Cache
- Cache-Aside Pattern
- RabbitMQ
- Background Worker
- Async Processing
- HTTP Resilience

---

# Docker Architecture

```
                  Client
                     │
                     ▼
          TraineeManagement.Api
          │         │          │
          ▼         ▼          ▼
      MySQL      Redis     RabbitMQ
                                │
                                ▼
               SubmissionProcessor.Worker
                                │
                                ▼
                  TrainingDirectory.Api
```

---

# Running the Project

## 1. Clone Repository

```bash
git clone <repository-url>
```

---

## 2. Create Environment File

Create a `.env` file and configure:

- MySQL
- Redis
- RabbitMQ
- JWT
- AWS CodeArtifact
- Storage Location

---

## 3. Start Docker

```bash
docker compose up --build
```

---

## 4. Apply Database Migrations

```bash
dotnet ef database update
```

---

## 5. Run

Swagger:

```
http://localhost:5000/swagger
```

RabbitMQ Management:

```
http://localhost:15672
```

TrainingDirectory API:

```
http://localhost:8080
```

---

# Authentication

Default Admin

Username

```
admin
```

Password

```
admin@123
```

Login using:

```
POST /api/Auth/login
```

Copy the returned JWT token and authorize using Swagger.

---

# End-to-End Workflow

```
Authenticate

↓

Upload Submission

↓

Validate Request

↓

Store File Metadata

↓

Store Physical File

↓

Publish RabbitMQ Message

↓

Worker Consumes Message

↓

Retrieve Training Information

↓

Process Submission

↓

Update Job Status

↓

Invalidate Redis Cache

↓

Completed
```

---

# Future Improvements

- React Frontend
- Cloud File Storage (Amazon S3 / Azure Blob Storage)
- Kubernetes Deployment
- CI/CD Pipeline
- Automated Integration Testing

---

# Author

Developed as part of the **Backend Services Training Program** to learn modern ASP.NET Core backend development, secure API design, distributed systems, asynchronous messaging, and Docker-based deployment.