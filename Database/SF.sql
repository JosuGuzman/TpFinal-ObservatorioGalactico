USE BD_WatchTower;

DELIMITER $$

/* ============================================================
   1) VALIDACIONES Y UTILIDADES
   ============================================================ */

-- Verifica si existe un usuario por correo
CREATE FUNCTION fn_user_exists(pEmail VARCHAR(255))
RETURNS TINYINT
DETERMINISTIC
BEGIN
    DECLARE vCount INT;
    SELECT COUNT(*) INTO vCount FROM Users WHERE Email = pEmail;
    RETURN vCount > 0;
END $$


-- Verifica si existe una galaxia
CREATE FUNCTION fn_galaxy_exists(pGalaxyID INT)
RETURNS TINYINT
DETERMINISTIC
BEGIN
    DECLARE vCount INT;
    SELECT COUNT(*) INTO vCount FROM Galaxies WHERE GalaxyID = pGalaxyID;
    RETURN vCount > 0;
END $$


-- Verifica si un planeta pertenece a una estrella dada
CREATE FUNCTION fn_planet_belongs_to_star(pPlanetID INT, pStarID INT)
RETURNS TINYINT
DETERMINISTIC
BEGIN
    DECLARE vCount INT;
    SELECT COUNT(*) INTO vCount
    FROM Planets
    WHERE PlanetID = pPlanetID AND StarID = pStarID;
    RETURN vCount > 0;
END $$


-- Verifica si un nombre ya existe en cualquier catálogo (evitar duplicados)
CREATE FUNCTION fn_name_exists(pName VARCHAR(200))
RETURNS TINYINT
DETERMINISTIC
BEGIN
    DECLARE vCount INT;

    SELECT (
        (SELECT COUNT(*) FROM Galaxies WHERE Name = pName) +
        (SELECT COUNT(*) FROM Stars WHERE Name = pName) +
        (SELECT COUNT(*) FROM Planets WHERE Name = pName)
    ) INTO vCount;

    RETURN vCount > 0;
END $$


/* ============================================================
   2) CÁLCULOS ASTRONÓMICOS
   ============================================================ */

-- Convierte grados a radianes
CREATE FUNCTION fn_deg2rad(pDeg DOUBLE)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    RETURN pDeg * PI() / 180;
END $$


-- Convierte radianes a grados
CREATE FUNCTION fn_rad2deg(pRad DOUBLE)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    RETURN pRad * 180 / PI();
END $$


-- Distancia angular entre dos puntos RA/Dec usando fórmula esférica
CREATE FUNCTION fn_angular_distance(
    pRA1 DOUBLE,
    pDec1 DOUBLE,
    pRA2 DOUBLE,
    pDec2 DOUBLE
)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    DECLARE ra1r DOUBLE;
    DECLARE dec1r DOUBLE;
    DECLARE ra2r DOUBLE;
    DECLARE dec2r DOUBLE;

    SET ra1r = fn_deg2rad(pRA1);
    SET dec1r = fn_deg2rad(pDec1);
    SET ra2r = fn_deg2rad(pRA2);
    SET dec2r = fn_deg2rad(pDec2);

    RETURN fn_rad2deg(
        ACOS(
            SIN(dec1r) * SIN(dec2r) +
            COS(dec1r) * COS(dec2r) * COS(ra1r - ra2r)
        )
    );
END $$


-- Cálculo de luminosidad relativa (generalizada)
CREATE FUNCTION fn_luminosity_ratio(pL1 DOUBLE, pL2 DOUBLE)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    IF pL2 = 0 THEN
        RETURN NULL;
    END IF;
    RETURN pL1 / pL2;
END $$


-- Cálculo de magnitud absoluta
CREATE FUNCTION fn_absolute_magnitude(pAppMag DOUBLE, pDistanceLy DOUBLE)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    IF pDistanceLy <= 0 THEN
        RETURN NULL;
    END IF;
    RETURN pAppMag - 5 * (LOG10(pDistanceLy * 0.306601) - 1);
END $$


/* ============================================================
   3) JSON HELPERS (útiles para catálogo y adjuntos)
   ============================================================ */

-- Agrega un string a un arreglo JSON
CREATE FUNCTION fn_json_push(pJson JSON, pValue VARCHAR(500))
RETURNS JSON
DETERMINISTIC
BEGIN
    RETURN JSON_ARRAY_APPEND(pJson, '$', pValue);
END $$

-- Extrae un elemento de un array JSON por índice
CREATE FUNCTION fn_json_get_index(pJson JSON, pIndex INT)
RETURNS VARCHAR(500)
DETERMINISTIC
BEGIN
    RETURN JSON_UNQUOTE(JSON_EXTRACT(pJson, CONCAT('$[', pIndex, ']')));
