USE `BD_WatchTower`;

-- Trigger para actualizar UpdatedAt en artículos
DELIMITER $$
CREATE TRIGGER `UpdateArticleTimestamp` 
BEFORE UPDATE ON `Articles`
FOR EACH ROW
BEGIN
    SET NEW.UpdatedAt = NOW();
END$$
DELIMITER ;

-- Trigger para establecer PublishedAt cuando se publica un artículo
DELIMITER $$
CREATE TRIGGER `SetArticlePublishDate` 
BEFORE UPDATE ON `Articles`
FOR EACH ROW
BEGIN
    IF NEW.IsPublished = 1 AND OLD.IsPublished = 0 THEN
        SET NEW.PublishedAt = NOW();
    END IF;
END$$
DELIMITER ;

-- Trigger para incrementar contador de vistas cuando se visita un artículo
DELIMITER $$
CREATE TRIGGER `IncrementArticleViewCount` 
BEFORE UPDATE ON `Articles`
FOR EACH ROW
BEGIN
    IF NEW.ViewCount < OLD.ViewCount THEN
        SET NEW.ViewCount = OLD.ViewCount + 1;
    END IF;
END$$
DELIMITER ;

-- Trigger para registrar automáticamente en el historial cuando se marca como favorito
DELIMITER $$
CREATE TRIGGER `AddToHistoryOnFavorite` 
AFTER INSERT ON `Favorites`
FOR EACH ROW
BEGIN
    IF NEW.CelestialBodyId IS NOT NULL THEN
        INSERT INTO ExplorationHistory (UserId, CelestialBodyId, VisitedAt)
        VALUES (NEW.UserId, NEW.CelestialBodyId, NOW())
        ON DUPLICATE KEY UPDATE VisitedAt = NOW(), TimeSpent = TimeSpent + 60;
    END IF;
END$$
DELIMITER ;

-- Trigger para prevenir que usuarios inactivos realicen acciones
DELIMITER $$
CREATE TRIGGER `PreventInactiveUserActions` 
BEFORE INSERT ON `Discoveries`
FOR EACH ROW
BEGIN
    DECLARE user_active TINYINT;
    
    SELECT IsActive INTO user_active FROM Users WHERE UserId = NEW.ReportedBy;
    
    IF user_active = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuario inactivo no puede reportar descubrimientos';
    END IF;
END$$
DELIMITER ;