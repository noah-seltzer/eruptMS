# comp4911timesheets

to deploy on docker with live compiling (windows tested only)

1. navigate to project folder (outer folder)
2. run ```docker-compose -f run.yml up --build```
  - on mac ```docker-compose -f mac_run.yml up --build```
3. point browser to localhost:5000 (NOT https://localhost:5000)

Can also use visual studio

To debug, set solution configuration (drop down on toolbar) to debug. this will activate break points and other debug features (will not recompile live)

To build and run a production container set solution configuration to realease. 

Troubleshooting:

Restart docker 

##Branches
Master > UAT > STAGE > TEST
Master is protected, requiring 2 code reviewers
UAT & STAGE are unprotected, requiring 1 code reviewer
TEST is unprotected. please branch off from this.

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
	   `ConnectionStrings_DefaultConnection`
	   Value (This is your actual connection strings): 
	   `Server=tcp:{YOUR_SERVER_NAME}.database.windows.net,1433;Initial Catalog={YOUR_DB_NAME};Persist Security Info=False;User ID={YOUR_ID};Password={YOUR_PASSWORD};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;`
15.  Done!!!
16.  `$ dotnet run` on windows
