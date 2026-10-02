# AuthKit.NET — Especificação Arquitetural

> Biblioteca NuGet única que injeta autenticação completa direto na aplicação do usuário, sem boilerplate.

---

## 1. Visão Geral

AuthKit.NET é um framework de autenticação e autorização integrado à aplicação do usuário. A ideia é oferecer algo poderoso como Keycloak, mas com a simplicidade de baixar um NuGet e configurar via parâmetros no `Program.cs`.

### Princípios
- **Zero boilerplate** — só `Add` + `Use` no `Program.cs`
- **DB agnóstico** — interface `IAuthRepository`, implementações para EF Core, Dapper, InMemory
- **DB compartilhado ou dedicado** — usuário decide se usa próprio `DbContext` ou cria um separado
- **Injeção direta na app** — mapeia rotas, middlewares, UI automaticamente
- **Segurança por padrão** — Argon2id, rate limiting, token rotation, MFA
- **Multi-tenancy nativo** — sem trabalho extra
- **Escalável** — suporte a Redis para rate limiting/sessions (plano futuro)
- **Testável** — InMemory provider para testes unitários
- **OpenAPI/Scalar** — docs integradas para testar endpoints

---

## 2. Estrutura de Projetos

```
AuthKit.NET/
├── src/
│   ├── AuthKit.Core/                    ← Núcleo agnóstico de DB
│   │   ├── Models/                      ← Entidades (User, Role, Tenant, Session, etc)
│   │   ├── Services/                    ← Interfaces (IUserService, ITokenService, etc)
│   │   ├── Exceptions/                  ← Exceções customizadas
│   │   └── Contracts/                   ← Contratos genéricos
│   │
│   ├── AuthKit.Identity/                ← Lógica de auth (login, registro, token, MFA)
│   │   ├── PasswordHashing/             ← Argon2id / BCrypt
│   │   ├── TokenProviders/              ← JWT + Refresh Token + Rotação
│   │   ├── Mfa/                         ← TOTP
│   │   ├── SessionManager/              ← Sessão multi-dispositivo + revogação
│   │   └── Authorization/               ← RBAC + ABAC
│   │
│   ├── AuthKit.Data/                    ← Camada de dados (separada por provedor)
│   │   ├── Data.Abstractions/           ← Interfaces IAuthRepository, IQueryExecutor
│   │   ├── Data.EntityFramework/        ← Implementação EF Core
│   │   ├── Data.Dapper/                 ← Implementação Dapper (opcional)
│   │   └── Data.InMemory/               ← Implementação para testes
│   │
│   ├── AuthKit.Security/                ← Segurança
│   │   ├── RateLimiting/                ← Rate limit por IP/account
│   │   ├── Events/                      ← Eventos de segurança (login failed, token revoked)
│   │   └── Encryption/                  ← Criptografia de sensitive data
│   │
│   ├── AuthKit.MultiTenancy/            ← Multi-tenancy
│   │   ├── TenantResolver/              ← Resolução de tenant (header, subdomain, claim)
│   │   └── TenantContext/               ← Contexto do tenant atual
│   │
│   ├── AuthKit.Integration/             ← Integrações externas
│   │   ├── OAuth/                       ← OAuth2/OIDC providers (Google, GitHub, etc)
│   │   └── Webhooks/                    ← Webhooks de eventos de auth
│   │
│   ├── AuthKit.UI/                      ← Interface de gerenciamento
│   │   ├── ScalarApiDocs/               ← Swagger/Scalar UI integrado
│   │   └── AdminDashboard/              ← Dashboard minimalista (opcional, Blazor)
│   │
│   ├── AuthKit.Configuration/           ← Configuração fluente
│   │   └── FluentApi.cs                 ← AddAuthKit(options => { ... })
│   │
│   └── AuthKit.Middleware/              ← Middlewares ASP.NET Core
│       ├── AuthKitMiddleware.cs
│       ├── TenantMiddleware.cs
│       └── RateLimitMiddleware.cs
│
├── tests/
│   ├── AuthKit.Core.Tests/
│   ├── AuthKit.Identity.Tests/
│   ├── AuthKit.Data.Tests/
│   └── AuthKit.Integration.Tests/
│
└── samples/
    ├── Sample.WebApi/                   ← WebAPI minimalista com AuthKit
    └── Sample.MultiTenant/              ← Exemplo multi-tenancy
```

