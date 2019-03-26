pipeline {
  agent any
  stages {
    stage('set properties') {
      steps {
        sh 'sudo sed -i "s/eruptTEST/$containerName/g" $WORKSPACE/build.yml'
        sh 'cat build.yml'
        sh 'sudo sed -i "s/:5000/:$port/g" $WORKSPACE/COMP4911Timesheets/Properties/launchSettings.json'
        sh 'cat $WORKSPACE/COMP4911Timesheets/Properties/launchSettings.json'
        sh 'sudo sed -i "s/"DefaultConnection": "*"/"DefaultConnection": "Server=localhost,1433;Database=erupt$containerName;User ID=SA;Password=Password!123;/g" $WORKSPACE/COMP4911Timesheets/appSettings.json'
        sh 'cat $WORKSPACE/appSettings.json'
      }
    }
    stage('build container') {
      steps {
        sh 'sudo docker-compose -f build.yml up --build -d'
      }
    }
  }
  environment {
    eruptPassword = 'Password!123'
    containerName = sh (returnStdout: true, script: 'echo erupt$GIT_BRANCH').trim()
    port = sh (returnStdout: true, script: 'cat ./COMP4911Timesheets/Properties/$GIT_BRANCH').trim()
  }
}