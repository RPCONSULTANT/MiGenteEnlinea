-- Limpieza selectiva: SOLO usuarios ligados a Ofertantes/Contratistas
-- Preserva planes y catálogos (Planes_*, Provincias, Sectores, Servicios, etc.)
-- SQL Server
SET XACT_ABORT ON;
BEGIN TRAN;

DECLARE @DoCommit bit = 0; -- 0=ROLLBACK (prueba), 1=COMMIT (aplicar)

IF OBJECT_ID('tempdb..#TargetUsers') IS NOT NULL DROP TABLE #TargetUsers;

SELECT DISTINCT userID
INTO #TargetUsers
FROM (
    SELECT userID FROM Ofertantes
    UNION
    SELECT userID FROM Contratistas
) u
WHERE userID IS NOT NULL;

-- 1) NÓMINA EMPLEADOS FIJOS
DELETE d
FROM Empleador_Recibos_Detalle d
INNER JOIN Empleador_Recibos_Header h ON h.pagoID = d.pagoID
INNER JOIN Empleados e ON e.empleadoID = h.empleadoID
INNER JOIN #TargetUsers tu ON tu.userID = e.userID;

DELETE h
FROM Empleador_Recibos_Header h
INNER JOIN Empleados e ON e.empleadoID = h.empleadoID
INNER JOIN #TargetUsers tu ON tu.userID = e.userID;

DELETE r
FROM Remuneraciones r
LEFT JOIN Empleados e ON e.empleadoID = r.empleadoID
WHERE r.userID IN (SELECT userID FROM #TargetUsers)
   OR e.userID IN (SELECT userID FROM #TargetUsers);

DELETE n
FROM Empleados_Notas n
LEFT JOIN Empleados e ON e.empleadoID = n.empleadoID
WHERE n.userID IN (SELECT userID FROM #TargetUsers)
   OR e.userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Empleados
WHERE userID IN (SELECT userID FROM #TargetUsers);

-- 2) CONTRATACIONES TEMPORALES
DELETE d
FROM Empleador_Recibos_Detalle_Contrataciones d
INNER JOIN Empleador_Recibos_Header_Contrataciones h ON h.pagoID = d.pagoID
INNER JOIN Empleados_Temporales t ON t.contratacionID = h.contratacionID
INNER JOIN #TargetUsers tu ON tu.userID = t.userID;

DELETE FROM Detalle_Contrataciones
WHERE contratacionID IN (
    SELECT contratacionID
    FROM Empleados_Temporales
    WHERE userID IN (SELECT userID FROM #TargetUsers)
);

DELETE FROM Empleador_Recibos_Header_Contrataciones
WHERE contratacionID IN (
    SELECT contratacionID
    FROM Empleados_Temporales
    WHERE userID IN (SELECT userID FROM #TargetUsers)
);

DELETE FROM Empleados_Temporales
WHERE userID IN (SELECT userID FROM #TargetUsers);

-- 3) CONTRATISTAS / EMPLEADORES Y DERIVADOS
DELETE cf
FROM Contratistas_Fotos cf
INNER JOIN Contratistas c ON c.contratistaID = cf.contratistaID
WHERE c.userID IN (SELECT userID FROM #TargetUsers);

DELETE cs
FROM Contratistas_Servicios cs
INNER JOIN Contratistas c ON c.contratistaID = cs.contratistaID
WHERE c.userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Contratistas
WHERE userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Ofertantes
WHERE userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Suscripciones
WHERE userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Calificaciones
WHERE userID IN (SELECT userID FROM #TargetUsers);

-- 4) AUTH / PERFIL
DELETE prt
FROM PasswordResetTokens prt
WHERE prt.UserId IN (SELECT userID FROM #TargetUsers)
   OR prt.Email IN (
       SELECT email
       FROM Credenciales
       WHERE userID IN (SELECT userID FROM #TargetUsers)
   );

DELETE FROM perfilesInfo
WHERE userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Perfiles
WHERE userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM Credenciales
WHERE userID IN (SELECT userID FROM #TargetUsers);

DELETE FROM RefreshTokens
WHERE UserId IN (SELECT userID FROM #TargetUsers);

-- Identity (claims/logins/roles/tokens borran en cascada)
DELETE FROM AspNetUsers
WHERE Id IN (SELECT userID FROM #TargetUsers);

-- Resumen rápido
SELECT (SELECT COUNT(*) FROM #TargetUsers) AS TargetUsers;

IF @DoCommit = 1
    COMMIT TRAN;
ELSE
    ROLLBACK TRAN;
