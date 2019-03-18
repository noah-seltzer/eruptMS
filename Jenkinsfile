pipeline {
  agent any
  stages {
    stage('build container') {
      steps {
        sh 'docker-compose -f build.yml up --build -d'
        sh 'docker stop eruptTEST'
      }
    }
  }
}