FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:80
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TestRepo/TestRepo.csproj", "TestRepo/"]
RUN dotnet restore "TestRepo/TestRepo.csproj"
COPY . .
WORKDIR "/src/TestRepo"
RUN dotnet build "TestRepo.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestRepo.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestRepo.dll"]