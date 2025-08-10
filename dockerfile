FROM mcr.microsoft.com/dotnet/sdk:8.0 AS base
WORKDIR /app
RUN dotnet tool install --global dotnet-ef --version 8.*
ENV PATH="$PATH:/root/.dotnet/tools"
COPY . .

FROM base AS build
ARG BUILD_CONFIGURATION="Release"
ARG CSPROJ_PATH="./Es2al/Es2al.csproj"
RUN dotnet restore $CSPROJ_PATH
RUN dotnet publish -c $BUILD_CONFIGURATION $CSPROJ_PATH -o publish
RUN dotnet-ef migrations bundle --self-contained -r linux-x64 --project Es2al.DataAccess --startup-project Es2al -o ./publish/efbundle

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS publish
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT [ "/bin/sh","-c" ]
CMD ["./efbundle;dotnet Es2al.dll"]