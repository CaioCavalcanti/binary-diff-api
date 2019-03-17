FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.sln .
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.Domain/BinaryDiff.Input.Domain.csproj ./BinaryDiff.Input.Domain/
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.Infrastructure/BinaryDiff.Input.Infrastructure.csproj ./BinaryDiff.Input.Infrastructure/
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.WebApi/BinaryDiff.Input.WebApi.csproj ./BinaryDiff.Input.WebApi/
RUN dotnet restore
COPY ./Services/BinaryDiff.Input/ .
WORKDIR /src/BinaryDiff.Input.WebApi
RUN dotnet build "BinaryDiff.Input.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.Input.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD ./wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.Input.WebApi.dll"]