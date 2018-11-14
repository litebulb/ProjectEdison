FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Web/Edison.Microservices.IoTHubControllerService/Edison.IoTHubControllerService.csproj", "Edison.Web/Edison.Microservices.IoTHubControllerService/"]
RUN dotnet restore "Edison.Web/Edison.Microservices.IoTHubControllerService/Edison.IoTHubControllerService.csproj"
COPY . .
WORKDIR "/src/Edison.Web/Edison.Microservices.IoTHubControllerService"
RUN dotnet build "Edison.IoTHubControllerService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.IoTHubControllerService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.IoTHubControllerService.dll"]