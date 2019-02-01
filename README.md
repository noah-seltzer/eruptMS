# comp4911timesheets

[Code Conventions](/docs/conventions.md)
[Database Setup](/docs/dbsetup.md)

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
