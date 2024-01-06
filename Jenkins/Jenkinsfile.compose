pipeline {
    agent any

    stages {
        stage('Docker Build') {
            steps {
                sh 'docker compose -f docker-compose.yml up --build -d'
            }
        }
    }
}