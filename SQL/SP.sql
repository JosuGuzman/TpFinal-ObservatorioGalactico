USE `BD_WatchTower`;

-- Procedimiento para búsqueda en cuerpos celestes (para barra de búsqueda)
DELIMITER $$
CREATE PROCEDURE `SearchCelestialBodies`(
    IN search_term VARCHAR(100),
    IN body_type VARCHAR(50),
    IN max_distance DECIMAL(15,2),
    IN min_magnitude DECIMAL(5,2)
)
BEGIN
    SELECT 
        BodyId, 
        Name, 
        Type, 
        SubType,
        Constellation,
        Distance,
        ApparentMagnitude,
        Description,
        NASA_ImageURL
    FROM CelestialBodies 
    WHERE 
        (Name LIKE CONCAT('%', search_term, '%') 
        OR Description LIKE CONCAT('%', search_term, '%')
        OR Constellation LIKE CONCAT('%', search_term, '%'))
        AND (body_type IS NULL OR Type = body_type)
        AND (max_distance IS NULL OR Distance <= max_distance)
        AND (min_magnitude IS NULL OR ApparentMagnitude >= min_magnitude)
        AND IsVerified = 1
    ORDER BY 
        CASE 
            WHEN Name LIKE CONCAT(search_term, '%') THEN 1
            WHEN Constellation LIKE CONCAT(search_term, '%') THEN 2
            ELSE 3
        END,
        ApparentMagnitude ASC;
END$$
DELIMITER ;

-- Procedimiento para búsqueda en descubrimientos
DELIMITER $$
CREATE PROCEDURE `SearchDiscoveries`(
    IN search_term VARCHAR(100),
    IN status_filter VARCHAR(20),
    IN min_rating INT
)
BEGIN
    SELECT 
        d.DiscoveryId,
        d.Title,
        d.Description,
        d.Status,
        d.CreatedAt,
        u.Username AS ReportedBy,
        CalculateDiscoveryRating(d.DiscoveryId) AS Rating,
        cb.Name AS CelestialBodyName
    FROM Discoveries d
    INNER JOIN Users u ON d.ReportedBy = u.UserId
    LEFT JOIN CelestialBodies cb ON d.CelestialBodyId = cb.BodyId
    WHERE 
        (d.Title LIKE CONCAT('%', search_term, '%') 
        OR d.Description LIKE CONCAT('%', search_term, '%')
        OR cb.Name LIKE CONCAT('%', search_term, '%'))
        AND (status_filter IS NULL OR d.Status = status_filter)
    HAVING (min_rating IS NULL OR Rating >= min_rating)
    ORDER BY d.CreatedAt DESC;
END$$
DELIMITER ;

-- Procedimiento para búsqueda en artículos
DELIMITER $$
CREATE PROCEDURE `SearchArticles`(
    IN search_term VARCHAR(100),
    IN category_filter VARCHAR(20),
    IN published_only BOOLEAN
)
BEGIN
    SELECT 
        ArticleId,
        Title,
        Summary,
        Category,
        CreatedAt,
        ViewCount,
        (SELECT Username FROM Users WHERE UserId = AuthorId) AS AuthorName
    FROM Articles 
    WHERE 
        (Title LIKE CONCAT('%', search_term, '%') 
        OR Content LIKE CONCAT('%', search_term, '%')
        OR Summary LIKE CONCAT('%', search_term, '%'))
        AND (category_filter IS NULL OR Category = category_filter)
        AND (NOT published_only OR IsPublished = 1)
    ORDER BY 
        CASE 
            WHEN published_only THEN PublishedAt 
            ELSE CreatedAt 
        END DESC;
END$$
DELIMITER ;

-- Procedimiento para obtener estadísticas del usuario
DELIMITER $$
CREATE PROCEDURE `GetUserStatistics`(IN user_id INT)
BEGIN
    SELECT 
        u.Username,
        u.Role,
        u.CreatedAt,
        (SELECT COUNT(*) FROM Discoveries WHERE ReportedBy = user_id) AS TotalDiscoveries,
        (SELECT COUNT(*) FROM Discoveries WHERE ReportedBy = user_id AND Status = 'Verified') AS VerifiedDiscoveries,
        (SELECT COUNT(*) FROM ExplorationHistory WHERE UserId = user_id) AS ObjectsExplored,
        (SELECT COUNT(*) FROM Favorites WHERE UserId = user_id) AS TotalFavorites,
        (SELECT COUNT(*) FROM Comments WHERE UserId = user_id) AS TotalComments
    FROM Users u
    WHERE u.UserId = user_id;
END$$
DELIMITER ;

-- Procedimiento para agregar descubrimiento con validación
DELIMITER $$
CREATE PROCEDURE `AddDiscovery`(
    IN p_title VARCHAR(200),
    IN p_description TEXT,
    IN p_coordinates VARCHAR(100),
    IN p_reported_by INT,
    IN p_celestial_body_id INT
)
BEGIN
    DECLARE user_exists INT DEFAULT 0;
    DECLARE body_exists INT DEFAULT 0;
    
    -- Verificar que el usuario existe
    SELECT COUNT(*) INTO user_exists FROM Users WHERE UserId = p_reported_by;
    IF user_exists = 0 THEN
        SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Usuario no existe';
    END IF;
    
    -- Verificar que el cuerpo celeste existe (si se proporciona)
    IF p_celestial_body_id IS NOT NULL THEN
        SELECT COUNT(*) INTO body_exists FROM CelestialBodies WHERE BodyId = p_celestial_body_id;
        IF body_exists = 0 THEN
            SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Cuerpo celeste no existe';
        END IF;
    END IF;
    
    INSERT INTO Discoveries (Title, Description, Coordinates, ReportedBy, CelestialBodyId)
    VALUES (p_title, p_description, p_coordinates, p_reported_by, p_celestial_body_id);
    
    SELECT LAST_INSERT_ID() AS NewDiscoveryId;
END$$
DELIMITER ;