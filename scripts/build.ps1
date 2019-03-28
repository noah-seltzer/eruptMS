docker build -t comp4911timesheets ..\COMP4911Timesheets\COMP4911Timesheets\
docker run -d -p 5000 :80 --name comp4911timesheets comp4911timesheets