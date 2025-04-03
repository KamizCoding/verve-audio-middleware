# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy project files
COPY *.sln ./
COPY VerveCloudAudioMiddleware/*.csproj ./VerveCloudAudioMiddleware/
RUN dotnet restore

# Copy the rest of the code and build
COPY VerveCloudAudioMiddleware/. ./VerveCloudAudioMiddleware/
WORKDIR /app/VerveCloudAudioMiddleware
RUN dotnet publish -c Release -o out

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/VerveCloudAudioMiddleware/out ./

# Set environment variables (OPTIONAL: also set them on Render)
ENV ASPNETCORE_URLS=http://+:8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "VerveCloudAudioMiddleware.dll"]
