# Shop_Cart_Project
This is my online shop project. It enables functions for customers and admin. 
Customers can check the offer, add products to cart and then buy these things. 
Admin can add, update and remove items and categories, can also view orders sorted into different categories.
I used Entity Framework Core for database connection (SQL Server). Connection is realized by context inherits form IdentityDbContext. 
Repository is generic with common methods: add, get, get all, remove, remove range. Specific repositories have other methods.
UnitOfWork is container for repositories and it is inject in Program.cs file. 

Architecture (n-tier)
- ASP.NetCMS_Cart - controllers, views, static files
- DataAccess - dataBase context, migrations, repository and unitOfWork patterns, Models prepared for views
- Models - domain models for reality modeling
- Utility - useful enumerations and classes(email sender, roles)
Requirements:
- SOLID 
- DRY
- KISS
- CLEAN CODE
