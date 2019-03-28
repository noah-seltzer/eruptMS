pipeline {
  agent any
  stages {
    stage('Set Properties') {
      steps {
        sh 'sudo sed -i "s/eruptTEST/$containerName/g" $WORKSPACE/build.yml'
        sh 'cat build.yml'
        sh 'sudo sed -i "s/:5000/:$port/g" $WORKSPACE/COMP4911Timesheets/Properties/launchSettings.json'
        sh 'cat $WORKSPACE/COMP4911Timesheets/Properties/launchSettings.json'
        sh 'sudo sed -i "s/CONNECTION_STRING/Server=localhost,1433;Database=$containerName;User ID=SA;Password=$dbpassword;/g" ./COMP4911Timesheets/appsettings.json'
        sh 'cat $WORKSPACE/COMP4911Timesheets/appsettings.json'
      }
    }
    stage('Stop Container') {
      steps {
        sh 'echo sudo docker stop $containerName'
        sh 'echo sudo docker rm $containerName'
      }
    }
    stage('Build Container') {
      steps {
        sh 'sudo docker-compose -f build.yml up --build'
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