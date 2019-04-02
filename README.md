#### Final deployment instructions

Pre-install: 
- Create an mssql database which can be accessed by the intended host

Stage 1: configure environment

1. Replace CONNECTION_STRING in /COMP4911Timesheets/appsettings.json with a valid connection string. 
NOTE: as this is a docker depoyment, localhost, 127.0.0.1 and 0.0.0.0 will not function the way you may expect.
2. (optional) In build.yml, Customize the container name from eruptTEST to something more appropriet to your configuration
3. (optional) In build.yml Customize the outgoing port from 5000 to another port

Stage 2: build container

1. from the outermost project folder, which contains build.yml, run the console command 
`docker-compose -f build.yml up --build -d`
(optional) remove -d if you'd like to keep the kestrel web server's output



# IMPORTANT (for developers)

change the default connection in COMP4911Timesheets/appsettings.json

FROM

"DefaultConnection": "CONNECTION_STRING"

TO

"DefaultConnection": "Server=(localdb)\\\\mssqllocaldb;Database=4911;Trusted_Connection=True;MultipleActiveResultSets=true"
Edmund is using "Server=tcp:127.0.0.1,1433;Database=Erupt;UID=sa;PWD=Password123;"

# comp4911timesheets

[Code Conventions](/docs/conventions.md)
[Database Setup](/docs/dbsetup.md)

# Environments

http://deimos.edu.bcit.ca -> TEST

http://deimos.edu.bcit.ca:5000 -> STAGE

http://deimos.edu.bcit.ca:5001 -> UAT (not initilized, builds will fail for the time being)

http://deimos.edu.bcit.ca:5001 -> JENKINS (noah's branch)

#### Production Deployment Instructions

Pre-install: 
- Create an mssql database which can be accessed by the intended host
- keep 

Stage 1: configure environment

1. Replace CONNECTION_STRING in /COMP4911Timesheets/appsettings.json with a valid connection string. 
NOTE: as this is a docker depoyment, localhost, 127.0.0.1 and 0.0.0.0 will not function the way you may expect.
2. (optional) In build.yml, Customize the container name from eruptTEST to something more appropriet to your configuration
3. (optional) In build.yml Customize the outgoing port from 5000 to another port

Stage 2: build container

1. from the outermost project folder, which contains build.yml, run the console command 
`docker-compose -f build.yml up --build -d`
(optional) remove -d if you'd like to keep the kestrel web server's output

