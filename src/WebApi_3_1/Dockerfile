#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM swr.cn-north-1.myhuaweicloud.com/base-images/aspnetcoreruntime:3.1.15 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM swr.cn-north-1.myhuaweicloud.com/base-images/aspnetcoresdk:3.1 AS build
WORKDIR /src
COPY ["src/WebApi_3_1/WebApi_3_1.csproj", "src/WebApi_3_1/"]
RUN dotnet restore "src/WebApi_3_1/WebApi_3_1.csproj"
COPY . .
WORKDIR "/src/src/WebApi_3_1"
RUN dotnet build "WebApi_3_1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "WebApi_3_1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WebApi_3_1.dll"]