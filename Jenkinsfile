pipeline {
  agent {
    dockerfile {
      dir 'COMP4911Timesheets'
      filename 'build.Dockerfile'
      label 'jenktest'
    }

  }
  stages {
    stage('build container') {
      steps {
        sh 'pwd'
      }
    }
  }
}
