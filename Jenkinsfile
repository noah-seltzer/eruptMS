pipeline {
  agent any
  stages {
    stage('build container') {
      steps {
        sh 'sudo docker stop eruptTEST'
        sh 'sudo docker-compose -f build.yml up --build -d'
      }
    }
  }
}