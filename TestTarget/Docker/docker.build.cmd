REM build image
docker build -f Docker.Remote -t testtarget.local:1 .

REM build and run through docker-compose.
docker-compose -f docker-compose.yaml up  --remove-orphans --force-recreate