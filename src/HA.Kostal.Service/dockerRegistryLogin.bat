REM docker build -t kostal.service -f Dockerfile ..
REM docker tag 66e05a980e9a tkregistry.azurecr.io/kostal.servive:1.0.1
docker login -u tkeller69 -p SEE_DESKTOP tkregistry.azurecr.io
REM docker tag iobroker.servive:1.0.1 tkregistry.azurecr.io/kostal.servive:1.0.1
REM docker push tkregistry.azurecr.io/kostal.servive:1.0.1
REM docker pull tkregistry.azurecr.io/kostal.servive:1.0.1