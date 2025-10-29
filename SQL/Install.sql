-- Script de instalación completa de la base de datos Observatorio Paranal
-- Ejecutar este archivo para crear toda la estructura

SOURCE BD_WatchTower.sql;
SOURCE SF.sql;
SOURCE SP.sql;
SOURCE Triggers.sql;
SOURCE Inserts.sql;
SOURCE DCL.sql;

-- Mostrar mensaje de finalización
SELECT 'Base de datos Observatorio Paranal instalada exitosamente!' AS Message;

-- Mostrar resumen de datos insertados
SELECT 
    (SELECT COUNT(*) FROM Users) AS TotalUsers,
    (SELECT COUNT(*) FROM CelestialBodies) AS TotalCelestialBodies,
    (SELECT COUNT(*) FROM Discoveries) AS TotalDiscoveries,
    (SELECT COUNT(*) FROM Articles) AS TotalArticles,
    (SELECT COUNT(*) FROM Events) AS TotalEvents;