END $$

-- Cuenta elementos de un arreglo JSON
CREATE FUNCTION fn_json_count(pJson JSON)
RETURNS INT
DETERMINISTIC
BEGIN
    RETURN JSON_LENGTH(pJson);
END $$


/* ============================================================
   4) FUNCIONES PARA FAVORITOS, HISTORIAL Y DISCOVERY VOTES
   ============================================================ */

-- Devuelve cantidad de favoritos para un objeto
CREATE FUNCTION fn_favorites_count(pType VARCHAR(50), pObjectID INT)
RETURNS INT
DETERMINISTIC
BEGIN
    DECLARE v INT;
    SELECT COUNT(*) INTO v
    FROM UserFavorites
    WHERE ObjectType = pType AND ObjectID = pObjectID;
    RETURN v;
END $$

-- Devuelve total de votos positivos
CREATE FUNCTION fn_vote_up_count(pDiscoveryID INT)
RETURNS INT
DETERMINISTIC
BEGIN
    DECLARE c INT;
    SELECT COUNT(*) INTO c
    FROM DiscoveryVotes
    WHERE DiscoveryID = pDiscoveryID AND Vote = 1;
    RETURN c;
END $$

-- Devuelve total de votos negativos
CREATE FUNCTION fn_vote_down_count(pDiscoveryID INT)
RETURNS INT
DETERMINISTIC
BEGIN
    DECLARE c INT;
    SELECT COUNT(*) INTO c
    FROM DiscoveryVotes
    WHERE DiscoveryID = pDiscoveryID AND Vote = 0;
    RETURN c;
END $$


/* ============================================================
   5) FORMATEOS / STRING UTILITIES
   ============================================================ */

-- Slugify básico
CREATE FUNCTION fn_slugify(pText VARCHAR(300))
RETURNS VARCHAR(300)
DETERMINISTIC
BEGIN
    DECLARE s VARCHAR(300);

    SET s = LOWER(pText);
    SET s = REPLACE(s, ' ', '-');
    SET s = REPLACE(s, '_', '-');
    SET s = REPLACE(s, ',', '');
    SET s = REPLACE(s, '.', '');
    SET s = REPLACE(s, ':', '');
    SET s = REPLACE(s, ';', '');
    SET s = REPLACE(s, 'ñ', 'n');

    RETURN s;
END $$


-- Formatea RA/Dec en texto tipo “RA: 12.33°, DEC: -16.7°”
CREATE FUNCTION fn_format_coordinates(pRA DOUBLE, pDec DOUBLE)
RETURNS VARCHAR(100)
DETERMINISTIC
BEGIN
    RETURN CONCAT('RA: ', pRA, '°, DEC: ', pDec, '°');
END $$


/* ============================================================
   6) FUNCIONES SUPLEMENTARIAS ÚTILES PARA BACKEND
   ============================================================ */

-- Verifica si un texto coincide con un patrón de búsqueda (LIKE con comodines)
CREATE FUNCTION fn_match(pField VARCHAR(255), pQuery VARCHAR(255))
RETURNS TINYINT
DETERMINISTIC
BEGIN
    RETURN pField LIKE CONCAT('%', pQuery, '%');
END $$


-- Normaliza un valor null a 0
CREATE FUNCTION fn_nz(pValue DOUBLE)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    RETURN IFNULL(pValue, 0);
END $$


-- Calcular habitabilidad simplificada (normalización)
CREATE FUNCTION fn_habitability(
    pTemp DOUBLE,
    pDistanceAU DOUBLE,
    pMass DOUBLE,
    pRadius DOUBLE
)
RETURNS DOUBLE
DETERMINISTIC
BEGIN
    RETURN (
        (1 / (ABS(pTemp - 288) + 1)) * 0.4 +
        (1 / (ABS(pDistanceAU - 1) + 1)) * 0.4 +
        (1 / (ABS(pMass - 1) + 1)) * 0.1 +
        (1 / (ABS(pRadius - 1) + 1)) * 0.1
    );
END $$


-- Formatea magnitud absoluta con título y distancia
CREATE FUNCTION fn_format_magnitude(
    pAbsMag DOUBLE,
    pDistanceLy DOUBLE
)
RETURNS VARCHAR(200)
DETERMINISTIC
BEGIN
    RETURN CONCAT('Magnitud Absoluta: ', pAbsMag, ' (', pDistanceLy, ' ly)');
END $$


DELIMITER ;