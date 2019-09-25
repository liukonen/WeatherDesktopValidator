FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
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