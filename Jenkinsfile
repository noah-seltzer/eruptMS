pipeline {
  agent any
  stages {
    stage('build container') {
      steps {
        sh 'sudo docker stop eruptTEST'
        sh 'sudo docker rm eruptTEST'
        sh 'sudo docker-compose -f build.yml up --build -d'
      }
    }
    stage('build containername') {
      steps {
        sh 'echo Container name is $containerName'
        sh 'echo branch name is $env.GIT_BRANCH'
        sh 'containerName=\'erupt$env.GIT_BRANCH\''
        sh 'echo $containerName'
      }
    }
  }
  environment {
    containerName = 'erupt'
  }
}