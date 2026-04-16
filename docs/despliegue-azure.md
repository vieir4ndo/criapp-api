# Despliegue de CRIApp API en Azure

## Visión general

Este documento describe el proceso completo de despliegue de la API REST de CRIApp en la nube, utilizando una arquitectura sin coste económico apta para entornos de demostración y TFM.

---

## Arquitectura final

```
GitHub (código fuente)
    │
    └── GitHub Actions (CI/CD)
            ├── Build & Test (.NET 8)
            ├── Migrations (dotnet ef → Neon PostgreSQL)
            └── Deploy (Azure App Service F1)
                        │
                        └── Neon.tech (PostgreSQL serverless)
```

---

## Tecnologías y justificación de decisiones

### 1. Azure App Service (Plan F1 — gratuito)

**Qué es:** Plataforma como servicio (PaaS) de Microsoft Azure para alojar aplicaciones web.

**Por qué se eligió:**
- Tier F1 completamente gratuito (sin límite de tiempo).
- Soporte nativo para .NET 8 en Linux.
- Integración directa con GitHub Actions mediante OIDC (sin necesidad de gestionar credenciales manuales).
- Escalabilidad futura: migrar a B1/S1 sin cambios en el código ni en la pipeline.

**Limitaciones del tier F1:**
- Sin dominio personalizado con SSL.
- Sin slots de staging.
- CPU y memoria compartidas (suficiente para demo/TFM).

---

### 2. Neon.tech (PostgreSQL serverless — gratuito)

**Qué es:** Servicio de PostgreSQL serverless con tier permanentemente gratuito.

**Por qué se eligió:**
- Azure Database for PostgreSQL no dispone de tier gratuito (~12€/mes el más económico).
- Neon ofrece 0,5 GB de almacenamiento y conexiones ilimitadas sin coste.
- Compatible con Npgsql y Entity Framework Core sin cambios en el código.
- Soporte nativo de SSL (`sslmode=require`), lo que cumple requisitos de seguridad básicos.

**Configuración de conexión:**
La connection string debe estar en formato key-value para Npgsql:
```
Host=<host>;Port=5432;Database=<db>;Username=<user>;Password=<password>;SSL Mode=Require;Trust Server Certificate=true
```

> **Nota:** El formato URI de PostgreSQL (`postgresql://user:pass@host/db`) no es compatible con Npgsql en migraciones EF Core.

---

### 3. GitHub Actions (CI/CD — gratuito)

**Qué es:** Plataforma de integración y entrega continua integrada en GitHub.

**Por qué se eligió sobre Azure Pipelines:**
- Azure Pipelines requiere aprobación manual de Microsoft para ejecutar pipelines en proyectos privados (el paralelismo gratuito fue eliminado en 2021).
- GitHub Actions ofrece 2.000 minutos/mes gratuitos para repositorios privados.
- Integración nativa con el repositorio GitHub existente.

**Autenticación con Azure:**
Se utiliza autenticación OIDC (OpenID Connect) mediante un Service Principal federado, evitando el almacenamiento de credenciales estáticas. Los secrets utilizados son:
- `AZUREAPPSERVICE_CLIENTID_*`
- `AZUREAPPSERVICE_TENANTID_*`
- `AZUREAPPSERVICE_SUBSCRIPTIONID_*`

---

### 4. Entity Framework Core 8 (migraciones automáticas)

Las migraciones de base de datos se ejecutan como paso explícito en la pipeline **antes** del despliegue, garantizando que el esquema esté actualizado antes de que la nueva versión de la app arranque.

```
Build → Test → Migrate → Deploy
```

Esto evita errores de startup por incompatibilidad entre el esquema y el código desplegado.

---

### 5. Health Checks

Se implementaron dos endpoints de monitorización:

| Endpoint | Descripción |
|---|---|
| `GET /health` | Estado completo: base de datos, email (SMTP) y SMS (Twilio) |
| `GET /health/self` | Estado de la aplicación sin dependencias externas |

**Respuesta de `/health`:**
```json
{
  "status": "Healthy",
  "duration": "00:00:01.234",
  "checks": {
    "database": { "status": "Healthy", "duration": "00:00:00.123" },
    "email":    { "status": "Healthy", "duration": "00:00:00.456" },
    "sms":      { "status": "Healthy", "duration": "00:00:00.789" }
  }
}
```

---

## Pipeline CI/CD — Descripción detallada

Fichero: `.github/workflows/deploy.yml`

### Job 1: `build-and-test`
1. Checkout del código
2. Setup .NET 8 SDK
3. `dotnet restore`
4. `dotnet build --configuration Release`
5. `dotnet test` con recolección de cobertura de código
6. `dotnet publish` → genera artefacto ZIP
7. Sube artefacto a GitHub Actions

