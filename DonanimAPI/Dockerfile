FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DonanimAPI/DonanimAPI.csproj", "DonanimAPI/"]
RUN dotnet restore "DonanimAPI/DonanimAPI.csproj"
COPY . .
WORKDIR "/src/DonanimAPI"
RUN dotnet build "DonanimAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "DonanimAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY DonanimAPI/.env .
ENTRYPOINT ["dotnet", "DonanimAPI.dll"] 