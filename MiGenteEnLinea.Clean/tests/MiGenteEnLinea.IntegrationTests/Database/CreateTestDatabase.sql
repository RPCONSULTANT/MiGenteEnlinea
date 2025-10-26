-- ================================================================================
-- Script para crear Base de Datos de Integration Tests
-- MiGente En Línea - Clean Architecture
-- ================================================================================
-- Propósito: Reemplazar InMemory Database con SQL Server real para tests
-- Fecha: 2025-10-26
-- ================================================================================

USE master;
GO

-- Eliminar DB si existe (para recrear limpia en cada sesión de tests)
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'MiGenteEnLinea_IntegrationTests')
BEGIN
    ALTER DATABASE [MiGenteEnLinea_IntegrationTests] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [MiGenteEnLinea_IntegrationTests];
    PRINT 'Database eliminada: MiGenteEnLinea_IntegrationTests';
END
GO

-- Crear nueva database
CREATE DATABASE [MiGenteEnLinea_IntegrationTests]
    COLLATE Modern_Spanish_CI_AS;
GO

PRINT 'Database creada: MiGenteEnLinea_IntegrationTests';
GO

-- Configurar para tests
ALTER DATABASE [MiGenteEnLinea_IntegrationTests] SET RECOVERY SIMPLE;
GO

USE [MiGenteEnLinea_IntegrationTests];
GO

PRINT 'Database lista para EF Core Migrations.';
PRINT 'Ejecutar siguiente comando:';
PRINT 'dotnet ef database update --startup-project src/Presentation/MiGenteEnLinea.API --project src/Infrastructure/MiGenteEnLinea.Infrastructure --connection "Server=.;Database=MiGenteEnLinea_IntegrationTests;Trusted_Connection=True;TrustServerCertificate=True;"';
GO
