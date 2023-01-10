#!/usr/bin/env sh

sdk_image_stage="base_sdk"
runtime_image_stage="base_runtime"

all_csprojs="$(find ./ -type f -name '*.csproj')"
migration_csproj="$(find ./src -type f -name '*Migrations.csproj')"
src_entry="./src/$(ls ./src | grep '\.Startup$' | head -n 1)"
ddl_entry="$(ls $src_entry | grep -E '.+\.csproj' | sed 's#\.csproj#\.dll#')"

build_stage="base_build"
publish_stage="publish"
migrationbuilder_stage="migrationbuilder"
migrationrunner_stage="migrationrunner"
testrunner_stage="testrunner"
runtime_stage="runtime"


emit_copy() {
    for item in $(echo "${1}"); do
        echo "COPY ${item} ${item}"
    done
}

emit_copy_dirs() {
    for item in $(echo "${1}"); do
        local dir=$(dirname "$item")
        echo "COPY ${dir}/ ${dir}/"
    done
}


cat <<EOF > Dockerfile
# =======================================
# BASE
# =======================================
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS ${sdk_image_stage}
FROM mcr.microsoft.com/dotnet/aspnet:7.0 as ${runtime_image_stage}
EOF


cat <<EOF >> Dockerfile

# =======================================
# BUILD
# =======================================
# Essa etapa faz o "restore" e "build" de todos os projetos na solução.
# Isso faz com que alteração em qualquer arquivo fonte invalide o cache de todas
# as camadas (menos a de restore, cujo cache depende apenas dos .csproj).
#
# ATENÇÃO: O tamanho das imagens finais pode acabar muito maior do que o necessário
# por conta do tamanho da camada do "restore" que vai ter pacotes para a solução
# inteira e algumas imagens podem não precisar de todos esses pacotes.
FROM ${sdk_image_stage} AS ${build_stage}
WORKDIR /app

COPY ./*.sln ./
COPY ./Directory.Build.props ./

$(emit_copy "${all_csprojs}")

ARG nuget_url
# Não tem problema deixar o "source" mostrando o token já que as imagens finais
# não vão conter os arquivos do nuget. Caso isso mude vamos precisar lidar com isso.
RUN --mount=type=secret,id=nuget_token \\
    --mount=type=secret,id=nuget_user \\
    dotnet nuget add source \\
    --username "\$(cat /run/secrets/nuget_user)" \\
    --password "\$(cat /run/secrets/nuget_token)" \\
    --store-password-in-clear-text \\
    --name local \\
    "\${nuget_url}"
RUN dotnet restore --packages /app/restored_packages

$(emit_copy_dirs "${all_csprojs}")
RUN dotnet build --configuration=Release --no-restore --nologo
EOF


cat <<EOF >> Dockerfile

# =======================================
# MIGRATION BUILDER
# =======================================
FROM ${build_stage} AS ${migrationbuilder_stage}
WORKDIR /app

RUN dotnet tool install --verbosity quiet --global dotnet-ef
ENV PATH="\$PATH:/root/.dotnet/tools"

RUN mkdir /bundle
RUN dotnet ef migrations bundle --configuration=Release --project=${migration_csproj} --output=/bundle/migrate
EOF


cat <<EOF >> Dockerfile

# =======================================
# MIGRATION RUNNER
# =======================================
FROM ${runtime_image_stage} AS ${migrationrunner_stage}
WORKDIR /app

COPY --from=${migrationbuilder_stage} /bundle/. ./

ENTRYPOINT ["/app/migrate"]
EOF


cat <<EOF >> Dockerfile

# =======================================
# TEST RUNNER
# =======================================
# IMPORTANTE: Não usar a etapa de build como base! Se fizer isso as camadas com
# segredos vão sair na imagem final.
FROM ${sdk_image_stage} AS ${testrunner_stage}
WORKDIR /app

COPY --from=${build_stage} /app/ ./

ENTRYPOINT ["dotnet", "test", "--configuration=Release", "--no-build", "--nologo"]
EOF


cat <<EOF >> Dockerfile

# =======================================
# PUBLISH
# =======================================
FROM ${build_stage} AS ${publish_stage}
WORKDIR /app

RUN dotnet publish --configuration=Release --no-build --nologo --output=build ${src_entry}
EOF


cat <<EOF >> Dockerfile

# =======================================
# RUNTIME
# =======================================
FROM ${runtime_image_stage} AS ${runtime_stage}
WORKDIR /app

COPY --from=${publish_stage} /app/build ./

ENTRYPOINT ["dotnet", "${ddl_entry}"]
EOF