---

## 3. Entidades de Dados (Tabelas)

### 3.1 Users

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `TenantId` | Guid | Chave estrangeira para Tenant |
| `Email` | string | Email único (scoped por tenant) |
| `Username` | string | Nome de usuário (opcional) |
| `PasswordHash` | string | Hash Argon2id / BCrypt |
| `EmailConfirmed` | bool | Confirmação de email |
| `CreatedAt` | DateTime | Data de criação |
| `UpdatedAt` | DateTime | Data de atualização |
| `LastLoginAt` | DateTime? | Último login |

### 3.2 Roles

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `TenantId` | Guid | Chave estrangeira para Tenant |
| `Name` | string | Nome da role (ex: "Admin") |
| `Description` | string | Descrição da role |

### 3.3 UserRoles (Many-to-Many)

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `UserId` | Guid | Chave primária composta |
| `RoleId` | Guid | Chave primária composta |
| `TenantId` | Guid | Chave estrangeira para Tenant |

### 3.4 RolePermissions

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `RoleId` | Guid | Chave primária composta |
| `Permission` | string | Permissão (ex: "users:manage", "settings:edit") |

### 3.5 UserClaims (ABAC)

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `UserId` | Guid | Chave estrangeira para User |
| `ClaimType` | string | Tipo do atributo (ex: "department", "level") |
| `ClaimValue` | string | Valor do atributo |

### 3.6 Sessions

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `UserId` | Guid | Chave estrangeira para User |
| `DeviceInfo` | string | Navegador, OS, dispositivo |
| `RefreshTokenHash` | string | Hash do refresh token (não armazenado em plaintext) |
| `RefreshTokenJti` | Guid | JWT ID do token |
| `ExpiresAt` | DateTime | Expiração da sessão |
| `CreatedAt` | DateTime | Criação da sessão |
| `IsRevoked` | bool | Revogação da sessão |

### 3.7 OAuthAccounts

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `UserId` | Guid | Chave estrangeira para User |
| `Provider` | string | Nome do provider (ex: "Google", "GitHub") |
| `ProviderId` | string | ID do usuário no provider |
| `AccessToken` | string | Criptografado |
| `RefreshToken` | string | Criptografado (opcional) |
| `ExpiresAt` | DateTime | Expiração do access token |

### 3.8 TwoFactorSecrets

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `UserId` | Guid | Chave primária e estrangeira |
| `ProviderType` | string | Tipo de 2FA ("TOTP") |
| `Secret` | string | Segredo criptografado |
| `IsEnabled` | bool | 2FA habilitado? |
| `BackupCodesHashed` | string | Códigos de backup (hash SHA256) |
| `CreatedAt` | DateTime | Criação da configuração |

### 3.9 LoginAttempts (Rate Limiting)

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `Identifier` | string | Email ou IP |
| `AttemptType` | string | "Login", "ForgotPassword", "EmailConfirm" |
| `IpAddress` | string | IP do cliente |
| `FailedAt` | DateTime | Data da tentativa |
| `Succeeded` | bool | Se foi bem-sucedida |

