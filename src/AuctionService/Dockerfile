#base image, the runtime
#build so it can be cached and docker wont download .net again for other services
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
#docker filesystem
WORKDIR /src
#listen on port 80 internally, it can be only used by service inside not outside
EXPOSE 80

#copy all the .csproj file and restore as distinct layers
#Use of the same COPY cmd for every dckrfile in the project to take advntg of caching

#copy the sln in host pc and paste it to the WORKDIR /src
COPY Carsties.sln Carsties.sln
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj