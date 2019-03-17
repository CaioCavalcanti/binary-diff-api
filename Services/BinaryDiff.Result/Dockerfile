FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:6000
EXPOSE 6000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ./BinaryDiff.Result.sln .
COPY ./BinaryDiff.Result.Domain/BinaryDiff.Result.Domain.csproj ./BinaryDiff.Result.Domain/
COPY ./BinaryDiff.Result.Infrastructure/BinaryDiff.Result.Infrastructure.csproj ./BinaryDiff.Result.Infrastructure/
COPY ./BinaryDiff.Result.WebApi/BinaryDiff.Result.WebApi.csproj ./BinaryDiff.Result.WebApi/
RUN dotnet restore
COPY . .
WORKDIR /src/BinaryDiff.Result.WebApi
RUN dotnet build "BinaryDiff.Result.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.Result.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.Result.WebApi.dll"]