### Job 2: `migrate` (depende de `build-and-test`)
1. Checkout del código
2. Setup .NET 8 SDK
3. `dotnet restore`
4. Instala `dotnet-ef` globalmente
5. Ejecuta `dotnet ef database update` contra Neon PostgreSQL usando el secret `NEON_CONNECTION_STRING_KV`

### Job 3: `deploy` (depende de `migrate`)
1. Descarga artefacto del Job 1
2. Login en Azure mediante OIDC
3. Despliega en Azure App Service (`criapp-api`)

---

## Variables de entorno en Azure App Service

Todas las configuraciones sensibles se almacenan como Application Settings en Azure (equivalentes a variables de entorno), nunca en el código fuente:

| Variable | Descripción |
|---|---|
| `ASPNETCORE_ENVIRONMENT` | `Production` |
| `ConnectionStrings__Postgres` | Connection string key-value de Neon |
| `JwtTokenSettings__Key` | Clave secreta para firma de tokens JWT |
| `JwtTokenSettings__Issuer` | Issuer del token JWT |
| `JwtTokenSettings__ExpiresInMinutes` | Expiración del token |
| `UserSettings__PasswordPepper` | Pepper para hashing de contraseñas |
| `UserSettings__PasswordHashIterations` | Iteraciones del algoritmo de hashing |
| `TwilioSettings__ClientId` | Account SID de Twilio |
| `TwilioSettings__ClientSecret` | Auth Token de Twilio |
| `TwilioSettings__SenderPhoneNumber` | Número remitente de SMS |
| `MailjetSettings__*` | Configuración SMTP para envío de emails |

---

## URLs de producción

| Recurso | URL |
|---|---|
| API base | `https://criapp-api-a0fef0esc6h6dxe5.spaincentral-01.azurewebsites.net` |
| Swagger UI | `.../swagger` |
| Health completo | `.../health` |
| Health app | `.../health/self` |

---

## Problemas encontrados y soluciones

### 1. `.github` en `.gitignore`
**Problema:** Los ficheros de workflow nunca se subían al repositorio.  
**Solución:** Eliminar la entrada `.github` del `.gitignore`.

### 2. Azure Pipelines requiere pago
**Problema:** El paralelismo gratuito fue eliminado por Microsoft para proyectos privados.  
**Solución:** Migrar a GitHub Actions.

### 3. `DOTNET_VERSION` reservada por el runner
**Problema:** La variable de entorno `DOTNET_VERSION` en el YAML conflictúa con una variable interna del runner de GitHub Actions, causando formato inválido.  
**Solución:** Renombrar a `DOTNET_SDK_VERSION`.

### 4. `AddDbContext` sin constructor compatible
**Problema:** `AppContext` solo tenía constructor sin parámetros; `AddDbContext` con opciones externas requiere constructor que acepte `DbContextOptions<T>`.  
**Solución:** Añadir constructor `public AppContext(DbContextOptions<AppContext> options) : base(options) {}`.

### 5. Incompatibilidad de versiones EF Core 6 vs 8
**Problema:** `CRIApp.Repository` usaba EF Core 6.0 mientras la API usaba EF Core 8.0, causando conflictos de dependencias (`NU1605`).  
**Solución:** Actualizar todos los paquetes EF Core a la versión 8.0.4 en todos los proyectos, alineando también `Microsoft.AspNetCore.Authentication.JwtBearer` y `System.IdentityModel.Tokens.Jwt`.

### 6. Formato URI incompatible con Npgsql en migraciones
**Problema:** Neon proporciona la connection string en formato URI (`postgresql://...`), que Npgsql no acepta en tiempo de diseño (migraciones EF).  
**Solución:** Crear un segundo secret `NEON_CONNECTION_STRING_KV` con formato key-value para las migraciones, y actualizar el mismo formato en Azure App Service.

### 7. `OnConfiguring` no leía variables de entorno
**Problema:** El método `OnConfiguring` del `AppContext` solo leía `appsettings.json`, ignorando las variables de entorno de la pipeline.  
**Solución:** Añadir `.AddEnvironmentVariables()` al `ConfigurationBuilder` y el paquete `Microsoft.Extensions.Configuration.EnvironmentVariables`.

### 8. Hangfire LiteDB crasheaba en startup
**Problema:** Hangfire intentaba crear `hf.db` en el directorio de trabajo, que puede no ser escribible en Azure App Service F1.  
**Solución temporal:** Hangfire desactivado en producción. Pendiente de reemplazar por `IHostedService` o migrar a un storage compatible.

---

## Pendiente / Trabajo futuro

- [ ] Reactivar el job recurrente de Hangfire usando `IHostedService` en lugar de LiteDB.
- [ ] Configurar dominio personalizado con SSL (requiere plan B1 o superior).
- [ ] Revisar y rotar las credenciales de Twilio y Mailjet almacenadas en `appsettings.json` (actualmente también en el repositorio).
- [ ] Añadir cobertura de código al informe de la pipeline.
- [ ] Considerar mover a `appsettings.Production.json` generado en la pipeline mediante secrets.
