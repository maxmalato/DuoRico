# Estágio 1: Build da Aplicação
# Usa a imagem completa do SDK do .NET 8 para compilar o projeto.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia os arquivos de projeto/solução e restaura as dependências.
COPY ["DuoRico.sln", "./"]
COPY ["DuoRico/DuoRico.csproj", "DuoRico/"]
RUN dotnet restore "./DuoRico.sln"

# Copia todo o código fonte.
COPY . .
WORKDIR "/src/DuoRico"

# Publica a aplicação em modo Release.
RUN dotnet publish "DuoRico.csproj" -c Release -o /app/publish

# Estágio 2: Imagem Final para Execução
# Usa a imagem leve do ASP.NET Runtime.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# --- INÍCIO DA CONFIGURAÇÃO DE IDIOMA (LOCAL CORRETO) ---
# Instala e configura o idioma Português (pt-BR) no container Linux.
# Isso garante que o .NET consiga lidar com acentos corretamente.
RUN apt-get update && apt-get install -y locales && rm -rf /var/lib/apt/lists/*
RUN sed -i -e 's/# pt_BR.UTF-8 UTF-8/pt_BR.UTF-8 UTF-8/' /etc/locale.gen && \
    locale-gen
ENV LANG pt_BR.UTF-8
ENV LANGUAGE pt_BR:pt
ENV LC_ALL pt_BR.UTF-8
# --- FIM DA CONFIGURAÇÃO DE IDIOMA ---

WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "DuoRico.dll"]