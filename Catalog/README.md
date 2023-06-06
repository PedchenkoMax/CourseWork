# 📱 Catalog microservice

### 📌 Table of contents

- [|❓ Main responsibilities](#-main-responsibilities)
- [|🧱 Technology stack](#-technology-stack)
- [|🚀 How to start](#-how-to-start)

### |❓ Main responsibilities

### |🧱 Technology stack

- **SDK:** `.NET 7`
- **Framework:** `ASP.NET Core 7.0`
- **Persistence:**
    - Database: `PostgreSQL`
    - ORM: `Dapper`
    - Migrations: `FluentMigrator`
- **Cache Provider:** `Redis`
- **Blob Storage:** `MinIO`
- **Messaging:**
    - Bus: `MassTransit`
    - Broker: `RabbitMQ`
- **Testing:**
    - Unit Testing: `XUnit`, `FluentAssertions`,
    - Integration Testing: `XUnit`, `FluentAssertions`, `Testcontainers`
- **CI/CD:** `GitHub Actions`
- **Containerization:** `Docker`
- **API Documentation:** `OpenAPI (Swagger)`

### |🚀 How to start

#### To clone and run this application, you'll need [Git](https://git-scm.com) and [Docker](https://www.docker.com/get-started). From your command line:

```bash
# Clone the repository
$ git clone --branch dev https://github.com/PedchenkoMax/CourseWork.git

# Navigate to the src folder
$ cd Catalog

# Create a .env file from the .env.example file
$ cp .env.example .env

# Edit the .env file to set your environment variables, if needed

# Build and run app with Docker Compose
$ docker-compose up --detach

--- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- --- 

# Go to the API
Use a REST client like Postman to access the API endpoints (http://localhost:42000/api)

# Access the API documentation using Swagger
Open your browser and go to http://localhost:42000/ to view and interact with the API documentation.

# Access Seq
Open your browser and go to http://localhost:5341/

# Access Health Checks UI
Open your browser and go to http://localhost:42000/health-ui

# Access Minio blob (username:minio123, password:minio123)
Open your browser and go to http://localhost:9001/browser

# Access RabbitMq (username:guest, password:guest)
Open your browser and go to  http://localhost:15672/
```


