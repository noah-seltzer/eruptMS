pipeline {
  agent any
  stages {
    stage('build container') {
      steps {
        sh 'ls'
        sh 'cat $propertiesPath$GIT_BRANCH'
        sh 'echo sudo docker stop $containerName'
        sh 'echo sudo docker rm $containerName'
        sh 'echo port num is $port'
        sh 'pwd'
        sh 'sudo sed -i "s/eruptTEST/$containerName/g" $WORKSPACE/build.yml'
        sh 'cat build.yml'
        sh 'sudo docker-compose -f build.yml up --build -d'
      }
    }
    stage('build containername') {
      steps {
        sh 'echo Container name is $containerName'
        sh 'echo branch name is ${GIT_BRANCH}'
        sh 'echo erupt$GIT_BRANCH'
        sh 'echo $containerName'
      }
    }
  }
  environment {
    propertiesPath = './COMP4911Timesheets/Properties/'
    containerName = sh (returnStdout: true, script: 'echo erupt$GIT_BRANCH').trim()
    port = sh (returnStdout: true, script: 'cat $propertiesPath$GIT_BRANCH').trim()
  }
}
