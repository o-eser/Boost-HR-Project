# Human Resources Panel

The Human Resources Panel is a web project developed using Asp.Net technology with the Onion architecture on the API side. The project encompasses the following key features and utilizes specific technologies:

## Technologies and Architecture

•	Backend Development: Developed using Asp.Net Core and designed with the Onion architecture.

•	Database Approach: Implemented using the code-first approach with Entity Framework Core.

•	Database: MSSQL is used as the database.

•	IoC Container: Autofac IoC container is employed.

•	Repository Design: The project utilizes the generic repository design.

•	Storage: Azure Blob Storage is employed for image and file processing.

•	Security and Authorization

•	Security is ensured on the API side through the utilization of JWT tokens.

•	User authorization is implemented during API communication.

## Client-Side

•	Frontend Development: Created using Asp.Net MVC.

•	API Communication: Interaction between the client and API, data transfer, login, and register processes are facilitated through a custom ApiService.

•	Email Processing: Mail-related operations are managed on the client side.

•	User Profiles

•	The project is designed to accommodate three distinct user profiles:

### Administrator:

•	User management through CRUD operations.

•	Special permissions and controls.

### Company Manager:

•	Internal management and reporting capabilities within the company.

### Employee:

•	Updating personal information and internal communication.

Developed with the Onion architecture and modern technologies, this project offers a comprehensive solution for human resources management.
