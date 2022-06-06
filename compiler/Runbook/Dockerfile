FROM mcr.microsoft.com/dotnet/runtime:6.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ["runbook/runbook.csproj", "runbook/"]
RUN dotnet restore "runbook/runbook.csproj"
COPY . .
WORKDIR "/src/runbook"
RUN dotnet build "runbook.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "runbook.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "runbook.dll"]
