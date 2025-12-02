USE BD_WatchTower;

DELIMITER $$

/* ============================================================
   1) USUARIOS Y AUTENTICACIÓN
   ============================================================ */

CREATE PROCEDURE sp_register_user(
    IN pEmail VARCHAR(255),
    IN pUserName VARCHAR(100),
    IN pPasswordHash VARCHAR(255),
    IN pRoleID SMALLINT,
    OUT pUserID INT
)
BEGIN
    INSERT INTO Users (Email, UserName, PasswordHash, RoleID)
    VALUES (pEmail, pUserName, pPasswordHash, pRoleID);
    SET pUserID = LAST_INSERT_ID();
END $$

CREATE PROCEDURE sp_login(IN pEmail VARCHAR(255))
BEGIN
    SELECT * FROM Users WHERE Email = pEmail;
END $$

CREATE PROCEDURE sp_update_last_login(IN pUserID INT)
BEGIN
    UPDATE Users SET LastLogin = NOW() WHERE UserID = pUserID;
END $$

/* ============================================================
   2) GALAXIAS (CRUD COMPLETO)
   ============================================================ */

-- ALTA
CREATE PROCEDURE sp_insert_galaxy(
    IN pName VARCHAR(200),
    IN pType VARCHAR(50),
    IN pDistance DOUBLE,
    IN pMag FLOAT,
    IN pRA DOUBLE,
    IN pDec DOUBLE,
    IN pDescription TEXT
)
BEGIN
    INSERT INTO Galaxies(Name, Type, DistanceLy, ApparentMagnitude, RA, Dec, Description)
    VALUES(pName, pType, pDistance, pMag, pRA, pDec, pDescription);
END $$

-- UPDATE
CREATE PROCEDURE sp_update_galaxy(
    IN pGalaxyID INT,
    IN pName VARCHAR(200),
    IN pType VARCHAR(50),
    IN pDistance DOUBLE,
    IN pMag FLOAT,
    IN pRA DOUBLE,
    IN pDec DOUBLE,
    IN pDescription TEXT
)
BEGIN
    UPDATE Galaxies
    SET Name = pName,
        Type = pType,
        DistanceLy = pDistance,
        ApparentMagnitude = pMag,
        RA = pRA,
        Dec = pDec,
        Description = pDescription
    WHERE GalaxyID = pGalaxyID;
END $$

-- BORRADO CON CASCADA MANUAL (Estrellas + Planetas)
CREATE PROCEDURE sp_delete_galaxy(IN pGalaxyID INT)
BEGIN
    DELETE FROM Planets
    WHERE StarID IN (SELECT StarID FROM Stars WHERE GalaxyID = pGalaxyID);

    DELETE FROM Stars
    WHERE GalaxyID = pGalaxyID;

    DELETE FROM Galaxies
    WHERE GalaxyID = pGalaxyID;
END $$

-- GET
CREATE PROCEDURE sp_get_galaxy_by_id(IN pGalaxyID INT)
BEGIN
    SELECT * FROM Galaxies WHERE GalaxyID = pGalaxyID;
END $$

/* ============================================================
   3) ESTRELLAS (CRUD COMPLETO)
   ============================================================ */

-- ALTA
CREATE PROCEDURE sp_insert_star(
    IN pGalaxyID INT,
    IN pName VARCHAR(200),
    IN pSpectral VARCHAR(10),
    IN pTemp INT,
    IN pMass DOUBLE,
    IN pRadius DOUBLE,
    IN pLum DOUBLE,
    IN pDistance DOUBLE,
    IN pRA DOUBLE,
    IN pDec DOUBLE
)
BEGIN
    INSERT INTO Stars (GalaxyID, Name, SpectralType, SurfaceTempK, MassSolar, RadiusSolar,
                       LuminositySolar, DistanceLy, RA, Dec)
    VALUES(pGalaxyID, pName, pSpectral, pTemp, pMass, pRadius, pLum, pDistance, pRA, pDec);
END $$

-- UPDATE
CREATE PROCEDURE sp_update_star(
    IN pStarID INT,
    IN pName VARCHAR(200),
    IN pSpectral VARCHAR(10),
    IN pTemp INT,
    IN pMass DOUBLE,
    IN pRadius DOUBLE,
    IN pLum DOUBLE,
    IN pDistance DOUBLE,
    IN pRA DOUBLE,
    IN pDec DOUBLE
)
BEGIN
    UPDATE Stars
    SET Name = pName,
        SpectralType = pSpectral,
        SurfaceTempK = pTemp,
        MassSolar = pMass,
        RadiusSolar = pRadius,
        LuminositySolar = pLum,
        DistanceLy = pDistance,
        RA = pRA,
        Dec = pDec
    WHERE StarID = pStarID;
END $$

-- BORRADO CON CASCADA MANUAL (Planetas)
CREATE PROCEDURE sp_delete_star(IN pStarID INT)
BEGIN
    DELETE FROM Planets WHERE StarID = pStarID;
    DELETE FROM Stars WHERE StarID = pStarID;
