FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 51412
EXPOSE 44307

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["Edison.Api/Edison.Api.csproj", "Edison.Api/"]
RUN dotnet restore "Edison.Api/Edison.Api.csproj"
COPY . .
WORKDIR "/src/Edison.Api"
RUN dotnet build "Edison.Api.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "Edison.Api.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Edison.Api.dll"]