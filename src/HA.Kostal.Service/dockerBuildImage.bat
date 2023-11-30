dotnet publish -c Release
docker build -t kostal.service -f Dockerfile ..
docker image ls
REM docker tag 66e05a980e9a tkregistry.azurecr.io/kostal.servive:1.0.1
