#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

ARG DB_HOST
ARG DB_NAME
ARG DB_USER
ARG DB_PASS

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["X-ZIGZAG SERVER WEB API/X-ZIGZAG SERVER WEB API.csproj", "X-ZIGZAG SERVER WEB API/"]
RUN dotnet restore "./X-ZIGZAG SERVER WEB API/X-ZIGZAG SERVER WEB API.csproj"
COPY . .
WORKDIR "/src/X-ZIGZAG SERVER WEB API"
RUN dotnet build "./X-ZIGZAG SERVER WEB API.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./X-ZIGZAG SERVER WEB API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN sed -i "s/{{DB_HOST}}/$DB_HOST/g" appsettings.json && \
    sed -i "s/{{DB_USER}}/$DB_USER/g" appsettings.json && \
    sed -i "s/{{DB_NAME}}/$DB_NAME/g" appsettings.json && \
    sed -i "s/{{DB_PASS}}/$DB_PASS/g" appsettings.json
ENTRYPOINT ["dotnet", "X-ZIGZAG SERVER WEB API.dll"]