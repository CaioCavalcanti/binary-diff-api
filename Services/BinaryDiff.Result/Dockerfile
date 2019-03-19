FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 6000

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src

COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Domain/BinaryDiff.Shared.Domain.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.Domain/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/BinaryDiff.Shared.Infrastructure.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.Infrastructure/
COPY ./Services/BinaryDiff.Shared/BinaryDiff.Shared.WebApi/BinaryDiff.Shared.WebApi.csproj ./BinaryDiff.Shared/BinaryDiff.Shared.WebApi/

COPY ./Services/BinaryDiff.Result/BinaryDiff.Result.sln ./Result/
COPY ./Services/BinaryDiff.Result/BinaryDiff.Result.Domain/BinaryDiff.Result.Domain.csproj ./Result/BinaryDiff.Result.Domain/
COPY ./Services/BinaryDiff.Result/BinaryDiff.Result.Infrastructure/BinaryDiff.Result.Infrastructure.csproj ./Result/BinaryDiff.Result.Infrastructure/
COPY ./Services/BinaryDiff.Result/BinaryDiff.Result.WebApi/BinaryDiff.Result.WebApi.csproj ./Result/BinaryDiff.Result.WebApi/

WORKDIR /src/Result
RUN dotnet restore
COPY ./Services/BinaryDiff.Result/ .

WORKDIR /src/BinaryDiff.Shared
COPY ./Services/BinaryDiff.Shared/ .

WORKDIR /src/Result/BinaryDiff.Result.WebApi
RUN dotnet build "BinaryDiff.Result.WebApi.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "BinaryDiff.Result.WebApi.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ADD ./wait-for-it.sh .
RUN chmod +x wait-for-it.sh
ENTRYPOINT ["dotnet", "BinaryDiff.Result.WebApi.dll"]