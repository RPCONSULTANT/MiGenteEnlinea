SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRANSACTION;

IF NOT EXISTS (SELECT 1 FROM Planes_empleadores)
BEGIN
    SET IDENTITY_INSERT Planes_empleadores ON;

    INSERT INTO Planes_empleadores (planID, nombre, precio, empleados, historico, nomina)
    VALUES
        (1, 'Mi Gente, Soy Yo', 495.00, 1, 12, 0),
        (2, 'Mi Gente en Familia', 1695.00, 5, 12, 0),
        (3, 'Mi Gente Somos Todos', 3750.00, 15, 12, 1);

    SET IDENTITY_INSERT Planes_empleadores OFF;
END;

IF NOT EXISTS (SELECT 1 FROM Planes_Contratistas)
BEGIN
    SET IDENTITY_INSERT Planes_Contratistas ON;

    INSERT INTO Planes_Contratistas (planID, nombrePlan, precio)
    VALUES (4, 'Plan Ofertantes', 499.00);

    SET IDENTITY_INSERT Planes_Contratistas OFF;
END;

IF NOT EXISTS (SELECT 1 FROM Provincias)
BEGIN
    SET IDENTITY_INSERT Provincias ON;

    INSERT INTO Provincias (provinciaID, nombre) VALUES
        (0, 'Cualquier Ubicacion'),
        (1, 'Azua'), (2, 'Bahoruco'), (3, 'Barahona'), (4, 'Dajabon'),
        (5, 'Distrito Nacional'), (6, 'Duarte'), (7, 'Elias Pina'),
        (8, 'El Seibo'), (9, 'Espaillat'), (10, 'Hato Mayor'),
        (11, 'Hermanas Mirabal'), (12, 'Independencia'), (13, 'La Altagracia'),
        (14, 'La Romana'), (15, 'La Vega'), (16, 'Maria Trinidad Sanchez'),
        (17, 'Monsenor Nouel'), (18, 'Monte Cristi'), (19, 'Monte Plata'),
        (20, 'Pedernales'), (21, 'Peravia'), (22, 'Puerto Plata'),
        (23, 'Samana'), (24, 'San Cristobal'), (25, 'San Jose de Ocoa'),
        (26, 'San Juan'), (27, 'San Pedro de Macoris'), (28, 'Sanchez Ramirez'),
        (29, 'Santiago'), (30, 'Santiago Rodriguez'), (31, 'Valverde'),
        (32, 'Santo Domingo Este'), (33, 'Santo Domingo Oeste'), (34, 'Santo Domingo Norte');

    SET IDENTITY_INSERT Provincias OFF;
END;

IF NOT EXISTS (SELECT 1 FROM Sectores)
BEGIN
    SET IDENTITY_INSERT Sectores ON;

    INSERT INTO Sectores (sectorID, sector) VALUES
        (1, 'Medicina y Salud'), (2, 'Tecnologia de la Informacion'),
        (3, 'Educacion y Docencia'), (4, 'Finanzas y Contabilidad'),
        (5, 'Marketing y Publicidad'), (6, 'Diseno Grafico y Multimedia'),
        (7, 'Arquitectura y Construccion'), (8, 'Ingenieria'),
        (9, 'Derecho y Asesoria Legal'), (10, 'Recursos Humanos y Gestion de Personal'),
        (11, 'Consultoria Empresarial'), (12, 'Comunicacion y Medios de Comunicacion'),
        (13, 'Turismo y Hosteleria'), (14, 'Arte y Cultura'),
        (15, 'Agricultura y Agroindustria'), (16, 'Ciencia y Investigacion'),
        (17, 'Desarrollo Sostenible y Medio Ambiente'), (18, 'Deportes y Actividad Fisica'),
        (19, 'Alimentacion y Gastronomia'), (20, 'Belleza y Estetica'),
        (21, 'Fotografia y Videografia'), (22, 'Entretenimiento y Eventos'),
        (23, 'Reparaciones y Mantenimiento'), (24, 'Jardineria y Paisajismo'),
        (25, 'Peluqueria y Barberia'), (26, 'Transporte y Logistica'),
        (27, 'Artesania y Manualidades'), (28, 'Escritura y Redaccion'),
        (29, 'Traduccion e Interpretacion'), (30, 'Programacion y Desarrollo de Software'),
        (31, 'Soporte Tecnico y Reparacion de Equipos'),
        (32, 'Diseno Web y Desarrollo Frontend'), (33, 'Ingenieria de Software'),
        (34, 'Ciberseguridad'), (35, 'Analisis de Datos y Business Intelligence'),
        (36, 'Redes y Comunicaciones'), (37, 'Administracion de Sistemas'),
        (38, 'Robotica y Automatizacion'), (39, 'Electronica y Hardware'),
        (40, 'Audio y Produccion Musical'), (41, 'Ebanisteria');

    SET IDENTITY_INSERT Sectores OFF;
END;

IF NOT EXISTS (SELECT 1 FROM Servicios)
BEGIN
    SET IDENTITY_INSERT Servicios ON;

    INSERT INTO Servicios (servicioID, descripcion, userID) VALUES
        (1, 'Plomeria', NULL), (2, 'Electricidad', NULL), (3, 'Carpinteria', NULL),
        (4, 'Pintura', NULL), (5, 'Albanileria', NULL), (6, 'Jardineria', NULL),
        (7, 'Limpieza Residencial', NULL), (8, 'Limpieza Comercial', NULL),
        (9, 'Mecanica Automotriz', NULL), (10, 'Aire Acondicionado', NULL),
        (11, 'Refrigeracion', NULL), (12, 'Herreria', NULL), (13, 'Cerrajeria', NULL),
        (14, 'Techado', NULL), (15, 'Instalacion de Pisos', NULL),
        (16, 'Instalacion de Vidrios', NULL), (17, 'Mudanzas', NULL),
        (18, 'Transporte', NULL), (19, 'Cuidado de Ninos', NULL),
        (20, 'Cuidado de Adultos Mayores', NULL), (21, 'Cocina/Chef', NULL),
        (22, 'Reposteria', NULL), (23, 'Peluqueria', NULL), (24, 'Barberia', NULL),
        (25, 'Estetica', NULL), (26, 'Masajes', NULL), (27, 'Entrenador Personal', NULL),
        (28, 'Clases Particulares', NULL), (29, 'Traduccion', NULL),
        (30, 'Diseno Grafico', NULL), (31, 'Fotografia', NULL), (32, 'Videografia', NULL),
        (33, 'Desarrollo Web', NULL), (34, 'Reparacion de Computadoras', NULL),
        (35, 'Reparacion de Celulares', NULL), (36, 'Asesoria Legal', NULL),
        (37, 'Asesoria Contable', NULL), (38, 'Asesoria Financiera', NULL),
        (39, 'Marketing Digital', NULL), (40, 'Redes Sociales', NULL);

    SET IDENTITY_INSERT Servicios OFF;
END;

INSERT INTO Ofertantes (userID, fechaPublicacion, descripcion)
SELECT p.userID, GETUTCDATE(), CONCAT('Empleador: ', p.nombre, ' ', p.apellido)
FROM Perfiles p
LEFT JOIN Ofertantes o ON p.userID = o.userID
WHERE p.tipo = 1 AND o.ofertanteID IS NULL;

COMMIT TRANSACTION;
