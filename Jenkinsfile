pipeline {
  agent any
  stages {
    stage('build container') {
      steps {
        sh 'echo sudo docker stop eruptTEST'
        sh 'echo sudo docker rm eruptTEST'
        sh 'sudo docker-compose -f build.yml up --build -d'
      }
    }
    stage('name container') {
      steps {
        echo 'this is branch '+env.GIT_BRANCH
        echo '${\'Container name is\' + $containerName}'
        sh '$containerName = \'erupt\' + env.GIT_BRANCH'
        echo '${\'Container name is\' + $containerName}'
      }
    }
  }
  environment {
    containerName = 'erupt'
  }
}