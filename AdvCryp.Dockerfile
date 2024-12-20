FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# based on eshops on container
# https://github.com/dotnet-architecture/eShopOnContainers/blob/dev/src/Services/Basket/Basket.API/Dockerfile
# will not bother with cache yet

COPY . .

WORKDIR /src/AdvCrypExam
RUN dotnet publish -c Release -o /app

FROM build AS publish

# the final
FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "AdvCrypExam.dll", "--urls=http://0.0.0.0:80", "--contentRoot=."]
