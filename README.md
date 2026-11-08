# SmartInventory

Aplicación de demostración para administrar un catálogo de productos e inventario mediante una API REST. El cliente web se ejecuta localmente y consume la API por HTTP. No procesa pagos ni se conecta a servicios de terceros.

## Estado actual

- API ASP.NET Core sobre .NET 10.
- Capas Domain, Application, Infrastructure y API.
- Persistencia configurada con EF Core 10 y SQL Server/Azure SQL.
- Endpoints iniciales para consultar, buscar y registrar productos.
- Cliente React + TypeScript + Vite para consumir esos endpoints.
- Configuración local mediante .NET User Secrets; no guardar cadenas de conexión en archivos versionados.

## Tecnologías

| Tecnología | Uso |
|---|---|
| C# / .NET 10 / ASP.NET Core | API HTTP y validación de reglas del catálogo |
| Clean Architecture | Mantener dominio, casos de uso, persistencia y HTTP separados |
| EF Core 10 / SQL Server | Persistencia, consultas y futuras migraciones |
| React / TypeScript / Vite | Cliente web local conectado a la API |
| GitHub Actions | CI para compilar la API y verificar lint/build del frontend |

## Estructura

```text
src/
  SmartInventory.Domain/         Entidades e invariantes
  SmartInventory.Application/    Casos de uso y contratos
  SmartInventory.Infrastructure/ Persistencia EF Core y adaptadores
  SmartInventory.Api/            Controladores REST y composition root
web/
  smartinventory-web/            Cliente React que consume la API
```

La API registra los servicios de Application e Infrastructure en `Program.cs`. Los controladores dependen de contratos/casos de uso de Application, no de EF Core.

## Ejecución local

Requisitos: .NET 10 SDK, Node.js LTS y una instancia accesible de SQL Server 2019 o posterior. También puede usarse Azure SQL.

Desde la raíz:

```powershell
dotnet restore SmartInventory.slnx
dotnet user-secrets init --project src/SmartInventory.Api
dotnet user-secrets set "ConnectionStrings:SmartInventory" "Server=localhost;Database=SmartInventory;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true" --project src/SmartInventory.Api
dotnet tool restore
dotnet tool run dotnet-ef database update --project src/SmartInventory.Infrastructure/SmartInventory.Infrastructure.csproj --startup-project src/SmartInventory.Api/SmartInventory.Api.csproj
dotnet run --project src/SmartInventory.Api
```

El nombre del servidor debe ajustarse a la instancia disponible. El proyecto no contiene credenciales predeterminadas. API: `http://localhost:5058`; health: `/health`; OpenAPI en Development: `/openapi/v1.json`.

En otra terminal:

```powershell
cd web/smartinventory-web
npm ci
npm run dev
```

Cliente: `http://localhost:5173`. Para otra URL de API, copia `.env.example` como `.env.local` y cambia `VITE_API_BASE_URL`.

## Hosting gratuito para demostración

Vercel ahora documenta el despliegue de aplicaciones ASP.NET Core en una imagen Docker ejecutada como Vercel Function. La imagen debe escuchar en `0.0.0.0` y en el puerto definido por `PORT`. La API y el cliente React pueden publicarse en Vercel para una demo personal. El plan Hobby es gratuito para uso personal/no comercial e incluye límites mensuales; al excederlos, las funciones afectadas se pausan hasta que se restablezca el límite.

El `Dockerfile` de la raíz publica `SmartInventory.Api`. En el proyecto de Vercel se debe usar la raíz del repositorio y definir `PORT=8080`, `ConnectionStrings__SmartInventory` y `Cors__AllowedOrigins__0` en Variables de entorno. La cadena SQL debe configurarse como variable sensible, nunca en archivos del repositorio. El cliente web puede configurarse como un proyecto Vercel separado usando `web/smartinventory-web` como directorio raíz.

La base de datos debe ser externa: el sistema de archivos del contenedor no es persistente. Azure SQL Database ofrece hasta 100,000 vCore-seconds, 32 GB de datos y 32 GB de backups por mes y base de datos. Se debe seleccionar la pausa al llegar al límite para impedir cargos por excedente. La elegibilidad depende de la suscripción y región. La API no debe usar `BackgroundService` para tareas que deban sobrevivir entre solicitudes.

Referencias: [ASP.NET Core en Vercel con Docker](https://vercel.com/kb/guide/dot-net-asp-net-on-vercel-with-docker), [límites del plan Hobby](https://vercel.com/docs/plans/hobby), [Azure SQL Database gratis](https://learn.microsoft.com/en-us/azure/azure-sql/database/free-offer).

No se crea ni despliega ningún recurso cloud hasta revisar la cuenta, región, límites y posibles cargos.

## Próximos hitos

1. Completar balances y movimientos de stock con transacciones y control de concurrencia.
2. Añadir endpoints de almacenes, reportes y auditoría.
3. Incorporar autenticación y roles.
4. Añadir pruebas unitarias/de integración y ampliar CI para ejecutarlas.
5. Configurar variables de entorno y migraciones cloud para Vercel.
6. Desplegar una demo gratuita después de confirmar cuenta, límites y base de datos.
