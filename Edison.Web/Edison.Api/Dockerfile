FROM microsoft/dotnet:2.1.5-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80
EXPOSE 53145

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Web/Edison.Api/Edison.Api.csproj", "Edison.Web/Edison.Api/"]
RUN dotnet restore "Edison.Web/Edison.Api/Edison.Api.csproj"
COPY . .
WORKDIR "/src/Edison.Web/Edison.Api"
RUN dotnet build "Edison.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.Api.dll"]