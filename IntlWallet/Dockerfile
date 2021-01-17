#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1-buster-slim AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["IntlWallet.API/IntlWallet.API.csproj", "IntlWallet.API/"]
COPY ["IntlWallet.Data/IntlWallet.Data.csproj", "IntlWallet.Data/"]
COPY ["IntlWallet.Models/IntlWallet.Models.csproj", "IntlWallet.Models/"]
COPY ["IntlWallet.Core/IntlWallet.Core.csproj", "IntlWallet.Core/"]
COPY ["IntlWallet.DTOs/IntlWallet.DTOs.csproj", "IntlWallet.DTOs/"]
COPY ["IntlWallet.Utils/IntlWallet.Utils.csproj", "IntlWallet.Utils/"]
RUN dotnet restore "IntlWallet.API/IntlWallet.API.csproj"
COPY . .
WORKDIR "/src/IntlWallet.API"
RUN dotnet build "IntlWallet.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "IntlWallet.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "IntlWallet.API.dll"]