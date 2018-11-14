FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Web/Edison.Microservices.NotificationHubService/Edison.NotificationHubService.csproj", "Edison.Web/Edison.Microservices.NotificationHubService/"]
RUN dotnet restore "Edison.Web/Edison.Microservices.NotificationHubService/Edison.NotificationHubService.csproj"
COPY . .
WORKDIR "/src/Edison.Web/Edison.Microservices.NotificationHubService"
RUN dotnet build "Edison.NotificationHubService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.NotificationHubService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.NotificationHubService.dll"]