pipeline {
    agent any

    environment {
        PATH = "$PATH:$HOME/.dotnet/tools"
    }

    stages {
        stage('Instalar ReportGenerator') {
            steps {
                script {
                    sh "dotnet tool install -g dotnet-reportgenerator-globaltool"
                }
            }
        }
        stage('Checkout') {
            steps {
                script {
                    checkout scm
                }
            }
        }
        stage('Parar Serviços') {
            steps {
                script {
                    sh "docker-compose down"
                }
            }
        }
        stage('Construir e Subir Serviços') {
            steps {
                script {
                    sh "docker-compose up -d --build"
                }
            }
        }
        stage('Limpar Imagens Docker') {
            steps {
                script {
                    sh "docker image prune -f"
                }
            }
        }
        stage('Limpar Recursos Docker') {
            steps {
                script {
                    sh "docker network prune -f"
                    sh "docker volume prune -f"
                }
            }
        }
    }
    post {
        always {
            cleanWs() // Limpa o workspace ao final, independentemente do resultado
        }
    }
}
