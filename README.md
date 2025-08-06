# Duo Rico 💰
_Gerenciando finanças, juntos._

![Badge do .NET](https://img.shields.io/badge/.NET-8-blueviolet)
![Badge do Bootstrap](https://img.shields.io/badge/Bootstrap-5-purple)

Duo Rico é uma aplicação web de finanças pessoais desenvolvida para casais. O objetivo é fornecer uma plataforma centralizada e intuitiva onde ambos os parceiros possam registrar e visualizar suas receitas e despesas, facilitando o planejamento financeiro conjunto.

Este projeto foi desenvolvido como um estudo aprofundado do ecossistema ASP.NET Core 8, aplicando conceitos modernos de desenvolvimento web, desde a arquitetura do backend até a estilização responsiva do frontend e o deploy em plataformas de nuvem.

## 🚀 Funcionalidades

- [x] **Autenticação de Usuários:** Sistema completo de Cadastro e Login usando ASP.NET Core Identity.
- [x] **Dashboard Financeiro:** Visualização rápida do saldo, total de receitas e despesas do mês selecionado.
- [x] **Filtro por Período:** Consulte as finanças de qualquer mês e ano.
- [x] **CRUD de Transações:** Crie, leia, atualize e exclua receitas e despesas de forma intuitiva.
- [x] **Lógica de Parcelamento:** Crie transações parceladas (ex: uma compra em 3x) que geram registros automáticos para os meses futuros.
- [x] **Confirmação Segura:** Modal de confirmação para ações destrutivas, como a exclusão de uma transação.
- [x] **Design Responsivo:** Interface moderna construída com Bootstrap 5 que se adapta perfeitamente a celulares, tablets e desktops.
- [x] **Localização:** Mensagens de validação e interface em Português (pt-BR).

## 🛠️ Tecnologias Utilizadas

| Categoria | Tecnologia |
| :--- | :--- |
| **Backend** | .NET 8, ASP.NET Core Razor Pages, Entity Framework Core, ASP.NET Core Identity |
| **Frontend** | Bootstrap 5, JavaScript, HTML5, CSS3 |
| **Banco de Dados** | PostgreSQL |
| **Infraestrutura & Deploy**| Docker, Render, Azure App Service, GitHub Actions |

## ⚙️ Rodando o Projeto Localmente

### Pré-requisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Uma instância de PostgreSQL (você pode usar o [Neon](https://neon.tech/) para uma base de dados gratuita na nuvem ou instalar o Postgres localmente).

### Passo a Passo

1.  **Clone o repositório:**
    ```bash
    git clone [https://github.com/seu-usuario/DuoRico.git](https://github.com/seu-usuario/DuoRico.git)
    cd DuoRico
    ```

2.  **Configure a Conexão com o Banco de Dados:**
    A aplicação usa o "User Secrets" para armazenar a string de conexão. Na raiz da solução, execute o comando:
    ```bash
    dotnet user-secrets init
    dotnet user-secrets set "ConnectionStrings:DefaultConnection" "SUA_STRING_DE_CONEXÃO_DO_POSTGRESQL_AQUI"
    ```

3.  **Aplique as Migrations:**
    Este comando criará todas as tabelas necessárias no seu banco de dados.
    ```bash
    dotnet ef database update
    ```

4.  **Execute a Aplicação:**
    ```bash
    dotnet run
    ```
    A aplicação estará disponível em `https://localhost:xxxx` e `http://localhost:xxxx`.

## ☁️ Deploy

Este projeto está configurado para deploy contínuo (CI/CD) nas seguintes plataformas:
- **Render:** Utilizando um `Dockerfile` para conteinerização e deploy automático a cada push na branch principal.
- **Azure App Service:** Utilizando GitHub Actions para build e publicação automática.
