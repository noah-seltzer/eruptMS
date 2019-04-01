# IMPORTANT 

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
#### To deploy on docker with live compiling (windows tested only)

1. navigate to project folder (outer folder)
2. run ```docker-compose -f run.yml up --build```
   - on mac ```docker-compose -f mac_run.yml up --build```
3. point browser to localhost:5000 (NOT https://localhost:5000)

#### To build and run a production docker container
1.  navigate to project folder
2.  run `docker-compose -f build.yml up --build`
3.  point browser to localhost:5000

##### To run in background

append `-d` to the end of the docker-compose command

`docker-compose -f run.yml up --build -d`

##### visual studio

To debug, set solution configuration (drop down on toolbar) to debug and . this will activate break points and other debug features (will not recompile live)

To build and run a production container set solution configuration to realease. 

##### Important docker commands
these commands are compatible with powershell and bash

- stop all docker containers `docker stop $(docker ps -a -q)`
- remove all stopped docker containers `docker rm $(docker ps -a -q)`
- list docker images `docker images`
- list docker containers which are running `docker ps`
  - `docker ps -a` to list stopped containers as well
- to open a bash console inside of the container `docker exec -i -t container_name /bin/bash` 
- list docker images `docker images`

Troubleshooting:

Restart docker for windows!

##Branches
Master > UAT > STAGE > TEST
Master is protected, requiring 2 code reviewers	
UAT & STAGE are unprotected, requiring 1 code reviewer	
TEST is unprotected. please branch off from this.
