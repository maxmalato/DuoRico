# Imagem base para tempo de execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Fase de build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DuoRico/DuoRico.csproj", "DuoRico/"]
RUN dotnet restore "DuoRico/DuoRico.csproj"
COPY . .
WORKDIR "/src/DuoRico"
RUN dotnet build "DuoRico.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Fase de publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "DuoRico.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Imagem final para execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DuoRico.dll"]

# Definição do idioma pt-br
RUN apt-get update && apt-get install -y locales
RUN sed -i -e 's/# pt_BR.UTF-8 UTF-8/pt_BR.UTF-8 UTF-8/' /etc/locale.gen && \
    locale-gen
ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR:pt
ENV LC_ALL pt_BR.UTF-8