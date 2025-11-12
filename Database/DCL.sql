USE `BD_WatchTower`;

-- Crear usuarios para la aplicación
CREATE USER 'astro_app'@'localhost' IDENTIFIED BY 'SecurePassword123!';
CREATE USER 'astro_read'@'localhost' IDENTIFIED BY 'ReadOnlyPass123!';

-- Conceder permisos completos al usuario de la aplicación
GRANT SELECT, INSERT, UPDATE, DELETE ON `BD_WatchTower`.* TO 'astro_app'@'localhost';

-- Conceder permisos de solo lectura para reportes
GRANT SELECT ON `BD_WatchTower`.* TO 'astro_read'@'localhost';

-- Permisos específicos para ejecutar stored procedures
GRANT EXECUTE ON PROCEDURE `BD_WatchTower`.`SearchCelestialBodies` TO 'astro_app'@'localhost';
GRANT EXECUTE ON PROCEDURE `BD_WatchTower`.`SearchDiscoveries` TO 'astro_app'@'localhost';
GRANT EXECUTE ON PROCEDURE `BD_WatchTower`.`SearchArticles` TO 'astro_app'@'localhost';
GRANT EXECUTE ON PROCEDURE `BD_WatchTower`.`GetUserStatistics` TO 'astro_app'@'localhost';
GRANT EXECUTE ON PROCEDURE `BD_WatchTower`.`AddDiscovery` TO 'astro_app'@'localhost';

-- Permisos de solo lectura para funciones
GRANT EXECUTE ON FUNCTION `BD_WatchTower`.`CalculateDiscoveryRating` TO 'astro_read'@'localhost';
GRANT EXECUTE ON FUNCTION `BD_WatchTower`.`CountUserDiscoveries` TO 'astro_read'@'localhost';
GRANT EXECUTE ON FUNCTION `BD_WatchTower`.`IsVisibleFromEarth` TO 'astro_read'@'localhost';

-- Aplicar los cambios
FLUSH PRIVILEGES;