END $$

-- GET
CREATE PROCEDURE sp_get_star_by_id(IN pStarID INT)
BEGIN
    SELECT * FROM Stars WHERE StarID = pStarID;
END $$

/* ============================================================
   4) PLANETAS (CRUD COMPLETO)
   ============================================================ */

-- ALTA
CREATE PROCEDURE sp_insert_planet(
    IN pStarID INT,
    IN pName VARCHAR(200),
    IN pType VARCHAR(50),
    IN pMass DOUBLE,
    IN pRadius DOUBLE,
    IN pPeriod DOUBLE,
    IN pDistance DOUBLE,
    IN pEcc DOUBLE,
    IN pHabit DOUBLE
)
BEGIN
    INSERT INTO Planets(StarID, Name, PlanetType, MassEarth, RadiusEarth, OrbitalPeriodDays,
                        OrbitalDistanceAU, Eccentricity, HabitabilityScore)
    VALUES(pStarID, pName, pType, pMass, pRadius, pPeriod, pDistance, pEcc, pHabit);
END $$

-- UPDATE
CREATE PROCEDURE sp_update_planet(
    IN pPlanetID INT,
    IN pName VARCHAR(200),
    IN pType VARCHAR(50),
    IN pMass DOUBLE,
    IN pRadius DOUBLE,
    IN pPeriod DOUBLE,
    IN pDistance DOUBLE,
    IN pEcc DOUBLE,
    IN pHabit DOUBLE
)
BEGIN
    UPDATE Planets
    SET Name = pName,
        PlanetType = pType,
        MassEarth = pMass,
        RadiusEarth = pRadius,
        OrbitalPeriodDays = pPeriod,
        OrbitalDistanceAU = pDistance,
        Eccentricity = pEcc,
        HabitabilityScore = pHabit
    WHERE PlanetID = pPlanetID;
END $$

-- BORRADO
CREATE PROCEDURE sp_delete_planet(IN pPlanetID INT)
BEGIN
    DELETE FROM Planets WHERE PlanetID = pPlanetID;
END $$

-- GET
CREATE PROCEDURE sp_get_planet_by_id(IN pPlanetID INT)
BEGIN
    SELECT * FROM Planets WHERE PlanetID = pPlanetID;
END $$

/* ============================================================
   5) DESCUBRIMIENTOS Y VOTOS
   ============================================================ */

-- ALTA
CREATE PROCEDURE sp_create_discovery(
    IN pUserID INT,
    IN pObjectType VARCHAR(50),
    IN pName VARCHAR(200),
    IN pRA DOUBLE,
    IN pDec DOUBLE,
    IN pDescription TEXT
)
BEGIN
    INSERT INTO Discoveries(ReporterUserID, ObjectType, SuggestedName, RA, Dec, Description)
    VALUES(pUserID, pObjectType, pName, pRA, pDec, pDescription);
END $$

-- UPDATE ESTADO
CREATE PROCEDURE sp_update_discovery_status(IN pDiscoveryID INT, IN pState VARCHAR(50))
BEGIN
    UPDATE Discoveries SET State = pState WHERE DiscoveryID = pDiscoveryID;
END $$

-- BORRADO CON CASCADA MANUAL (votos relacionados)
CREATE PROCEDURE sp_delete_discovery(IN pDiscoveryID INT)
BEGIN
    DELETE FROM DiscoveryVotes WHERE DiscoveryID = pDiscoveryID;
    DELETE FROM Discoveries WHERE DiscoveryID = pDiscoveryID;
END $$

-- GET DETAIL
CREATE PROCEDURE sp_get_discovery_by_id(IN pDiscoveryID INT)
BEGIN
    SELECT d.*, 
           COALESCE(v.upvotes,0) AS Upvotes,
           COALESCE(v.downvotes,0) AS Downvotes
    FROM Discoveries d
    LEFT JOIN vw_discovery_votes v ON v.DiscoveryID = d.DiscoveryID
    WHERE d.DiscoveryID = pDiscoveryID;
END $$

/* ============================================================
   6) ARTÍCULOS
   ============================================================ */

CREATE PROCEDURE sp_create_article(
    IN pTitle VARCHAR(300),
    IN pSlug VARCHAR(300),
    IN pContent LONGTEXT,
    IN pUserID INT
)
BEGIN
    INSERT INTO Articles (Title, Slug, Content, AuthorUserID)
    VALUES(pTitle, pSlug, pContent, pUserID);
END $$

CREATE PROCEDURE sp_update_article(
    IN pArticleID INT,
    IN pTitle VARCHAR(300),
    IN pContent LONGTEXT,
    IN pState VARCHAR(30)
)
BEGIN
    UPDATE Articles
    SET Title = pTitle,
        Content = pContent,
        State = pState
    WHERE ArticleID = pArticleID;
END $$

CREATE PROCEDURE sp_delete_article(IN pArticleID INT)
BEGIN
    DELETE FROM Articles WHERE ArticleID = pArticleID;
