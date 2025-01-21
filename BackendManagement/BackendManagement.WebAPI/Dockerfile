FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["BackendManagement.WebAPI/BackendManagement.WebAPI.csproj", "BackendManagement.WebAPI/"]
COPY ["BackendManagement.Application/BackendManagement.Application.csproj", "BackendManagement.Application/"]
COPY ["BackendManagement.Domain/BackendManagement.Domain.csproj", "BackendManagement.Domain/"]
COPY ["BackendManagement.Infrastructure/BackendManagement.Infrastructure.csproj", "BackendManagement.Infrastructure/"]
RUN dotnet restore "BackendManagement.WebAPI/BackendManagement.WebAPI.csproj"
COPY . .
WORKDIR "/src/BackendManagement.WebAPI"
RUN dotnet build "BackendManagement.WebAPI.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BackendManagement.WebAPI.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "BackendManagement.WebAPI.dll"] 