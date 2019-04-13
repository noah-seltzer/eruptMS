# Erupt Management Systems

Academic project by an 11-member team developing a project management system for a fictional company with 2000+ employees.

• Web application built with ASP.NET Core MVC with Entity Framework Core, SQL Server, Docker, and Bootstrap
• For managing employees, projects, work packages within projects, timesheets, and report generation

# IMPORTANT For Developers


change the default connection in COMP4911Timesheets/appsettings.json

FROM

"DefaultConnection": "CONNECTION_STRING"

TO


"DefaultConnection": "Server=(localdb)\\\\mssqllocaldb;Database=4911;Trusted_Connection=True;MultipleActiveResultSets=true"
Edmund is using "Server=tcp:127.0.0.1,1433;Database=Erupt;UID=sa;PWD=Password123;"


# comp4911timesheets

[Code Conventions](/docs/conventions.md)
[Database Setup](/docs/dbsetup.md)


#### Deployment Instructions

Pre-install: 
- Create an mssql database which can be accessed by the intended host
- install docker on the users host

Stage 1: checkout

1. checkout source from https://github.com/noah-seltzer/comp4911timesheets
2. CD into comp4911timesheets folder

Stage 2: configure environment files

1. Replace CONNECTION_STRING in /COMP4911Timesheets/appsettings.json with a valid connection string. 
NOTE: as this is a docker depoyment, localhost, 127.0.0.1 and 0.0.0.0 will not function the way you may expect. use the IP address of the docker host
2. (optional) In build.yml, Customize the container name from eruptTEST to something more appropriate to your configuration
3. (optional) In build.yml Customize the outgoing port from 5000 to another port.

Stage 3: build container

1. from the outermost project folder, which contains build.yml, run the console command 
`docker-compose -f build.yml up --build`
(optional) in production, run `docker-compose -f build.yml up --build -d`. the -d flag detaches your console from the container
2. Point your browser to localhost:{the-port-you-chose)
3. Sign in with 
Username: cameron.lay@infosys.ca
Password: Password123!
