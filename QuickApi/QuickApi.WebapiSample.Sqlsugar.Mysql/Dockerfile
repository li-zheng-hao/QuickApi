FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["QuickApi.WebapiSample.Sqlsugar.Mysql/QuickApi.WebapiSample.Sqlsugar.Mysql.csproj", "QuickApi.WebapiSample.Sqlsugar.Mysql/"]
RUN dotnet restore "QuickApi.WebapiSample.Sqlsugar.Mysql/QuickApi.WebapiSample.Sqlsugar.Mysql.csproj"
COPY . .
WORKDIR "/src/QuickApi.WebapiSample.Sqlsugar.Mysql"
RUN dotnet build "QuickApi.WebapiSample.Sqlsugar.Mysql.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "QuickApi.WebapiSample.Sqlsugar.Mysql.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "QuickApi.WebapiSample.Sqlsugar.Mysql.dll"]
