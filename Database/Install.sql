-- ======================================================
-- SCRIPT DE INSTALACIÓN COMPLETA - BD_WatchTower
-- ======================================================

-- Desactivar verificaciones de claves foráneas temporalmente
SET FOREIGN_KEY_CHECKS = 0;

-- Mostrar mensaje de inicio
SELECT 'Iniciando instalación de BD_WatchTower...' AS Message;

-- ======================================================
-- 1. CREAR ESQUEMA DE BASE DE DATOS
-- ======================================================
SOURCE BD_WatchTower.sql;

-- Verificar creación de tablas
SELECT 'Tablas creadas exitosamente.' AS Message;

-- ======================================================
-- 2. CREAR FUNCIONES ALMACENADAS
-- ======================================================
SOURCE SF.sql;

-- Verificar creación de funciones
SELECT 'Funciones almacenadas creadas exitosamente.' AS Message;

-- ======================================================
-- 3. CREAR PROCEDIMIENTOS ALMACENADOS
-- ======================================================
SOURCE SP.sql;

-- Verificar creación de procedimientos
SELECT 'Procedimientos almacenados creados exitosamente.' AS Message;

-- ======================================================
-- 4. CREAR TRIGGERS
-- ======================================================
SOURCE Triggers.sql;

-- Verificar creación de triggers
SELECT 'Triggers creados exitosamente.' AS Message;

-- ======================================================
-- 5. INSERTAR DATOS DE PRUEBA USANDO CALLs
-- ======================================================
SOURCE Inserts.sql;

-- Verificar inserción de datos
SELECT 'Datos de prueba insertados exitosamente.' AS Message;

-- ======================================================
-- 6. CONFIGURAR PERMISOS Y SEGURIDAD
-- ======================================================
SOURCE DCL.sql;

-- Verificar configuración de seguridad
SELECT 'Permisos de seguridad configurados exitosamente.' AS Message;

-- Reactivar verificaciones de claves foráneas
SET FOREIGN_KEY_CHECKS = 1;

-- ======================================================
-- 7. VERIFICACIÓN FINAL DEL SISTEMA
-- ======================================================

-- Mostrar mensaje de finalización
SELECT '===============================================' AS Separator;
SELECT 'Base de datos BD_WatchTower instalada exitosamente!' AS Message;
SELECT '===============================================' AS Separator;

-- Mostrar resumen de datos insertados
SELECT 'Resumen de datos instalados:' AS Title;

SELECT 
    (SELECT COUNT(*) FROM Galaxies) AS TotalGalaxias,
    (SELECT COUNT(*) FROM Stars) AS TotalEstrellas,
    (SELECT COUNT(*) FROM Planets) AS TotalPlanetas,
    (SELECT COUNT(*) FROM Users) AS TotalUsuarios,
    (SELECT COUNT(*) FROM Articles) AS TotalArticulos,
    (SELECT COUNT(*) FROM Events) AS TotalEventos,
    (SELECT COUNT(*) FROM Discoveries) AS TotalDescubrimientos,
    (SELECT COUNT(*) FROM DiscoveryVotes) AS TotalVotos,
    (SELECT COUNT(*) FROM UserFavorites) AS TotalFavoritos;

-- Mostrar información del sistema
SELECT 'Información del sistema:' AS Title;

SELECT 
    (SELECT COUNT(*) FROM information_schema.tables 
     WHERE table_schema = 'BD_WatchTower') AS TablasTotales,
    (SELECT COUNT(*) FROM information_schema.routines 
     WHERE routine_schema = 'BD_WatchTower' AND routine_type = 'FUNCTION') AS FuncionesTotales,
    (SELECT COUNT(*) FROM information_schema.routines 
     WHERE routine_schema = 'BD_WatchTower' AND routine_type = 'PROCEDURE') AS ProcedimientosTotales,
    (SELECT COUNT(*) FROM information_schema.triggers 
     WHERE trigger_schema = 'BD_WatchTower') AS TriggersTotales;

-- Mostrar usuarios creados
SELECT 'Usuarios disponibles:' AS Title;
SELECT UserID, UserName, Email, 
       (SELECT RoleName FROM Roles WHERE RoleID = Users.RoleID) AS Rol 
FROM Users ORDER BY UserID;

-- Mostrar ejemplos de datos insertados
SELECT 'Ejemplos de datos:' AS Title;
SELECT 'Galaxias:' AS Tipo, Name AS Nombre, Type AS TipoGalaxia, DistanceLy AS DistanciaLy 
FROM Galaxies LIMIT 3;

SELECT 'Estrellas:' AS Tipo, Name AS Nombre, SpectralType AS TipoEspectral, DistanceLy AS DistanciaLy 
FROM Stars LIMIT 3;

SELECT 'Planetas:' AS Tipo, Name AS Nombre, PlanetType AS TipoPlaneta, OrbitalDistanceAU AS DistanciaAU 
FROM Planets LIMIT 3;

-- Mostrar procedimientos disponibles
SELECT 'Procedimientos disponibles para pruebas:' AS Title;
SELECT 'CALL sp_get_galaxy_by_id(1);' AS Ejemplo_Consulta_Galaxia;
SELECT 'CALL sp_get_star_by_id(1);' AS Ejemplo_Consulta_Estrella;
SELECT 'CALL sp_get_planet_by_id(1);' AS Ejemplo_Consulta_Planeta;
SELECT 'CALL sp_get_discovery_by_id(1);' AS Ejemplo_Consulta_Descubrimiento;
SELECT 'CALL sp_search_fulltext(''galaxia'', 10, 0);' AS Ejemplo_Busqueda_Texto;
SELECT 'CALL sp_objects_nearby(10.0, 20.0, 5.0, 10);' AS Ejemplo_Busqueda_Cercana;

-- Instrucciones finales
SELECT '===============================================' AS Separator;
SELECT 'INSTRUCCIONES:' AS Title;
SELECT '1. La base de datos está lista para usar.' AS Instruccion;
SELECT '2. Usuario administrador: admin@watchtower.com' AS Instruccion;
SELECT '3. Usuarios de prueba: exp1@watchtower.com, obs77@watchtower.com' AS Instruccion;
SELECT '4. Ver archivo README.md para más detalles de uso.' AS Instruccion;
SELECT '===============================================' AS Separator;