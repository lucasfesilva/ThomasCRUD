# ThomasCRUD API

Uma API RESTful desenvolvida com **.NET 8.0**, utilizando o **Entity Framework Core**, **SQL Server 2019**, **Repository Pattern**, **DTOs** e **ASP.NET Identity** para autenticação/autorização com JWT.

## Tecnologias Utilizadas
- .NET 8.0  
- ASP.NET Core Web API  
- Entity Framework Core  
- ASP.NET Identity  
- SQL Server 2019  

## Arquitetura e Padrões
- Repository Pattern  
- DTOs (Data Transfer Objects)  
- Autenticação e Autorização com JWT  
- Swagger (Swashbuckle)

  
## Funcionalidades:

Autenticação:
- Registro de usuários (`/api/auth/register`)
- Login de usuários (`/api/auth/login`)
- Proteção de rotas com `[Authorize]`
- Emissão e validação de tokens JWT

Cliente:
- Criar cliente
- Atualizar cliente
- Remover cliente (com seus logradouros)
- Obter todos os clientes (com logradouros)
- Obter cliente por ID (com logradouros)

Logradouro:
- Criar logradouro vinculado a um cliente
- Atualizar logradouro
- Remover logradouro
- Obter logradouro por ID

## Como Executar

Pré-requisitos:
- .NET SDK 8.0+  
- SQL Server 2019 ou compatível  
- Visual Studio 2022+ ou VS Code

Criação do Banco de Dados:
1. Abra o SQL Server Management Studio.
   - Conecte-se ao seu servidor (ex: localhost\SQLEXPRESS)
   - Clique com o botão direito em Databases > New Database
   - Nomeie como ThomasCRUD e clique em OK
Ou execute o comando SQL `CREATE DATABASE ThomasCRUD;`

2. **Clone o repositório**
   
3. **Configure a string de conexão** no `appsettings.json`
```json
   "ConnectionStrings": {
     "DefaultConnection": "SUA-STRING-DE-CONEXÃO-AQUI"
   }
```

4. Abra o Visual Studio 2022 ou VS Code
  - No terminal execute o comando `dotnet ef database update`
  - Ou no Gerenciado de Pacotes NuGet execute o comando `Update-Database`
Isso irá criar as tabelas e Stored Procedures no banco de dados com base nas entidades e contexto

5. Rode a aplicação
  - Execute o comando `dotnet run` no terminal.

6. Testando a Autenticação no Swagger:
  - Acesso o endpoint `/api/Auth/register` para *criar um usuário*
  - Em seguida, vá para `/api/Auth/login` e envie o email e senha criados anteriormente para fazer o *login*
  - Copie o token JWT retornado
  - Clique no botão `Authorize` no topo do Swagger
  - Cole o token
**Agora você poderá acessar todos os endpoints protegidos**
