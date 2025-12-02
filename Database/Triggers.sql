USE BD_WatchTower;

DELIMITER $$

/* ============================================================
   TRIGGERS – WATCHTOWER
   ============================================================ */

/* ------------------------------------------------------------
   1) TRIGGERS PARA ARTÍCULOS – Generación y actualización de SLUG
   ------------------------------------------------------------ */

CREATE TRIGGER trg_articles_before_insert
BEFORE INSERT ON Articles
FOR EACH ROW
BEGIN
    IF NEW.Slug IS NULL OR NEW.Slug = '' THEN
        SET NEW.Slug = fn_slugify(NEW.Title);
    END IF;
END $$

CREATE TRIGGER trg_articles_before_update
BEFORE UPDATE ON Articles
FOR EACH ROW
BEGIN
    IF NEW.Title <> OLD.Title THEN
        SET NEW.Slug = fn_slugify(NEW.Title);
    END IF;
END $$

/* ------------------------------------------------------------
   2) Actualización automática del campo UpdatedAt
   ------------------------------------------------------------ */

CREATE TRIGGER trg_galaxies_update_timestamp
BEFORE UPDATE ON Galaxies
FOR EACH ROW
BEGIN
    SET NEW.UpdatedAt = NOW();
END $$

CREATE TRIGGER trg_stars_update_timestamp
BEFORE UPDATE ON Stars
FOR EACH ROW
BEGIN
    SET NEW.UpdatedAt = NOW();
END $$

CREATE TRIGGER trg_planets_update_timestamp
BEFORE UPDATE ON Planets
FOR EACH ROW
BEGIN
    SET NEW.UpdatedAt = NOW();
END $$

/* ------------------------------------------------------------
   3) Cálculo automático de Habitabilidad en Planetas
   ------------------------------------------------------------ */

CREATE TRIGGER trg_planets_before_insert
BEFORE INSERT ON Planets
FOR EACH ROW
BEGIN
    SET NEW.HabitabilityScore = fn_habitability(
        NEW.SurfaceTempK,
        NEW.OrbitalDistanceAU,
        NEW.MassEarth,
        NEW.RadiusEarth
    );
END $$

CREATE TRIGGER trg_planets_before_update
BEFORE UPDATE ON Planets
FOR EACH ROW
BEGIN
    SET NEW.HabitabilityScore = fn_habitability(
        NEW.SurfaceTempK,
        NEW.OrbitalDistanceAU,
        NEW.MassEarth,
        NEW.RadiusEarth
    );
END $$

/* ------------------------------------------------------------
   4) LOG automático de creación de galaxias
   ------------------------------------------------------------ */

CREATE TRIGGER trg_log_galaxy_insert
AFTER INSERT ON Galaxies
FOR EACH ROW
BEGIN
    INSERT INTO SystemLogs(UserID, EventType, Description, Status)
    VALUES(NULL, 'GalaxyInsert', CONCAT('Se creó la galaxia ', NEW.Name), 'OK');
END $$

/* ------------------------------------------------------------
   5) Prevención de corrupción de contador de visitas
   ------------------------------------------------------------ */

CREATE TRIGGER trg_article_view_count
BEFORE UPDATE ON Articles
FOR EACH ROW
BEGIN
    IF NEW.Views < OLD.Views THEN
        SET NEW.Views = OLD.Views;
    END IF;
END $$

/* ------------------------------------------------------------
   6) Limpieza automática cuando se elimina un usuario
   ------------------------------------------------------------ */

CREATE TRIGGER trg_user_before_delete
BEFORE DELETE ON Users
FOR EACH ROW
BEGIN
    DELETE FROM UserFavorites WHERE UserID = OLD.UserID;
    DELETE FROM SavedSearches WHERE UserID = OLD.UserID;
    DELETE FROM ExplorationHistory WHERE UserID = OLD.UserID;
    DELETE FROM DiscoveryVotes WHERE VoterUserID = OLD.UserID;
    DELETE FROM Discoveries WHERE ReporterUserID = OLD.UserID;
END $$

/* ------------------------------------------------------------
   7) Si la estrella cambia de galaxia, limpiar sus planetas
   ------------------------------------------------------------ */

CREATE TRIGGER trg_star_change_cleanup
BEFORE UPDATE ON Stars
FOR EACH ROW
BEGIN
    IF NEW.GalaxyID <> OLD.GalaxyID THEN
        DELETE FROM Planets WHERE StarID = OLD.StarID;
    END IF;
END $$

DELIMITER ;
