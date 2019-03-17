FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 4000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ./Apps/BinaryDiff.ApiGateway/BinaryDiff.ApiGateway.sln .
COPY ./Apps/BinaryDiff.ApiGateway/BinaryDiff.ApiGateway/BinaryDiff.ApiGateway.csproj ./BinaryDiff.ApiGateway/
RUN dotnet restore
COPY ./Apps/BinaryDiff.ApiGateway/ .
WORKDIR /src/BinaryDiff.ApiGateway
RUN dotnet build "BinaryDiff.ApiGateway.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.ApiGateway.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD ./wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.ApiGateway.dll"]