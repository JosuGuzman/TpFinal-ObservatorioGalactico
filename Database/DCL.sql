USE BD_WatchTower;

/* ============================================================
   DCL – WATCHTOWER
   Data Control Language (Seguridad y permisos de BD)
   ============================================================ */

-- Crear usuarios internos del servidor MySQL
CREATE USER 'watchtower_admin'@'%' IDENTIFIED BY 'AdminPassword123!';
CREATE USER 'watchtower_api'@'%' IDENTIFIED BY 'ApiPassword123!';
CREATE USER 'watchtower_readonly'@'%' IDENTIFIED BY 'ReadOnly123!';

-- Privilegios del ADMIN (control total del esquema)
GRANT ALL PRIVILEGES ON WatchTower.* TO 'watchtower_admin'@'%';

-- Privilegios del API (permisos necesarios para el backend)
GRANT SELECT, INSERT, UPDATE, DELETE, EXECUTE
ON WatchTower.* TO 'watchtower_api'@'%';

-- Permisos específicos para ejecutar SP y SF
GRANT EXECUTE ON PROCEDURE WatchTower.* TO 'watchtower_api'@'%';
GRANT EXECUTE ON FUNCTION WatchTower.* TO 'watchtower_api'@'%';

-- Usuario solo lectura (por ejemplo para auditoría o analytics)
GRANT SELECT ON WatchTower.* TO 'watchtower_readonly'@'%';

-- Aplicar los cambios de permisos
FLUSH PRIVILEGES;