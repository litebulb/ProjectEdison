FROM microsoft/dotnet:2.1.6-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Web/Edison.Microservices.ChatService/Edison.ChatService.csproj", "Edison.Web/Edison.Microservices.ChatService/"]
RUN dotnet restore "Edison.Web/Edison.Microservices.ChatService/Edison.ChatService.csproj"
COPY . .
WORKDIR "/src/Edison.Web/Edison.Microservices.ChatService"
RUN dotnet build "Edison.ChatService.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.ChatService.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.ChatService.dll"]