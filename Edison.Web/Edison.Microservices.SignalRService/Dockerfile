FROM microsoft/dotnet:2.1.6-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Web/Edison.Microservices.SignalRService/Edison.SignalRService.csproj", "Edison.Web/Edison.Microservices.SignalRService/"]
RUN dotnet restore "Edison.Web/Edison.Microservices.SignalRService/Edison.SignalRService.csproj"
COPY . .
WORKDIR "/src/Edison.Web/Edison.Microservices.SignalRService"
RUN dotnet build "Edison.SignalRService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.SignalRService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.SignalRService.dll"]