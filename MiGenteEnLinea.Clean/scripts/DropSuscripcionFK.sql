-- Script para eliminar la FK que causa conflictos con planes de contratistas
-- El problema es que Suscripciones.planID puede venir de:
--   - Planes_empleadores (para empleadores)
--   - Planes_contratistas (para contratistas)
-- Pero la FK solo apunta a Planes_empleadores, causando error cuando un contratista compra su plan

USE MiGenteTestDB2;
GO

-- Verificar qué constraints existen en la tabla Suscripciones
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
    OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferencedColumn
FROM sys.foreign_keys fk
INNER JOIN sys.foreign_key_columns fc ON fk.object_id = fc.constraint_object_id
WHERE OBJECT_NAME(fk.parent_object_id) = 'Suscripciones';
GO

-- Eliminar la FK problemática (si existe)
IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Suscripciones_Planes_empleadores')
BEGIN
    ALTER TABLE Suscripciones DROP CONSTRAINT FK_Suscripciones_Planes_empleadores;
    PRINT 'FK_Suscripciones_Planes_empleadores eliminada exitosamente';
END
ELSE
BEGIN
    PRINT 'FK_Suscripciones_Planes_empleadores no existe';
END
GO

-- Verificar que la FK fue eliminada
SELECT 
    fk.name AS ForeignKeyName,
    OBJECT_NAME(fk.parent_object_id) AS TableName
FROM sys.foreign_keys fk
WHERE OBJECT_NAME(fk.parent_object_id) = 'Suscripciones';
GO
