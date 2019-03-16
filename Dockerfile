FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ["BinaryDiff.sln", "./"]
COPY ["BinaryDiff.API/BinaryDiff.API.csproj", "BinaryDiff.API/"]
COPY ["BinaryDiff.Domain/BinaryDiff.Domain.csproj", "BinaryDiff.Domain/"]
COPY ["BinaryDiff.Infrastructure/BinaryDiff.Infrastructure.csproj", "BinaryDiff.Infrastructure/"]
COPY ["BinaryDiff.Tests/BinaryDiff.Tests.csproj", "BinaryDiff.Tests/"]
RUN dotnet restore
COPY . .
RUN dotnet test --no-restore -l:"console;verbosity=minimal" /p:CollectCoverage=true /p:Exclude="[xunit*]*"
WORKDIR "/src/BinaryDiff.API"
RUN dotnet build "BinaryDiff.API.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.API.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "BinaryDiff.API.dll"]