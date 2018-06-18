FROM microsoft/dotnet:2.0-sdk AS build-env
WORKDIR /app

COPY *.csproj ./
RUN dotnet restore

COPY . ./
RUN dotnet publish -c Release -o out

FROM microsoft/dotnet:2.0.5-runtime-nanoserver-1709

WORKDIR /app
COPY --from=build-env /app/out ./

CMD ["dotnet", "azureiotedge-simulated-temperature-sensor.dll"]