FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 5000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Domain/BinaryDiff.Shared.Domain.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.Domain/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/BinaryDiff.Shared.Infrastructure.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.WebApi/BinaryDiff.Shared.WebApi.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.WebApi/

COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.sln ./Input/
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.Domain/BinaryDiff.Input.Domain.csproj ./Input/BinaryDiff.Input.Domain/
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.Infrastructure/BinaryDiff.Input.Infrastructure.csproj ./Input/BinaryDiff.Input.Infrastructure/
COPY ./Services/BinaryDiff.Input/BinaryDiff.Input.WebApi/BinaryDiff.Input.WebApi.csproj ./Input/BinaryDiff.Input.WebApi/

WORKDIR /src/Input
RUN dotnet restore
COPY ./Services/BinaryDiff.Input/ .

WORKDIR /src/BinaryDiff.Shared
COPY ./Services/BinaryDiff.Shared/ .

WORKDIR /src/Input/BinaryDiff.Input.WebApi
RUN dotnet build "BinaryDiff.Input.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.Input.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD ./wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.Input.WebApi.dll"]