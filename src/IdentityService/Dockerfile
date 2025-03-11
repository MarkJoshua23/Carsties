#base image, the runtime
#build is the image id so it can be cached and docker wont download .net again for other services
FROM mcr.microsoft.com/dotnet/sdk:8.0 as build
#docker filesystem
WORKDIR /app
#listen on port 80 internally, it can be only used by service inside not outside
EXPOSE 80


#copy the sln in host pc and paste it to the WORKDIR /app
COPY Carsties.sln Carsties.sln

#copy all the .csproj file and restore as distinct layers
#Use of the same COPY cmd for every dckrfile in the project to take advntg of caching

#copy the csproj in host pc and paste it to the WORKDIR /app
COPY src/AuctionService/AuctionService.csproj src/AuctionService/AuctionService.csproj
COPY src/SearchService/SearchService.csproj src/SearchService/SearchService.csproj
COPY src/GatewayService/GatewayService.csproj src/GatewayService/GatewayService.csproj
COPY src/BiddingService/BiddingService.csproj src/BiddingService/BiddingService.csproj
COPY src/Contracts/Contracts.csproj src/Contracts/Contracts.csproj
COPY src/IdentityService/IdentityService.csproj src/IdentityService/IdentityService.csproj

#restore package dependecies
RUN dotnet restore Carsties.sln

#copy auction folders to docker
COPY src/IdentityService src/IdentityService

#after doing everything in the root, switch directory like doing cd on linux to do something at that directory
WORKDIR /app/src/IdentityService

#publish to dll
RUN dotnet publish -c Release -o /app/src/out

#build runtime image, here we dont need the dotnet sdk since app is already published we just need the runtime 
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

#copy the out files from previous image to new image dir app/
COPY --from=build /app/src/out .
ENTRYPOINT [ "dotnet","IdentityService.dll" ]