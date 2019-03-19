pipeline {
  agent {
    dockerfile {
      filename 'COMP4911Timesheets/build.Dockerfile'
    }

  }
  stages {
    stage('docker ps') {
      steps {
        sh 'docker ps'
      }
    }
  }
}