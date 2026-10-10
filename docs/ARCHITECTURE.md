# AuthKit.NET — Especificação Arquitetural

> Biblioteca NuGet única que injeta autenticação completa direto na aplicação do usuário, sem boilerplate.

---

## 1. Visão Geral

AuthKit.NET é um framework de autenticação e autorização integrado à aplicação do usuário. A ideia é oferecer algo poderoso como Keycloak, mas com a simplicidade de baixar um NuGet e configurar via parâmetros no `Program.cs`.

### Princípios
- **Zero boilerplate** — só `Add` + `Use` no `Program.cs`
- **DB agnóstico** — interface `IAuthRepository`, implementação EF Core
- **DB compartilhado ou dedicado** — usuário decide se usa próprio `DbContext` ou cria um separado
- **Injeção direta na app** — mapeia rotas, middlewares, UI automaticamente
- **Segurança por padrão** — PBKDF2/BCrypt, rate limiting, token rotation, MFA
- **Multi-tenancy nativo** — sem trabalho extra
- **Escalável** — suporte a Redis para rate limiting/sessions (plano futuro)
- **Testável** — InMemory provider para testes unitários
- **OpenAPI/Scalar** — docs integradas para testar endpoints

---

## 2. Estrutura de Projetos (Consolidada)

```
AuthKit.NET/
├── src/
│   ├── AuthKit.Core/                    ← Núcleo: Models, Options, DTOs, Interfaces base
│   │   ├── Models/                      ← 11 entidades (User, Role, Tenant, Session, etc)
│   │   ├── Options/                     ← 12 classes de configuração + enum
│   │   └── Services/                    ← Interfaces base + DTOs (record)
│   │
│   ├── AuthKit.Interfaces/              ← Todas as interfaces de serviço (contratos)
│   │   ├── Services/                    ← 11 interfaces (IPasswordHasher, ITokenService, etc.)
│   │   └── Repositories/                ← IAuthRepository, DatabaseProvider
│   │
│   ├── AuthKit.AuthKit/                 ← Configuration + Security (consolidado)
│   │   ├── Configuration/               ← AuthKitOptions, AuthKitBuilder, DI extensions
│   │   └── Services/                    ← JwtTokenService, PasswordHashService
│   │
│   ├── AuthKit.Data/
│   │   └── AuthKit.Data.EntityFramework/ ← EF Core impl (DbContext + Repository)
│   │
│   ├── AuthKit.Identity/                ← Lógica de autenticação (serviços)
│   │   └── Services/                    ← Login, Registration, Password, MFA, Session, Authorization
│   │
│   └── AuthKit.Middleware/              ← Middlewares ASP.NET Core
│       └── (pendente de implementação)
│
├── tests/
│   ├── AuthKit.Core.Tests/              ← 23 testes (models + options)
│   ├── AuthKit.AuthKit.Tests/           ← 21 testes (JWT + password hashing)
│   ├── AuthKit.Identity.Tests/          ← placeholder
│   └── AuthKit.Data.Tests/              ← placeholder
│
└── samples/
    └── Sample.WebApi/                   ← Template WebAPI (pendente de configuração)
```

**Total: 10 projetos (6 source + 4 test + 1 sample)**

### Diagrama de Dependências

