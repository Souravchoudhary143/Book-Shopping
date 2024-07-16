# Book-Shopping Web Application

## Overview

The Book-Shopping web application is a comprehensive e-commerce platform designed to facilitate online transactions, inventory management, and customer interactions. Built with ASP.NET Core (MVC), HTML, CSS, Bootstrap, JavaScript, and C#, this solution integrates various components to ensure seamless operation and an enhanced user experience.

## Key Features

### Project Based on SOLID Principles
- The application adheres to SOLID principles to ensure maintainability, scalability, and robustness of the codebase.

### User Management
- **Secure Registration and Authentication**: Utilizes the Identity framework for secure user registration and authentication.
- **Role-Based Access Control**: Assigns roles such as Admin, Employee, Company, and User to control data access and authority.
- **Social Login Options**: Supports login via Google, Facebook, LinkedIn, and Instagram.
- **Two-Factor Authentication**: Enhances security by adding an extra layer of authentication.

### Product Management
- **Shopping Cart**: Fully functional shopping cart that allows adding, updating, and deleting products.
- **Product Selection**: Feature to select products from the cart with a tick mark.
- **Home Page Display**: Products are displayed based on the quantity sold, and it also shows how many times each book has been sold.

### Order Processing
- **Efficient Order Placement**: Streamlined process for placing orders.
- **Order Tracking and Management**: Features to track and manage orders efficiently.

### Payment Integration
- **Secure Payment Gateways**: Integration with Stripe, Google Pay, and PayPal for smooth and secure transactions.

### Order Management
- **Admin Role**: Provides full authority to admins to view all orders placed with complete details.

## Technologies Used
- **Frontend**: HTML, CSS, Bootstrap, JavaScript
- **Backend**: ASP.NET Core (MVC), C#
- **Database**: SQL Server
- **Authentication**: Identity Framework, Two-Factor Authentication
- **Payment Gateways**: Stripe, Google Pay, PayPal
- **Version Control**: Git

## Project Structure
- **Controllers**: Handle HTTP requests and responses.
- **Models**: Define the structure of the data.
- **Views**: Present data to the user using Razor view engine.
- **Services**: Implement business logic and handle data processing.

## Installation and Setup
1. **Clone the Repository**:
   ```bash
   git clone https://github.com/Souravchoudhary143/Book-Shopping.git
