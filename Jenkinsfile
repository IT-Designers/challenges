pipeline {
    agent {
        label 'jenkins-agent'
    }
    stages {
        stage('Publish Docker Image "Sandbox"') {
            steps {
                sh "docker build -t test -f `pwd`/devops/testrunner/Dockerfile devops/testrunner"
            }
        }
        stage('Publish Docker Image "Challenge-Platform"') {
            steps {
                sh "cp /etc/ssl/certs/* `pwd`/devops/server/certs/ -r"
                sh "rm `pwd`/devops/server/certs/NetLock_Arany_=Class_Gold=_F??tan??s??tv??ny.pem"
                sh "docker build -t challenge-platform-server --build-arg target=Release --build-arg sonar_project_key=${env.cp_sonar_project_key} --build-arg sonar_host_url=${env.cp_sonar_host_url} --build-arg sonar_token=${env.cp_sonar_token} --build-arg dependency_track_host_url=${env.cp_dependency_track_host_url} --build-arg dependency_track_api_key=${env.cp_dependency_track_api_key} --build-arg dependency_track_project_name=${env.cp_dependency_track_project_name} --build-arg target_version=${env.GIT_BRANCH} -f `pwd`/devops/server/Dockerfile ."
            }
        }
        stage('Build agent cleanup') {
            steps {
                sh "docker system prune -f"
            }
        }
    }
}
