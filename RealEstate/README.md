# RealEstate API (MongoDB + .NET 8)

API REST para gestión de inmuebles, propietarios, trazas y autenticación JWT.

## Requisitos
- .NET SDK 8.0
- MongoDB (local o en Docker)
- Opcional: Docker/Docker Compose para levantar MongoDB rápidamente

## Ejecutar local
1) Restaurar y compilar
```
dotnet restore
dotnet build -c Release
```

2) Base de datos MongoDB
- Opción A: Docker Compose (usuario root/example)
```
docker compose up -d mongo
```
- Opción B: Mongo local sin credenciales (ajusta la cadena de conexión según tu instalación)

3) Variables de entorno (recomendado)
Configura variables para apuntar a tu Mongo y JWT. En macOS/Linux (zsh/bash):
```
export ASPNETCORE_ENVIRONMENT=Development
export MongoDb__ConnectionString="mongodb://root:example@localhost:27017"
export MongoDb__Database="RealEstateDb"
export Jwt__Issuer="RealEstateApi"
export Jwt__Audience="RealEstateApiClients"
export Jwt__SecretKey="cambia-esta-clave-en-produccion"
export Jwt__ExpirationMinutes=60
export FileStorage__RootPath="wwwroot"
export FileStorage__BaseRequestPath="/"
```
Notas:
- Si usas el `docker-compose.yml` incluido (con root/example), necesitas credenciales en la cadena de conexión.
- Puedes dejar los valores por defecto del `appsettings.json`, pero no se recomienda para producción (no subas SecretKey reales al repo).

4) Levantar la API
```
dotnet run --project src/RealEstate.Api/RealEstate.Api.csproj
```
La API quedará en `http://localhost:5106` (según `launchSettings.json`). Swagger: `http://localhost:5106/swagger`.

## Endpoints principales
- Auth
  - POST `/api/auth/register` (AllowAnonymous)
  - POST `/api/auth/login` (AllowAnonymous)
- Owners (propietarios)
  - GET `/api/owner/{id}`
  - GET `/api/owner?name=...&skip=0&take=50`
  - POST `/api/owner`
  - PUT `/api/owner/{id}`
  - DELETE `/api/owner/{id}`
- Properties (inmuebles)
  - GET `/api/properties` (público; filtros via query: `ownerId`, `text`, `priceMin`, `priceMax`, `year`, `page`, `pageSize`)
  - GET `/api/properties/{id}` (requiere JWT)
  - POST `/api/properties` (requiere JWT)
  - PUT `/api/properties/{id}` (requiere JWT)
  - PATCH `/api/properties/{id}/price` (requiere JWT)
  - PUT `/api/properties/{id}/price` (requiere JWT)
- Property Images
  - POST `/api/properties/{id}/images` (multipart/form-data, requiere JWT)

## Ejemplos (curl)
1) Registro y login
```
curl -s -X POST http://localhost:5106/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{"email":"user1@example.com","password":"Passw0rd!"}'

curl -s -X POST http://localhost:5106/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"user1@example.com","password":"Passw0rd!"}'
# Respuesta: { accessToken, expiresAtUtc, email, role }
```
Guarda el token:
```
TOKEN="<pega_aqui_accessToken>"
```

2) Crear Owner (requiere JWT)
```
curl -s -X POST http://localhost:5106/api/owner \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{"name":"John Doe","address":"Main St","photo":""}'
```

3) Listado público de Properties
```
curl -s "http://localhost:5106/api/properties?page=1&pageSize=10"
```

4) Crear Property (requiere JWT)
```
OWNER_ID="<guid_owner>"
curl -s -X POST http://localhost:5106/api/properties \
  -H "Authorization: Bearer $TOKEN" \
  -H "Content-Type: application/json" \
  -d '{
    "name":"Casa Centro",
    "street":"Calle 1",
    "city":"Bogotá",
    "state":"Cundinamarca",
    "country":"CO",
    "zipCode":"110111",
    "price":200000,
    "currency":"USD",
    "year":2015,
    "area":120.5,
    "ownerId":"'"$OWNER_ID"'",
    "active":true
  }'
```

5) Subir imagen (multipart) (requiere JWT)
```
curl -s -X POST http://localhost:5106/api/properties/<propertyId>/images \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@./foto.png" \
  -F "description=fachada" \
  -F "enabled=true"
```
Las imágenes se guardan bajo `wwwroot/uploads/yyyy/MM/dd/`. La URL pública se construye con `FileStorage.BaseRequestPath`.

## Validación, errores y logging
- Validación con FluentValidation: errores 400 con formato consistente.
- Middleware de excepciones devuelve `ErrorResponse` (traceId, error, message, errors).
- Serilog a consola (ajustable en configuración).

## Seguridad (JWT)
- Endpoints sensibles requieren header `Authorization: Bearer <token>`.
- Genera token con `/api/auth/login`.
- En Swagger, usa el botón Authorize (si está configurado el esquema) y pega el token.

## Tests
- Unit tests (NUnit + Moq):
```
dotnet test tests/RealEstate.UnitTests/RealEstate.UnitTests.csproj -c Release
```
- Integration tests (WebApplicationFactory + Mongo2Go por defecto):
```
dotnet test tests/RealEstate.IntegrationTests/RealEstate.IntegrationTests.csproj -c Release
```
- También puedes apuntar los tests de integración a un Mongo externo exportando:
```
export MONGO_URL="mongodb://root:example@localhost:27017"
```

## Configuración (appsettings / variables)
Claves relevantes que puedes sobreescribir vía variables de entorno (`__` anidamiento):
- `MongoDb:ConnectionString` → `MongoDb__ConnectionString`
- `MongoDb:Database` → `MongoDb__Database`
- `Jwt:Issuer` → `Jwt__Issuer`
- `Jwt:Audience` → `Jwt__Audience`
- `Jwt:SecretKey` → `Jwt__SecretKey`
- `Jwt:ExpirationMinutes` → `Jwt__ExpirationMinutes`
- `FileStorage:RootPath` → `FileStorage__RootPath`
- `FileStorage:BaseRequestPath` → `FileStorage__BaseRequestPath`

## Despliegue / CI (resumen)
- Build y tests:
```
dotnet restore
dotnet build -c Release
dotnet test -c Release
```
- En CI (GitHub Actions), expón un servicio Mongo o usa `MONGO_URL` y ejecuta ambas suites (unit e integración).

## Notas
- Algunos warnings de paquetes (AutoMapper.Extensions vs AutoMapper 13.x). No afectan la ejecución; se pueden alinear versiones más adelante.
- No uses la `SecretKey` por defecto en entornos productivos.
