# README.md — EShopNETCoreMVC

# EShop

Personal e-commerce web application built with ASP.NET Core MVC.

The project was created to practice building a database-driven web application with user authentication and integration with external services.

## Technologies

* C#
* ASP.NET Core MVC
* ASP.NET Core 3.1
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* Stripe
* SendGrid
* Google Authentication
* Facebook Authentication
* Razor Pages / Razor Views

## Main technical areas

### Database

Entity Framework Core is used with SQL Server for persistence.

### Authentication

ASP.NET Core Identity provides the application authentication system.

The project also contains external authentication integrations for:

* Google
* Facebook

### Payments

Stripe is integrated as an external payment service.

### Email

SendGrid is used as an external email service.

### Sessions

The application uses ASP.NET Core session support for maintaining user-related application state.

## Project structure

```text
EShopNETCoreMVC
└── EshopApp
    ├── Controllers
    ├── Data
    ├── Models
    ├── Services
    ├── Views
    └── Areas
```

## Purpose of the project

The project was created to gain practical experience with:

* ASP.NET Core MVC
* Entity Framework Core
* SQL Server
* ASP.NET Core Identity
* authentication and authorization
* external service integration
* database-driven web applications
* organizing application code

## Configuration

External services require configuration values such as API keys, connection strings, and authentication credentials.

## Project status

This is a personal learning project created with an older ASP.NET Core version.
