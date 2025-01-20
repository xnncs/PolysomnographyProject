FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 3000

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["PolysomnographyProject/PolysomnographyProject.csproj", "PolysomnographyProject/"]
RUN dotnet restore "PolysomnographyProject/PolysomnographyProject.csproj"
COPY . . 
WORKDIR "/src/PolysomnographyProject"
RUN dotnet build "PolysomnographyProject.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "PolysomnographyProject.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

RUN mkdir -p /app/Public/Books
COPY ./Public/Books/113956570.a4.pdf /app/Public/Books/113956570.a4.pdf


# Set the entrypoint for the application
ENTRYPOINT ["dotnet", "PolysomnographyProject.dll"]