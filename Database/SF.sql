USE `BD_WatchTower`;

-- Función para calcular el rating de un descubrimiento
DELIMITER $$
CREATE FUNCTION `CalculateDiscoveryRating`(discovery_id INT) 
RETURNS INT
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE up_votes INT;
    DECLARE down_votes INT;
    
    SELECT COUNT(*) INTO up_votes FROM Votes 
    WHERE DiscoveryId = discovery_id AND VoteType = 'Up';
    
    SELECT COUNT(*) INTO down_votes FROM Votes 
    WHERE DiscoveryId = discovery_id AND VoteType = 'Down';
    
    RETURN up_votes - down_votes;
END$$
DELIMITER ;

-- Función para contar descubrimientos de un usuario
DELIMITER $$
CREATE FUNCTION `CountUserDiscoveries`(user_id INT) 
RETURNS INT
READS SQL DATA
DETERMINISTIC
BEGIN
    DECLARE discovery_count INT;
    
    SELECT COUNT(*) INTO discovery_count FROM Discoveries 
    WHERE ReportedBy = user_id AND Status = 'Verified';
    
    RETURN discovery_count;
END$$
DELIMITER ;

-- Función para verificar si un cuerpo celeste es visible desde la Tierra
DELIMITER $$
CREATE FUNCTION `IsVisibleFromEarth`(magnitude DECIMAL(5,2)) 
RETURNS VARCHAR(3)
DETERMINISTIC
BEGIN
    IF magnitude <= 6.5 THEN
        RETURN 'Yes';
    ELSE
        RETURN 'No';
    END IF;
END$$
DELIMITER ;

-- Función para categorizar la distancia
DELIMITER $$
CREATE FUNCTION `CategorizeDistance`(distance DECIMAL(15,2)) 
RETURNS VARCHAR(20)
DETERMINISTIC
BEGIN
    RETURN CASE 
        WHEN distance < 100 THEN 'Very Close'
        WHEN distance < 1000 THEN 'Close'
        WHEN distance < 100000 THEN 'Distant'
        WHEN distance < 1000000 THEN 'Very Distant'
        ELSE 'Extremely Distant'
    END;
END$$
DELIMITER ;