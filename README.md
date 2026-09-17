ABC Retail Management System
Overview
ABC Retail Management System is a cloud-based retail management application developed using ASP.NET Core MVC and Microsoft Azure.
The system is designed to support the management of products, customers, orders and background processing using Azure cloud services. It demonstrates how a traditional retail application can be migrated to a scalable cloud-based solution.
The application uses Azure Storage services for storing and processing retail information and Azure Functions for background processing.

Technologies Used
•	ASP.NET Core MVC
•	C#
•	.NET 10
•	Entity Framework Core
•	ASP.NET Core Identity
•	Microsoft Azure
•	Azure Storage Account
•	Azure Table Storage
•	Azure Blob Storage
•	Azure Queue Storage
•	Azure File Storage
•	Azure Functions
•	SQL Server / Azure SQL Database
•	HTML
•	CSS
•	Bootstrap
•	JavaScript
•	Git and GitHub
•	Visual Studio

Main Features
1. Dashboard
The home page provides access to the main sections of the ABC Retail Management System.
The dashboard provides navigation to:
•	Manage Products
•	Manage Customers
•	 Orders
•	Queue Processing
•	Logs 

2. Product Management
The product management functionality allows administrators to:
•	Add products
•	View products
•	Edit products
•	Delete products
•	Store product information in Azure Table Storage
•	Upload product images
•	Retrieve product images from Azure Blob Storage
Product information includes details such as:
•	Product name
•	Description
•	Price
•	Quantity
•	Product image

3. Customer Management
The customer management functionality allows the system to store and manage customer information.
Customer information includes:
•	Customer name
•	Email address
•	Phone number
•	Address
Customer data is stored using Azure Table Storage.

4. Order Management
The system supports the creation and management of customer orders.
Orders can contain information such as:
•	Customer
•	Product
•	Quantity
•	Total amount
•	Order status
Orders can also be submitted to Azure Queue Storage for background processing.

Azure Services
Azure Table Storage
Azure Table Storage is used for storing structured NoSQL data.
The application uses tables for information such as:
•	Products
•	Customers
•	Order
Table Storage provides a scalable way to store retail data without relying entirely on a relational database.

Azure Blob Storage
Azure Blob Storage is used for storing product multimedia, particularly product images.
The application uploads images to an Azure Blob container and stores the corresponding image URL with the product information.
Example container:
product-images

Azure Queue Storage
Azure Queue Storage is used for asynchronous processing.
The application can place messages into queues for background processing.
Queues include functionality for:
order-processing
inventory-processing
image-processing
This allows resource-intensive operations to be handled asynchronously instead of blocking the main web application.

Azure File Storage
Azure File Storage is used for application-related files and logging requirements.
The application uses an Azure File Share for storing application logs.
Example file share:
application-logs

Azure Functions
The project contains a separate Azure Functions project:
ABCRetail.Functions
The Functions project targets:
.NET 8
Azure Functions are used to process background tasks and Azure Storage Queue messages.
This allows queue processing to happen independently from the main MVC application.

Authentication and Authorization
The application uses ASP.NET Core Identity for authentication.
Users can:
•	Register an account
•	Log in
•	Log out
Identity is configured with password requirements to improve account security.
The application can also restrict administrative functionality to authenticated or authorized users.

Project Structure
The solution contains the following main projects:
ABCRetail
Controllers
•	AccountController.cs
•	CustomersController.cs
•	HomeController.cs
•	OrdersController.cs
•	ProductsController.cs

 Data
•	ABCRetailContext.cs

 Models
•	ApplicationUser.cs
•	Customer.cs
•	Order.cs
•	Product.cs

Services
•	TableStorageService.cs
•	BlobStorageService.cs
•	QueueStorageService.cs
•	FileStorageService.cs

Views
•	Account
•	Customers
•	Home
•	Orders
•	Products
•	Shared

wwwroot
Program.cs
appsettings.json
ABCRetail.csproj
Azure Functions project:
ABCRetail.Functions
•	Functions	
Program.cs
host.json
local.settings.json
 ABCRetail.Functions.csproj

Database
The application also uses SQL Server / Azure SQL Database for relational data and ASP.NET Core Identity.
The Azure SQL connection is configured using the application configuration system.
Example:
"ConnectionStrings": {
  "DefaultConnection": "YOUR_DATABASE_CONNECTION_STRING"
}
The actual connection string should not be committed to GitHub.

