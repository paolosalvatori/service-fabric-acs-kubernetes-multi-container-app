REM login to docker hub
docker login -u paolosalvatori -p trustno1

REM tag the local todoapi:latest image into paolosalvatori repository with latest tag
docker tag todoapi:latest paolosalvatori/todoapi:latest

REM publish paolosalvatori/todoapi:latest to paolosalvatori repository on docker hub
docker push paolosalvatori/todoapi:latest

REM tag the local todoweb:latest image into paolosalvatori repository with latest tag
docker tag todoweb:latest paolosalvatori/todoweb:latest

REM publish paolosalvatori/todoweb:latest to paolosalvatori repository on docker hub
docker push paolosalvatori/todoweb:latest

REM browse to https://hub.docker.com/r/paolosalvatori/
start chrome https://hub.docker.com/r/paolosalvatori/