pipeline {
  agent any
    environment {
        containerName = sh (returnStdout: true, script: 'echo erupt$GIT_BRANCH').trim()
      port = """${sh (returnStdout: true, script: 'if [ "$containerName" = "TEST" ]; then
          echo 80
        elif [ "$containerName" = "STAGE" ]; then
          echo 5000
        elif [ "$containerName" = "jenkins-build" ]; then
          echo 5001
        elif [ "$containerName" = "UAT" ]; then
          echo 5002
        fi')}"""
    }
  stages {
    stage('build container') {
      steps {
        sh 'echo sudo docker stop $containerName'
        sh 'echo sudo docker rm $containerName'
        sh 'echo branch name is $port'
        sh 'ls'
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
}
