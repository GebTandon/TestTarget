FROM mcr.microsoft.com/dotnet/core/aspnet:3.1 AS base
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build
WORKDIR /src
COPY ["TestTarget/TestTarget.csproj", "TestTarget/"]

RUN dotnet restore "TestTarget/TestTarget.csproj"
COPY . .
WORKDIR "/src/TestTarget"
RUN dotnet build "TestTarget.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TestTarget.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TestTarget.dll"]