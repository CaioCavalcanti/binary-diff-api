FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 4000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Domain/BinaryDiff.Shared.Domain.csproj \
    ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Domain/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/BinaryDiff.Shared.Infrastructure.csproj \
    ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.WebApi/BinaryDiff.Shared.WebApi.csproj  \
    ./Services/BinaryDiff.Shared/BinaryDiff.Shared.WebApi/

COPY ./Apps/BinaryDiff.ApiGateway/BinaryDiff.ApiGateway.sln ./Apps/ApiGateway/
COPY ./Apps/BinaryDiff.ApiGateway/BinaryDiff.ApiGateway.csproj ./Apps/ApiGateway/

WORKDIR /src/Apps/ApiGateway
RUN dotnet restore
COPY ./Apps/BinaryDiff.ApiGateway/ .
COPY ./Services/BinaryDiff.Shared/ /src/Services/BinaryDiff.Shared

WORKDIR /src/Apps/ApiGateway
RUN dotnet build "BinaryDiff.ApiGateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.ApiGateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD ./wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.ApiGateway.dll"]