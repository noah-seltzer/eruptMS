pipeline {
  agent any
  stages {
    stage('build container') {
      steps {
        sh 'docker stop eruptTEST'
        sh 'docker-compose -f build.yml up --build -d'
      }
    }
  }
}