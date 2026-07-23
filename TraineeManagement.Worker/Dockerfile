FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/sdk:10.0-alpine AS build-env

ARG DOMAIN_OWNER
ARG NUGET_REPO_NAME
ARG CODEARTIFACT_AUTH_TOKEN
ARG REGION

WORKDIR /AppCode

RUN cat > nuget.config <<EOF
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <clear />
    <add key="${NUGET_REPO_NAME}" value="https://zeuslearning-${DOMAIN_OWNER}.d.codeartifact.${REGION}.amazonaws.com/nuget/${NUGET_REPO_NAME}/v3/index.json" />
  </packageSources>
</configuration>
EOF

COPY ./TraineeManagement.Worker/ ./TraineeManagement.Worker/
COPY ./TraineeManagement.Data/ ./TraineeManagement.Data/

WORKDIR /AppCode/TraineeManagement.Worker

RUN dotnet nuget update source ${NUGET_REPO_NAME} \
    -u aws \
    -p "${CODEARTIFACT_AUTH_TOKEN}" \
    --store-password-in-clear-text \
    --configfile /AppCode/nuget.config

RUN dotnet restore --configfile /AppCode/nuget.config
RUN dotnet publish -c Release -o out --no-restore

FROM docker-registry-002.zeuslearning.com/zeuslearning/dotnet/aspnet:10.0-alpine AS runtime

WORKDIR /AppCode/TraineeManagement.Worker

COPY --from=build-env /AppCode/TraineeManagement.Worker/out .

EXPOSE 8080

ENTRYPOINT ["dotnet", "TraineeManagement.Worker.dll"]



















