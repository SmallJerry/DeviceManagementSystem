FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

WORKDIR /src
COPY ["src/DeviceManagementSystem.Web.Host/DeviceManagementSystem.Web.Host.csproj", "src/DeviceManagementSystem.Web.Host/"]
COPY ["src/DeviceManagementSystem.Web.Core/DeviceManagementSystem.Web.Core.csproj", "src/DeviceManagementSystem.Web.Core/"]
COPY ["src/DeviceManagementSystem.Application/DeviceManagementSystem.Application.csproj", "src/DeviceManagementSystem.Application/"]
COPY ["src/DeviceManagementSystem.Core/DeviceManagementSystem.Core.csproj", "src/DeviceManagementSystem.Core/"]
COPY ["src/DeviceManagementSystem.EntityFrameworkCore/DeviceManagementSystem.EntityFrameworkCore.csproj", "src/DeviceManagementSystem.EntityFrameworkCore/"]
WORKDIR "/src/src/DeviceManagementSystem.Web.Host"
RUN dotnet restore 

WORKDIR /src
COPY ["src/DeviceManagementSystem.Web.Host", "src/DeviceManagementSystem.Web.Host"]
COPY ["src/DeviceManagementSystem.Web.Core", "src/DeviceManagementSystem.Web.Core"]
COPY ["src/DeviceManagementSystem.Application", "src/DeviceManagementSystem.Application"]
COPY ["src/DeviceManagementSystem.Core", "src/DeviceManagementSystem.Core"]
COPY ["src/DeviceManagementSystem.EntityFrameworkCore", "src/DeviceManagementSystem.EntityFrameworkCore"]
WORKDIR "/src/src/DeviceManagementSystem.Web.Host"
RUN dotnet publish -c Release -o /publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0
EXPOSE 80
WORKDIR /app
COPY --from=build /publish .
ENTRYPOINT ["dotnet", "DeviceManagementSystem.Web.Host.dll"]
