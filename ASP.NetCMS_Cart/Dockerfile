#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["ASP.NetCMS_Cart/ASP.NetCMS_Cart.csproj", "ASP.NetCMS_Cart/"]
RUN dotnet restore "ASP.NetCMS_Cart/ASP.NetCMS_Cart.csproj"
COPY . .
WORKDIR "/src/ASP.NetCMS_Cart"
RUN dotnet build "ASP.NetCMS_Cart.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "ASP.NetCMS_Cart.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ASP.NetCMS_Cart.dll"]