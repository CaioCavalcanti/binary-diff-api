FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY ./BinaryDiff.Input.sln .
COPY ./BinaryDiff.Input.Domain/BinaryDiff.Input.Domain.csproj ./BinaryDiff.Input.Domain/
COPY ./BinaryDiff.Input.Infrastructure/BinaryDiff.Input.Infrastructure.csproj ./BinaryDiff.Input.Infrastructure/
COPY ./BinaryDiff.Input.WebApi/BinaryDiff.Input.WebApi.csproj ./BinaryDiff.Input.WebApi/
RUN dotnet restore
COPY . .
WORKDIR /src/BinaryDiff.Input.WebApi
RUN dotnet build "BinaryDiff.Input.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.Input.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD https://raw.githubusercontent.com/vishnubob/wait-for-it/master/wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.Input.WebApi.dll"]