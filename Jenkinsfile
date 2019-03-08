pipeline {
  agent {
    dockerfile {
      filename '/COMP4911timesheets/build.Dockerfile'
    }

  }
  stages {
    stage('build container') {
      steps {
        sh 'docker stop eruptTEST'
        sh 'docker-compose -f build.yml up --build -d'
      }
    }
  }
}