### 3.10 RefreshTokenRegistry (Token Rotation)

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Jti` | Guid | Chave primária (JWT ID) |
| `ParentHash` | string | Hash do refresh token pai |
| `ChildHash` | string | Hash do refresh token filho (gerado na rotação) |
| `UsedAt` | DateTime? | Quando foi usado (para detecção de reuso) |
| `RevokedAt` | DateTime? | Quando foi revogado |
| `ExpiresAt` | DateTime | Expiração do token |
| `SessionId` | Guid | Sessão associada |

### 3.11 Tenants

| Coluna | Tipo | Descrição |
|--------|------|-----------|
| `Id` | Guid | Chave primária |
| `Name` | string | Nome da organização/tenant |
| `Domain` | string | Domínio personalizado (opcional) |
| `Settings` | JSON | Configurações específicas do tenant |
| `IsActive` | bool | Tenant ativo? |

---

## 4. Endpoints (API de Auth)

### 4.1 Autenticação

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/auth/register` | Registro de novo usuário |
| `POST` | `/auth/login` | Login com email/senha |
| `POST` | `/auth/logout` | Logout + invalida sessão |
| `POST` | `/auth/refresh` | Renova access token via refresh token |
| `POST` | `/auth/forgot-password` | Solicita recuperação de senha |
| `POST` | `/auth/reset-password` | Reseta senha com token |
| `POST` | `/auth/verify-email` | Confirma email |

### 4.2 2FA / MFA

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/auth/2fa/enable` | Habilita 2FA TOTP |
| `POST` | `/auth/2fa/disable` | Desabilita 2FA |
| `POST` | `/auth/2fa/verify` | Verifica código TOTP |
| `POST` | `/auth/2fa/backups` | Gera backup codes |

### 4.3 Sessões

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/auth/sessions` | Lista sessões ativas do usuário |
| `DELETE` | `/auth/sessions/{id}` | Revoga sessão específica |
| `DELETE` | `/auth/sessions/all` | Revoga todas sessões |

### 4.4 OAuth / Social Login

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/auth/oauth/{provider}/authorize` | Redireciona para OAuth provider |
| `GET` | `/auth/oauth/{provider}/callback` | Callback OAuth |
| `POST` | `/auth/oauth/link` | Link OAuth a conta existente |

### 4.5 Admin / Gerenciamento

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/auth/docs` | Scalar UI / OpenAPI docs |
| `GET` | `/auth/users` | Listar usuários (admin) |
| `GET` | `/auth/users/{id}` | Detalhes usuário (admin) |

---

## 5. Fluxos de Segurança

### 5.1 Refresh Token Rotation

1. Client usa refresh token **A**
2. AuthKit valida, gera novo refresh token **B**
3. Token **A** é marcado como usado no `RefreshTokenRegistry`
4. Se **A** for reaproveitado após **B** → ambos revogados, usuário notificado
5. Token **B** é retornado ao cliente

### 5.2 Rate Limiting

| Endpoint | Limite | Janela |
|----------|--------|--------|
| `/login` | 5 tentativas por IP | 5 min |
| `/login` | 10 tentativas por conta | 10 min |
| `/forgot-password` | 3 tentativas por IP | 15 min |
| `/register` | 3 tentativas por IP | 15 min |

- Sliding window com contadores em memória (ou Redis em produção)
- Resposta: `429 Too Many Requests` com `Retry-After` header

### 5.3 Password Hashing

- **Argon2id** por padrão (configurável para BCrypt)
- Parâmetros: `memory=64MB`, `iterations=3`, `parallelism=1` (configuráveis)

### 5.4 Token Invalidation

- Sessões podem ser revogadas individualmente ou todas de uma vez
- Refresh tokens são rastreados no `RefreshTokenRegistry`
- `JWT blacklisting` opcional via `Jti`

---

## 6. Multi-Tenancy

### 6.1 Resolução de Tenant

1. Header `X-Tenant-Id`
2. Subdomain (`tenant.app.com`)
3. Claim `tenant_id` no JWT
4. Fallback: tenant padrão (`"default"`)

### 6.2 Isolamento

- Todas as queries filtram por `TenantId`
- Users, roles, sessions são scoped por tenant
- Configurações podem variar por tenant (ex: OAuth providers habilitados)

---

## 7. Configuração Fluent API

