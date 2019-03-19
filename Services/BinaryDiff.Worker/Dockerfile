FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Domain/BinaryDiff.Shared.Domain.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.Domain/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/BinaryDiff.Shared.Infrastructure.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.WebApi/BinaryDiff.Shared.WebApi.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.WebApi/

COPY ./Services/BinaryDiff.Worker/BinaryDiff.Worker.sln ./Worker/
COPY ./Services/BinaryDiff.Worker/BinaryDiff.Worker.Domain/BinaryDiff.Worker.Domain.csproj ./Worker/BinaryDiff.Worker.Domain/
COPY ./Services/BinaryDiff.Worker/BinaryDiff.Worker.Infrastructure/BinaryDiff.Worker.Infrastructure.csproj ./Worker/BinaryDiff.Worker.Infrastructure/
COPY ./Services/BinaryDiff.Worker/BinaryDiff.Worker.App/BinaryDiff.Worker.App.csproj ./Worker/BinaryDiff.Worker.App/

WORKDIR /src/Worker
RUN dotnet restore
COPY ./Services/BinaryDiff.Worker/ .

WORKDIR /src/BinaryDiff.Shared
COPY ./Services/BinaryDiff.Shared/ .

WORKDIR /src/Worker/BinaryDiff.Worker.App
RUN dotnet build "BinaryDiff.Worker.App.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.Worker.App.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD ./wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.Worker.App.dll"]