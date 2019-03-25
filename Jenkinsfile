pipeline {
  agent any
  stages {
    stage('build container') {
      environment {
        containerName = sh (returnStdout: true, script: 'echo erupt$GIT_BRANCH').trim()
      }
      steps {
        sh 'echo sudo docker stop $containerName'
        sh 'echo sudo docker rm $containerName'
        sh 'sh \'sed -i \'s/eruptTest/$containerName/g\' ./build.yml\''
        sh 'sudo docker-compose -f build.yml up --build -d'
        readFile(file: 'build.yml', encoding: 'utf-8')
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