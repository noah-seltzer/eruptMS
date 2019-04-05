pipeline {
  agent any
  stages {
    stage('Set Properties') {
      parallel {
        stage('Set Properties') {
          steps {
            sh 'sudo sed -i "s/eruptTEST/$containerName/g" $WORKSPACE/build.yml'
            sh 'cat build.yml'
            sh 'sudo sed -i "s/5000:80/$port:80/g" $WORKSPACE/build.yml'
            sh 'cat $WORKSPACE/COMP4911Timesheets/Properties/launchSettings.json'
            sh 'sudo sed -i "s/CONNECTION_STRING/Server=142.232.78.195,1433;Database=$containerName;User ID=SA;Password=$dbpassword;/g" ./COMP4911Timesheets/appsettings.json'
            sh 'cat $WORKSPACE/COMP4911Timesheets/appsettings.json'
          }
        }
        stage('Unit Test') {
          steps {
            sh 'dotnet test'
          }
        }
      }
    }
    stage('Stop Container') {
      steps {
        sh 'sudo docker stop $containerName'
        sh 'sudo docker rm $containerName'
      }
    }
    stage('Build Container') {
      steps {
        sh 'sudo docker-compose -f build.yml up --build -d'
      }
    }
  }
  environment {
    eruptPassword = 'Password!123'
    containerName = sh (returnStdout: true, script: 'echo erupt$GIT_BRANCH').trim()
    port = sh (returnStdout: true, script: 'cat ./COMP4911Timesheets/Properties/$GIT_BRANCH').trim()
    dbpassword = 'Password!123'
  }
}