```
AuthKit.Core (0 dependências) ← NÚCLEO PURO
    ↑
    ├── AuthKit.Interfaces (depends on Core)
    ├── AuthKit.AuthKit (depends on Core + Interfaces + Data.EntityFramework)
    └── AuthKit.Data.EntityFramework (depends on Core + Interfaces)
              ↑
    ┌─────────┴──────────┐
    │                    │
AuthKit.Identity     AuthKit.Middleware
(depends on Core +   (depends on Core +
 AuthKit + Interfaces) Identity + Interfaces)
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
| `PasswordHash` | string | Hash PBKDF2/HMACSHA256 ou BCrypt |
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
| `RefreshTokenHash` | string | Hash do refresh token (SHA256) |
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
| `Secret` | string | Segredo TOTP |
| `IsEnabled` | bool | 2FA habilitado? |
| `BackupCodesHashed` | string | Códigos de backup (hash Base64) |
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

## 4. Interfaces de Serviço

| Interface | Responsabilidade |
|-----------|-----------------|
| `IPasswordHasher` | Hash e verificação de senhas (PBKDF2/BCrypt) |
| `ITokenService` | Geração/validação de tokens JWT + refresh |
| `IEmailService` | Envio de emails (confirmation, reset, 2FA) |
| `ISessionService` | Gerenciamento de sessões multi-dispositivo |
| `IMfaService` | TOTP + backup codes para 2FA |
| `IAuthorizationService` | RBAC + ABAC (verificação roles, permissions, claims) |
| `ILoginService` | Login com email/senha, 2FA, logout |
| `IRegistrationService` | Registro de usuário, confirmação de email |
| `IPasswordService` | Forgot password, reset password |
| `ITenantService` | Gerenciamento de tenants |
| `IRateLimitingService` | Rate limiting por IP/account |

---

## 5. Endpoints (API de Auth)

### 5.1 Autenticação

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/auth/register` | Registro de novo usuário |
| `POST` | `/auth/login` | Login com email/senha |
| `POST` | `/auth/logout` | Logout + invalida sessão |
| `POST` | `/auth/refresh` | Renova access token via refresh token |
| `POST` | `/auth/forgot-password` | Solicita recuperação de senha |
| `POST` | `/auth/reset-password` | Reseta senha com token |
| `POST` | `/auth/verify-email` | Confirma email |

### 5.2 2FA / MFA

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `POST` | `/auth/2fa/enable` | Habilita 2FA TOTP |
| `POST` | `/auth/2fa/disable` | Desabilita 2FA |
| `POST` | `/auth/2fa/verify` | Verifica código TOTP |
| `POST` | `/auth/2fa/backups` | Gera backup codes |

### 5.3 Sessões

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/auth/sessions` | Lista sessões ativas do usuário |
| `DELETE` | `/auth/sessions/{id}` | Revoga sessão específica |
| `DELETE` | `/auth/sessions/all` | Revoga todas sessões |

### 5.4 Admin / Gerenciamento

| Método | Endpoint | Descrição |
|--------|----------|-----------|
| `GET` | `/auth/docs` | Scalar UI / OpenAPI docs |
| `GET` | `/auth/users` | Listar usuários (admin) |
| `GET` | `/auth/users/{id}` | Detalhes usuário (admin) |

---

## 6. Fluxos de Segurança

### 6.1 Refresh Token Rotation

1. Client usa refresh token **A**
2. AuthKit valida, gera novo refresh token **B**
3. Token **A** é marcado como usado no `RefreshTokenRegistry`
4. Se **A** for reaproveitado após **B** → ambos revogados, usuário notificado
5. Token **B** é retornado ao cliente

### 6.2 Rate Limiting

| Endpoint | Limite | Janela |
|----------|--------|--------|
| `/login` | 5 tentativas por IP | 5 min |
| `/login` | 10 tentativas por conta | 10 min |
| `/forgot-password` | 3 tentativas por IP | 15 min |
| `/register` | 3 tentativas por IP | 15 min |

- Sliding window com contadores em banco de dados (tabela `LoginAttempts`)
- Resposta: `429 Too Many Requests` com `Retry-After` header

### 6.3 Password Hashing

- **PBKDF2/HMACSHA256** por padrão (configurável para BCrypt)
- Parâmetros: `iterations=3` (configurável), `salt=16 bytes`, `hash=32 bytes`
- Formato: version(1) + prf(1) + iterations(4) + salt(16) + hash(32)

### 6.4 Token Invalidation

- Sessões podem ser revogadas individualmente ou todas de uma vez
- Refresh tokens são rastreados no `RefreshTokenRegistry`
- `JWT blacklisting` via `Jti`

---

## 7. Multi-Tenancy

### 7.1 Resolução de Tenant

1. Header `X-Tenant-Id`
2. Subdomain (`tenant.app.com`)
3. Claim `tenant_id` no JWT
4. Fallback: tenant padrão (`"default"`)

### 7.2 Isolamento

- Todas as queries filtram por `TenantId`
- Users, roles, sessions são scoped por tenant
- Configurações podem variar por tenant (ex: OAuth providers habilitados)

---

## 8. Configuração Fluent API

### 8.1 Exemplo Mínimo

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthKit(options =>
{
    options.SetConnectionString("Server=...;Database=...;");
    options.UseJwtBearer("sua-chave-super-secreta", TimeSpan.FromMinutes(15));
});

var app = builder.Build();

app.Run();
```