END $$

/* ============================================================
   7) EVENTOS
   ============================================================ */

CREATE PROCEDURE sp_create_event(
    IN pName VARCHAR(250),
    IN pType VARCHAR(50),
    IN pDate DATETIME,
    IN pDesc TEXT,
    IN pUserID INT
)
BEGIN
    INSERT INTO Events(Name, Type, EventDate, Description, CreatedByUserID)
    VALUES(pName, pType, pDate, pDesc, pUserID);
END $$

CREATE PROCEDURE sp_update_event(
    IN pEventID INT,
    IN pName VARCHAR(250),
    IN pType VARCHAR(50),
    IN pDate DATETIME,
    IN pDesc TEXT
)
BEGIN
    UPDATE Events
    SET Name = pName,
        Type = pType,
        EventDate = pDate,
        Description = pDesc
    WHERE EventID = pEventID;
END $$

CREATE PROCEDURE sp_delete_event(IN pEventID INT)
BEGIN
    DELETE FROM Events WHERE EventID = pEventID;
END $$

/* ============================================================
   8) FAVORITOS
   ============================================================ */

CREATE PROCEDURE sp_add_favorite(
    IN pUserID INT,
    IN pType VARCHAR(50),
    IN pObjectID INT
)
BEGIN
    INSERT INTO UserFavorites(UserID, ObjectType, ObjectID)
    VALUES(pUserID, pType, pObjectID);
END $$

CREATE PROCEDURE sp_delete_favorite(IN pFavoriteID INT)
BEGIN
    DELETE FROM UserFavorites WHERE FavoriteID = pFavoriteID;
END $$

/* ============================================================
   9) BÚSQUEDAS GUARDADAS
   ============================================================ */

CREATE PROCEDURE sp_save_search(
    IN pUserID INT,
    IN pName VARCHAR(200),
    IN pCriteria JSON
)
BEGIN
    INSERT INTO SavedSearches(UserID, Name, Criteria)
    VALUES(pUserID, pName, pCriteria);
END $$

CREATE PROCEDURE sp_delete_saved_search(IN pSearchID INT)
BEGIN
    DELETE FROM SavedSearches WHERE SavedSearchID = pSearchID;
END $$

/* ============================================================
   10) HISTORIAL
   ============================================================ */

CREATE PROCEDURE sp_add_history(
    IN pUserID INT,
    IN pType VARCHAR(50),
    IN pObjectID INT,
    IN pDuration INT,
    IN pCriteria JSON
)
BEGIN
    INSERT INTO ExplorationHistory(UserID, ObjectType, ObjectID, DurationSeconds, SearchCriteria)
    VALUES(pUserID, pType, pObjectID, pDuration, pCriteria);
END $$

/* ============================================================
   11) LOGS
   ============================================================ */

CREATE PROCEDURE sp_add_log(
    IN pUserID INT,
    IN pEvent VARCHAR(50),
    IN pDesc TEXT,
    IN pIP VARCHAR(45),
    IN pStatus VARCHAR(20)
)
BEGIN
    INSERT INTO SystemLogs(UserID, EventType, Description, IPAddress, Status)
    VALUES(pUserID, pEvent, pDesc, pIP, pStatus);
END $$

/* ============================================================
   12) BÚSQUEDA AVANZADA
   ============================================================ */

CREATE PROCEDURE sp_search_fulltext(
    IN pQuery TEXT,
    IN pLimit INT,
    IN pOffset INT
)
BEGIN
    SELECT 'Galaxy' AS Type, GalaxyID AS ID, Name AS Label
    FROM Galaxies
    WHERE MATCH(Name, Description) AGAINST(pQuery IN NATURAL LANGUAGE MODE)

    UNION ALL
    SELECT 'Star', StarID, Name
    FROM Stars
    WHERE MATCH(Name) AGAINST(pQuery)

    UNION ALL
    SELECT 'Planet', PlanetID, Name
    FROM Planets
    WHERE MATCH(Name) AGAINST(pQuery)

    LIMIT pLimit OFFSET pOffset;
END $$

/* ============================================================
   13) OBJETOS CERCANOS
   ============================================================ */

CREATE PROCEDURE sp_objects_nearby(
    IN pRA DOUBLE,
    IN pDec DOUBLE,
    IN pRadius DOUBLE,
    IN pLimit INT
)
BEGIN
    SELECT 'Galaxy' AS Type, GalaxyID AS ID, Name, RA, Dec,
           SQRT(POW(RA - pRA,2) + POW(Dec - pDec,2)) AS Distance
    FROM Galaxies
    HAVING Distance <= pRadius

    UNION ALL

    SELECT 'Star', StarID, Name, RA, Dec,
           SQRT(POW(RA - pRA,2) + POW(Dec - pDec,2)) AS Distance
    FROM Stars
    HAVING Distance <= pRadius

    ORDER BY Distance ASC
    LIMIT pLimit;
END $$

DELIMITER ;