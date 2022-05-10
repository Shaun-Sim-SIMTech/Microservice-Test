#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 5000
ENV RABBITMQ_HOST localhost
ENV RABBITMQ_PORT 32001 



FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["TestProducer1.csproj", "."]
RUN dotnet restore "./TestProducer1.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "TestProducer1.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestProducer1.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestProducer1.dll"]