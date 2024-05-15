dotnet build
docker build -t magnus-backend .
docker run -it --rm -p 3000:8080 magnus-backend