### 8.2 Configuração Completa

```csharp
builder.Services.AddAuthKit(options =>
{
    // Database
    options.UseDatabase("Server=...;Database=...;", autoMigrate: true);
    
    // JWT
    options.UseJwtBearer("sua-chave", TimeSpan.FromMinutes(15));
    options.UseRefreshTokens(TimeSpan.FromDays(7), rotation: true);
    
    // Senhas
    options.SetPasswordHashing(HashingAlgorithm.Pbkdf2, bcryptStrength: 12);
    
    // 2FA
    options.EnableTwoFactor();
    
    // Rate Limiting
    options.EnableRateLimiting();
    
    // Multi-Tenancy
    options.EnableMultiTenancy();
    
    // OAuth
    options.EnableOAuthProviders(providers =>
    {
        providers.AddGoogle("client-id", "client-secret");
        providers.AddGitHub("client-id", "client-secret");
    });
    
    // Scalar UI
    options.EnableScalarUI("/auth/docs", ScalarTheme.Dark);
});
```

---

## 9. Arquitetura de Camadas

```
┌─────────────────────────────────────────────────────┐
│                 Aplicação do Usuário                  │
│                   (Program.cs)                       │
└────────────────────────┬────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────┐
│             AuthKit.AuthKit                           │
│  ┌──────────────────┬──────────────────────────┐    │
│  │ Configuration    │ Services (JWT + Password) │    │
│  │ - AuthKitBuilder │ - JwtTokenService         │    │
│  │ - DI Extensions  │ - PasswordHashService     │    │
│  └──────────────────┴──────────────────────────┘    │
└────────────────────────┬────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────┐
│             AuthKit.Identity                          │
│  ┌──────────────┬──────────────┬─────────────────┐  │
│  │ LoginService │ Registration │ Authorization   │  │
│  │ PasswordSvc  │ MfaService   │ SessionService  │  │
│  └──────────────┴──────────────┴─────────────────┘  │
└────────────────────────┬────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────┐
│             AuthKit.Data                              │
│  ┌──────────────────────────────────────────────┐   │
│  │ EfCoreAuthRepository (IAuthRepository)       │   │
│  └──────────────────────────────────────────────┘   │
└────────────────────────┬────────────────────────────┘
                          │
                          ▼
┌─────────────────────────────────────────────────────┐
│                   Banco de Dados                      │
└─────────────────────────────────────────────────────┘
```

---

## 10. Dependências Externas

| Biblioteca | Uso |
|------------|-----|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | JWT Bearer tokens |
| `System.IdentityModel.Tokens.Jwt` | JWT handling |
| `Microsoft.AspNetCore.Cryptography.KeyDerivation` | PBKDF2 password hashing |
| `BCrypt.Net-Next` | BCrypt password hashing (fallback) |
| `Microsoft.EntityFrameworkCore` | EF Core ORM |
| `Microsoft.EntityFrameworkCore.SqlServer` | SQL Server provider |
| `Scalar.AspNetCore` | OpenAPI/Scalar UI |
| `OpenTelemetry.*` | Telemetry/Metrics (middleware) |

---

## 11. Planos Futuros

- [ ] Suporte a **Redis** para rate limiting e sessions distribuídas
- [ ] **Event Sourcing** para auditoria de auth events
- [ ] **Blazor Admin Dashboard** para gerenciar users, roles, tenants
- [ ] **GraphQL endpoint** para queries de auth
- [ ] **Internationalization** (i18n) para mensagens de erro
- [ ] **Health Checks** para monitoring
- [ ] Dapper repository (fallback mais leve)
- [ ] InMemory repository (para testes)
- [ ] OAuth providers (Google, GitHub, Microsoft, Facebook)
- [ ] Multi-tenant middleware

---

*Documento de referência para desenvolvimento. Versão 2.0 — Out 2025*
