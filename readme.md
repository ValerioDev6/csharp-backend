# 🚀 Sistema Web API

API REST desarrollada en **.NET 7** con Entity Framework Core, autenticación JWT y datos fake con Bogus.

## 📋 Requisitos

- .NET 7.0.410 SDK
- SQL Server (LocalDB o instancia)
- Visual Studio Code o Visual Studio

## 🛠️ Instalación y Configuración

### 1. Crear el proyecto desde cero

```bash
# Configurar versión de .NET
dotnet new globaljson --sdk-version 7.0.410 --force

# Crear solución
dotnet new sln --name SistemaApi

# Crear proyecto Web API
dotnet new webapi -o src/Sistema.WebApi --name Sistema.WebApi

# Agregar proyecto a la solución
dotnet sln add src/Sistema.WebApi/Sistema.WebApi.csproj
```

### 2. Instalar paquetes NuGet

```bash
# Entity Framework Core para SQL Server
dotnet add src/Sistema.WebApi package Microsoft.EntityFrameworkCore.SqlServer --version 7.0.20

# Bogus para datos de prueba
dotnet add src/Sistema.WebApi package Bogus

# JWT Bearer para autenticación
dotnet add src/Sistema.WebApi package Microsoft.AspNetCore.Authentication.JwtBearer
```

### 3. Compilar y ejecutar

```bash
# Compilar el proyecto
dotnet build

# Ejecutar la aplicación
dotnet run --project src/Sistema.WebApi
```

## 🌐 Acceso a la API

Una vez ejecutado el proyecto:

- **Swagger UI**: `http://localhost:5162/swagger/index.html`
- **API Base URL**: `http://localhost:5162/api`

## 📁 Estructura del Proyecto

```
src/Sistema.WebApi/
├── Controllers/          # Controladores de la API
├── Entities/            # Modelos de datos
├── Datos/               # Contexto de base de datos
├── ViewModels/          # DTOs y ViewModels
├── Mapping/             # Configuración de Entity Framework
├── Utils/               # Utilidades (PasswordUtil, etc.)
├── common/              # Respuestas comunes (ApiResponse)
└── Program.cs           # Configuración de la aplicación
```

## 🔧 Principales Funcionalidades

- ✅ **CRUD completo** para múltiples entidades
- ✅ **Autenticación JWT** con roles
- ✅ **Entity Framework Core** con SQL Server
- ✅ **Datos de prueba** generados con Bogus
- ✅ **Swagger/OpenAPI** documentación automática
- ✅ **Arquitectura limpia** por capas

## 🎯 Endpoints Principales

### Autenticación
- `POST /api/auth/login` - Iniciar sesión
- `POST /api/auth/register` - Registrar usuario

### Gestión
- `GET/POST/PUT/DELETE /api/categorias` - Gestión de categorías
- `GET/POST/PUT/DELETE /api/articulos` - Gestión de artículos
- `GET/POST/PUT/DELETE /api/usuarios` - Gestión de usuarios
- `GET/POST/PUT/DELETE /api/ventas` - Gestión de ventas
- `GET/POST/PUT/DELETE /api/compras` - Gestión de compras