Azure Storage Configuration
The Azure Storage connection is configured separately.
Example:
"AzureStorage": {
  "ConnectionString": ""
}
The actual Azure Storage connection string must not be stored directly in the Git repository.
For local development, use a secure configuration method such as:
•	.NET User Secrets
•	Environment variables
•	Azure App Service Application Settings

Local Development Setup
Prerequisites
Install the following:
•	Visual Studio
•	.NET SDK
•	Azure account
•	SQL Server or access to Azure SQL Database
•	Azure Storage Account
•	Azure Functions tooling

1. Clone the Repository
Clone the project from GitHub:
git clone : gh repo clone PalesaCindi/ABCRetail2
https://github.com/PalesaCindi/ABCRetail2.githttps://github.com/PalesaCindi/ABCRetail2.git
Navigate into the project:
cd ABCRetail

2. Open the Solution
Open the solution in Visual Studio.
Build the solution:
dotnet build
The project should build without errors.

3. Configure the Database
Add the database connection string using a secure configuration method.
Do not commit credentials or passwords to GitHub.

4. Configure Azure Storage
Create an Azure Storage Account containing the required storage services.
Configure the Azure Storage connection string using User Secrets or environment variables.
For example, the application expects:
AzureStorage:ConnectionString

5. Create Required Azure Storage Resources
Create the required resources:
Tables
Products
Customers
Blob Container
product-images
Queues
order-processing
inventory-processing
image-processing
File Share
application-logs

Running the Application
From Visual Studio, select the ABCRetail project and run the application.
Alternatively:
dotnet run
The application will start using the configured ASP.NET Core development environment.

Running the Azure Functions Project
The Functions project is located in:
ABCRetail.Functions
The project targets .NET 8.
To build the Functions project:
dotnet build
To publish it:
dotnet publish
The published files are generated under:
ABCRetail.Functions\bin\Release\net8.0\publish\

Application Publishing
The ASP.NET Core application can be published to Azure App Service.
The ASP.NET Core application publishes to:
ABCRetail\bin\Release\net10.0\publish\
The Azure Functions application publishes to:
ABCRetail.Functions\bin\Release\net8.0\publish\
Before deploying, make sure the required Azure Application Settings and connection strings have been configured.

Security
Sensitive information must never be committed to source control.
The following should not contain real credentials:
appsettings.json
appsettings.Development.json
local.settings.json
Sensitive values include:
•	Azure Storage access keys
•	Azure Storage connection strings
•	Database passwords
•	API keys
•	Authentication secrets
Use secure configuration such as:
•	User Secrets
•	Environment variables
•	Azure App Service Configuration
•	Azure Key Vault

GitHub Push Protection
GitHub Secret Scanning and Push Protection are enabled for the repository.
If GitHub detects a secret in a commit, the push will be rejected.
For example:
GH013: Repository rule violations found
Push cannot contain secrets
Secrets should be removed from both the current files and Git history before pushing.
Never bypass GitHub's secret protection by intentionally pushing an exposed credential.
If an Azure access key has been exposed, regenerate the key in Azure and update the application with the new credential using secure configuration.

Testing
Before deployment, perform the following checks:
Application
•	Build the solution
•	Run the application
•	Test login and registration
•	Test product CRUD operations
•	Test customer operations
•	Test order processing
•	Test image upload
Azure Storage
•	Verify Products table
•	Verify Customers table
•	Verify product images in Blob Storage
•	Verify queue messages
•	Verify application logs
Azure Functions
•	Verify Functions build successfully
•	Verify queue-triggered functions
•	Verify messages are processed correctly

Build Verification
The application can be verified using:
dotnet build
A successful build should display:
Build succeeded.
0 Warning(s)
0 Error(s)
The application can also be tested with:
dotnet publish
A successful publish generates the deployment files in the publish directory.

Architecture
The application follows a cloud-based architecture:
 

Project Status
The application includes:
•	ASP.NET Core MVC web application
•	ASP.NET Core Identity authentication
•	Azure Table Storage integration
•	Azure Blob Storage integration
•	Azure Queue Storage integration
•	Azure File Storage integration
•	Azure Functions project
•	SQL Server / Azure SQL integration
•	Product management
•	Customer management
•	Order management
•	Background queue processing
•	Azure deployment support

Author
Palesa Cindi
ABC Retail Management System
Software Development / Application Development Project

