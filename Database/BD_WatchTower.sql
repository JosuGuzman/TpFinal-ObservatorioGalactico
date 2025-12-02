-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema BD_WatchTower
-- -----------------------------------------------------
DROP SCHEMA IF EXISTS `BD_WatchTower` ;
CREATE SCHEMA IF NOT EXISTS `BD_WatchTower` DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci ;
USE `BD_WatchTower` ;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Roles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Roles (
  RoleID SMALLINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  RoleName VARCHAR(50) NOT NULL UNIQUE,
  Description VARCHAR(255)
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Usuario`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Users (
  UserID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  Email VARCHAR(255) NOT NULL UNIQUE,
  UserName VARCHAR(100) NOT NULL,
  PasswordHash VARCHAR(255) NOT NULL, -- bcrypt hash (cost adecuado)
  IsActive TINYINT(1) NOT NULL DEFAULT 1,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  LastLogin DATETIME NULL,
  RoleID SMALLINT UNSIGNED NOT NULL,
  ApiKey VARCHAR(64) NULL, -- para API per-user (si aplica)
  CONSTRAINT fk_users_role FOREIGN KEY (RoleID) REFERENCES Roles(RoleID)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

-- Index para búsquedas por usuario o email
CREATE INDEX idx_users_email ON Users (Email);
CREATE INDEX idx_users_username ON Users (UserName);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Galaxias`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Galaxies (
  GalaxyID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  Name VARCHAR(200) NOT NULL,
  Type ENUM('Espiral','Eliptica','Irregular','Lenticular') NOT NULL,
  DistanceLy DOUBLE NOT NULL, -- años luz
  ApparentMagnitude FLOAT NULL,
  RA DOUBLE NOT NULL, -- Ascensión recta (grados) 0-360
  Dec DOUBLE NOT NULL, -- Declinación (grados) -90 a 90
  Redshift DOUBLE NULL,
  Description TEXT,
  ImageURLs JSON NULL, -- array JSON de URLs
  Discoverer VARCHAR(200) NULL,
  YearDiscovery YEAR NULL,
  References JSON NULL, -- array JSON de enlaces
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  -- Generated POINT column for spatial queries (lat/lon-like using RA/Dec)
  Coordinates POINT AS (ST_PointFromText(CONCAT('POINT(', RA, ' ', Dec, ')'))) PERSISTENT,
  SPATIAL INDEX sp_idx_galaxies_coords (Coordinates),
  UNIQUE KEY uq_galaxy_name (Name)
) ENGINE=InnoDB
  ROW_FORMAT=DYNAMIC;

-- Índices adicionales para queries frecuentes
CREATE INDEX idx_galaxies_type_distance ON Galaxies(Type, DistanceLy);
CREATE FULLTEXT INDEX ft_galaxies_name_desc ON Galaxies(Name, Description);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Estrellas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Stars (
  StarID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  GalaxyID INT UNSIGNED NULL,
  Name VARCHAR(200) NOT NULL,
  SpectralType VARCHAR(10) NULL, -- O,B,A,F,G,K,M
  SurfaceTempK INT NULL,
  MassSolar DOUBLE NULL,
  RadiusSolar DOUBLE NULL,
  LuminositySolar DOUBLE NULL,
  DistanceLy DOUBLE NULL,
  RA DOUBLE NOT NULL,
  Dec DOUBLE NOT NULL,
  RadialVelocity DOUBLE NULL,
  ApparentMagnitude FLOAT NULL,
  EstimatedAgeMillionYears DOUBLE NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Coordinates POINT AS (ST_PointFromText(CONCAT('POINT(', RA, ' ', Dec, ')'))) PERSISTENT,
  SPATIAL INDEX sp_idx_stars_coords (Coordinates),
  CONSTRAINT fk_stars_galaxy FOREIGN KEY (GalaxyID) REFERENCES Galaxies(GalaxyID)
    ON UPDATE CASCADE ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE INDEX idx_stars_sptype ON Stars (SpectralType);
CREATE FULLTEXT INDEX ft_stars_name ON Stars (Name);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Planetas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Planets (
  PlanetID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  StarID INT UNSIGNED NOT NULL,
  Name VARCHAR(200) NOT NULL,
  PlanetType ENUM('Terrestre','Gigante gas','Enana hielo','Otro') DEFAULT 'Otro',
  MassEarth DOUBLE NULL,
  RadiusEarth DOUBLE NULL,
  OrbitalPeriodDays DOUBLE NULL,
  OrbitalDistanceAU DOUBLE NULL,
  Eccentricity DOUBLE NULL,
  HabitabilityScore DOUBLE NULL, -- índice calculable
  Atmosphere JSON NULL,
  SurfaceTempK DOUBLE NULL,
  DiscoveryDate DATE NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_planet_star FOREIGN KEY (StarID) REFERENCES Stars(StarID)
    ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE INDEX idx_planets_star ON Planets (StarID);
CREATE FULLTEXT INDEX ft_planets_name ON Planets (Name);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Descubrimientos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Discoveries (
  DiscoveryID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  ReporterUserID INT UNSIGNED NOT NULL,
  ObjectType ENUM('Galaxia','Estrella','Planeta','Otro') NOT NULL,
  SuggestedName VARCHAR(250) NULL,
  RA DOUBLE NOT NULL,
  Dec DOUBLE NOT NULL,
  Description TEXT,
  Attachments JSON NULL,
  State ENUM('Pendiente','ValidacionComunitaria','RevisadoAstronomo','Aprobado','Rechazado') NOT NULL DEFAULT 'Pendiente',
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  Coordinates POINT AS (ST_PointFromText(CONCAT('POINT(', RA, ' ', Dec, ')'))) PERSISTENT,
  SPATIAL INDEX sp_idx_discoveries_coords (Coordinates),
  CONSTRAINT fk_discoveries_reporter FOREIGN KEY (ReporterUserID) REFERENCES Users(UserID)
    ON UPDATE CASCADE ON DELETE CASCADE
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Votaciones`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS DiscoveryVotes (
  VoteID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  DiscoveryID INT UNSIGNED NOT NULL,
  VoterUserID INT UNSIGNED NOT NULL,
  Vote TINYINT(1) NOT NULL, -- 1 upvote, 0 downvote
  Comment VARCHAR(500) NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_vote_discovery FOREIGN KEY (DiscoveryID) REFERENCES Discoveries(DiscoveryID)
    ON UPDATE CASCADE ON DELETE CASCADE,
  CONSTRAINT fk_vote_user FOREIGN KEY (VoterUserID) REFERENCES Users(UserID)
    ON UPDATE CASCADE ON DELETE CASCADE,
  UNIQUE KEY uq_vote_unique (DiscoveryID, VoterUserID)
) ENGINE=InnoDB;

CREATE INDEX idx_votes_discovery ON DiscoveryVotes (DiscoveryID);

-- Helper: view with vote counts (rápido para dashboard)
CREATE OR REPLACE VIEW vw_discovery_votes AS
SELECT d.DiscoveryID, d.SuggestedName, d.State,
  SUM(CASE WHEN v.Vote=1 THEN 1 ELSE 0 END) AS upvotes,
  SUM(CASE WHEN v.Vote=0 THEN 1 ELSE 0 END) AS downvotes,
  COUNT(v.VoteID) AS total_votes
FROM Discoveries d
LEFT JOIN DiscoveryVotes v ON v.DiscoveryID = d.DiscoveryID
GROUP BY d.DiscoveryID;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Articulos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Articles (
  ArticleID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  Title VARCHAR(300) NOT NULL,
  Slug VARCHAR(300) NOT NULL UNIQUE,
  Content LONGTEXT NOT NULL, -- texto enriquecido
  AuthorUserID INT UNSIGNED NOT NULL,
  State ENUM('Draft','Published','Archived') NOT NULL DEFAULT 'Draft',
  Tags JSON NULL,
  FeaturedImage VARCHAR(500) NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PublishedAt DATETIME NULL,
  UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_article_author FOREIGN KEY (AuthorUserID) REFERENCES Users(UserID)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE FULLTEXT INDEX ft_articles_title_content ON Articles(Title, Content);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Eventos Astronómicos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS Events (
  EventID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  Name VARCHAR(250) NOT NULL,
  Type ENUM('Eclipse','Lluvia meteoros','Conjuncion','Otro') NOT NULL,
  EventDate DATETIME NOT NULL,
  Description TEXT NULL,
  Visibility JSON NULL, -- regiones/coords de visibilidad
  Coordinates JSON NULL, -- lista de coords relevantes
  DurationMinutes INT NULL,
  Resources JSON NULL,
  CreatedByUserID INT UNSIGNED NOT NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  UpdatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  CONSTRAINT fk_event_creator FOREIGN KEY (CreatedByUserID) REFERENCES Users(UserID)
    ON UPDATE CASCADE ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE INDEX idx_events_eventdate ON Events (EventDate);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Favoritos`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS UserFavorites (
  FavoriteID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  UserID INT UNSIGNED NOT NULL,
  ObjectType ENUM('Galaxy','Star','Planet','Article','Event','Discovery','Other') NOT NULL,
  ObjectID INT UNSIGNED NOT NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_fav_user FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB;
CREATE INDEX idx_favorites_user ON UserFavorites(UserID);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Busquedas`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS SavedSearches (
  SavedSearchID INT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  UserID INT UNSIGNED NOT NULL,
  Name VARCHAR(200) NOT NULL,
  Criteria JSON NOT NULL,
  CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  CONSTRAINT fk_saved_user FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Historial`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS ExplorationHistory (
  HistoryID BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  UserID INT UNSIGNED NOT NULL,
  ObjectType ENUM('Galaxy','Star','Planet','Article','Event','Discovery','Other') NOT NULL,
  ObjectID INT UNSIGNED NOT NULL,
  AccessedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  DurationSeconds INT UNSIGNED NULL,
  SearchCriteria JSON NULL,
  CONSTRAINT fk_history_user FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE CASCADE
) ENGINE=InnoDB;
CREATE INDEX idx_history_user_time ON ExplorationHistory (UserID, AccessedAt);

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`System logs`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS SystemLogs (
  LogID BIGINT UNSIGNED PRIMARY KEY AUTO_INCREMENT,
  UserID INT UNSIGNED NULL,
  EventType VARCHAR(50) NOT NULL,
  Description TEXT,
  Timestamp DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  IPAddress VARCHAR(45) NULL,
  Status VARCHAR(20) NULL,
  -- For faster queries on recent logs, index timestamp
  INDEX idx_systemlogs_time (Timestamp),
  CONSTRAINT fk_logs_user FOREIGN KEY (UserID) REFERENCES Users(UserID) ON DELETE SET NULL
) ENGINE=InnoDB
  PARTITION BY RANGE ( TO_DAYS(Timestamp) ) (
    PARTITION p0 VALUES LESS THAN (TO_DAYS('2024-01-01')),
    PARTITION p_recent VALUES LESS THAN (TO_DAYS(NOW()) + 365000) -- default large bucket; manage partitions with maintenance
  );