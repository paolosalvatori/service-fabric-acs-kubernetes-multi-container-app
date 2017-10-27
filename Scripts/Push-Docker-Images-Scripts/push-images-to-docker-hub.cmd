REM login to docker hub
docker login -u DOCKER_HUB_REPOSITORY -p trustno1

REM tag the local todoapi:latest image with the name of the DOCKER_HUB_REPOSITORY
docker tag todoapi:latest DOCKER_HUB_REPOSITORY/todoapi:latest

REM push the image DOCKER_HUB_REPOSITORY/todoapi:latest to the DOCKER_HUB_REPOSITORY
docker push DOCKER_HUB_REPOSITORY/todoapi:latest

REM tag the local todoweb:latest image with the name of the DOCKER_HUB_REPOSITORY
docker tag todoweb:latest DOCKER_HUB_REPOSITORY/todoweb:latest

REM push the image DOCKER_HUB_REPOSITORY/todoweb:latest to the DOCKER_HUB_REPOSITORY 
docker push DOCKER_HUB_REPOSITORY/todoweb:latest

REM browse to https://hub.docker.com/r/DOCKER_HUB_REPOSITORY/
start chrome https://hub.docker.com/r/DOCKER_HUB_REPOSITORY/