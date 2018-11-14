FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Web/Edison.Workflows/Edison.Workflows.csproj", "Edison.Web/Edison.Workflows/"]
RUN dotnet restore "Edison.Web/Edison.Workflows/Edison.Workflows.csproj"
COPY . .
WORKDIR "/src/Edison.Web/Edison.Workflows"
RUN dotnet build "Edison.Workflows.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.Workflows.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.Workflows.dll"]