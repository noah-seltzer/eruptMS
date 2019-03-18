# Table of Contents
- [Set up EF Core with model classes](#set-up-ef-core-with-model-classes)
- [Deploy SQL DB on Azure](#deploy-sql-db-on-azure)
- [Troubleshooting](#troubleshooting)

## Set up EF Core with model classes
1. Make sure that correct `Connection_String` is set
2. If there are any changes on database schema (which is likely always the case), run `dotnet ef database drop` and `dotnet ef migrations remove`
3. Then run `dotnet ef migrations add EruptMigration -c ApplicationDbContext -o Data/Migrations`
4. Run `dotnet ef database update`
5. You should be able to see the tables created

## Deploy SQL DB on Azure
1.  Go to [portal.azure.com](http://portal.azure.com)
2.  Log in with your ID/Password
3.  Click SQL Databases on the left panel
4.  Click Add button
5.  Configure your DB with proper name(something that is unique and easy to remember for the project)
6.  For resource group, you can make anything you want, but I divided to to, “Intranet” and “Project” and keep using the one already exists
7.  Leave source to be empty
8.  For Server, I am also using only one, although I have multiple databases. I made intranet1 server, and all my databases are using the same server
9.  Select Not Now for elastic pool
10.  Select Basic pricing tier
11.  Click create button
12.  It will be created in a minute
13.  Open the database once it finishes deployment
14.  Set the System Environment Variable to
	   Key: 
	   `ASPNET_CONN`
	   Value (This is your actual connection strings): 
	   `Server=tcp:{YOUR_SERVER_NAME}.database.windows.net,1433;Initial Catalog={YOUR_DB_NAME};Persist Security Info=False;User ID={YOUR_ID};Password={YOUR_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`

 15. Set your System Environment Variable to `LOCAL_PASSWORD`=`Test123123`. This is not important to you guys if you're using Azure, not localhost. Set anything you like.
16. Done!!!
17. `$ dotnet run` on windows

## Troubleshooting
| Issue | Solution |
|--|--|
| Any `dotnet ef` command occurs the following error `error MSB4057: The target "GetEFProjectMetadata" does not exist in the project.` | Mac<br> `dotnet ef migrations list --msbuildprojectextensionspath obj/local` <br>Windows<br> `dotnet ef migrations list --msbuildprojectextensionspath obj\local` |