### 7.1 Exemplo Mínimo

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthKit(options =>
{
    options.UseDatabase(DatabaseType.EntityFrameworkCore);
    options.UseJwt("sua-chave-super-secreta", TimeSpan.FromMinutes(15));
});

var app = builder.Build();

app.UseAuthKit();
app.Run();
```

### 7.2 Configuração Completa

```csharp
builder.Services.AddAuthKit(options =>
{
    // Database - AGNÓSTICO: usa interface IAuthRepository
    options.UseDatabase(DatabaseType.EntityFrameworkCore);
    // ou .UseDapper() / .UseInMemory()
    // O usuário passa seu próprio DbContext via DI

    // JWT
    options.UseJwt("sua-chave", TimeSpan.FromMinutes(15));
    options.UseRefreshTokens(TimeSpan.FromDays(7), rotation: true);

    // Senhas
    options.PasswordHasher = HashingAlgorithm.Argon2id;

    // 2FA
    options.EnableTwoFactor<TotpProvider>();

    // Rate Limiting
    options.EnableRateLimiting();

    // Multi-Tenancy
    options.EnableMultiTenancy(tenant =>
    {
        tenant.ResolveByHeader("X-Tenant-Id");
    });

    // OAuth
    options.EnableOAuth(providers =>
    {
        providers.AddGoogle("client-id", "client-secret");
        providers.AddGitHub("client-id", "client-secret");
    });

    // UI
    options.EnableScalarUI("/auth/docs", ScalarTheme.Dark);
});
```

---

## 8. Arquitetura de Camadas

```
┌─────────────────────────────────────────────────────┐
│                 Aplicação do Usuário                  │
│                   (Program.cs)                       │
└────────────────────────┬────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│              AuthKit.Middleware                       │
│  ┌─────────────────┬─────────────────┬────────────┐ │
│  │ TenantMiddleware│ AuthKitMiddleware│ RateLimit │ │
│  └─────────────────┴─────────────────┴────────────┘ │
└────────────────────────┬────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│             AuthKit.Identity                          │
│  ┌──────────────┬──────────────┬─────────────────┐  │
│  │ LoginService │ TokenService │ Authorization   │  │
│  │ PasswordHash │ MfaService   │ RBAC / ABAC     │  │
│  └──────────────┴──────────────┴─────────────────┘  │
└────────────────────────┬────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│             AuthKit.Data                              │
│  ┌──────────────────┬──────────────┬──────────────┐ │
│  │ EfCoreRepository │ DapperRepo   │ InMemoryRepo │ │
│  └──────────────────┴──────────────┴──────────────┘ │
└────────────────────────┬────────────────────────────┘
                         │
                         ▼
┌─────────────────────────────────────────────────────┐
│                   Banco de Dados                      │
└─────────────────────────────────────────────────────┘
```

---

## 9. Dependências Externas

| Biblioteca | Uso |
|------------|-----|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer tokens |
| `System.IdentityModel.Tokens.Jwt` | JWT handling |
| `Minisign.Net` ou `Libsodium` | Argon2id hashing |
| `System.Security.Cryptography` | Nativo .NET, para BCrypt fallback |
| `Swashbuckle.AspNetCore` / `Scalar.AspNetCore` | OpenAPI / Scalar UI |
| `Dapper` (opcional) | Implementação Dapper do repositório |
| `Microsoft.EntityFrameworkCore` (opcional) | Implementação EF Core do repositório |

---

## 10. Planos Futuros

- [ ] Suporte a **Redis** para rate limiting e sessions distribuídas
- [ ] **Event Sourcing** para auditoria de auth events
- [ ] **Blazor Admin Dashboard** para gerenciar users, roles, tenants
- [ ] **GraphQL endpoint** para queries de auth
- [ ] **Internationalization** (i18n) para mensagens de erro
- [ ] **Health Checks** para monitoring
- [ ] **Telemetry / Metrics** (OpenTelemetry)

---

*Documento de referência para desenvolvimento. Versão 1.0 — Out 2025*
