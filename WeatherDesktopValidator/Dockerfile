FROM mcr.microsoft.com/dotnet/aspnet:6.0 as base
WORKDIR /app
EXPOSE 80



FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["WeatherDesktopValidator/WeatherDesktopValidator.csproj", "WeatherDesktopValidator/"]
RUN dotnet restore "WeatherDesktopValidator/WeatherDesktopValidator.csproj"
COPY . .
WORKDIR "/src/WeatherDesktopValidator"
RUN dotnet build "WeatherDesktopValidator.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "WeatherDesktopValidator.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "WeatherDesktopValidator.dll"]