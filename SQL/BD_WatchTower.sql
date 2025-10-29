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
-- Table `BD_WatchTower`.`Users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Users` (
  `UserId` INT NOT NULL AUTO_INCREMENT,
  `Username` VARCHAR(50) NOT NULL,
  `Email` VARCHAR(100) NOT NULL,
  `PasswordHash` VARCHAR(255) NOT NULL,
  `FirstName` VARCHAR(50) NULL,
  `LastName` VARCHAR(50) NULL,
  `Role` ENUM('Admin', 'Astronomer', 'Researcher', 'Visitor') NOT NULL DEFAULT 'Visitor',
  `IsActive` TINYINT NOT NULL DEFAULT 1,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `LastLogin` DATETIME NULL,
  PRIMARY KEY (`UserId`),
  UNIQUE INDEX `Username_UNIQUE` (`Username` ASC),
  UNIQUE INDEX `Email_UNIQUE` (`Email` ASC))
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`CelestialBodies`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`CelestialBodies` (
  `BodyId` INT NOT NULL AUTO_INCREMENT,
  `Name` VARCHAR(100) NOT NULL,
  `Type` ENUM('Galaxy', 'Star', 'Planet', 'Nebula', 'Cluster', 'Other') NOT NULL,
  `SubType` VARCHAR(50) NULL,
  `Constellation` VARCHAR(50) NULL,
  `RightAscension` VARCHAR(20) NULL,
  `Declination` VARCHAR(20) NULL,
  `Distance` DECIMAL(15,2) NULL COMMENT 'Distance in light years',
  `ApparentMagnitude` DECIMAL(5,2) NULL,
  `AbsoluteMagnitude` DECIMAL(5,2) NULL,
  `Mass` DECIMAL(20,5) NULL COMMENT 'Solar masses',
  `Radius` DECIMAL(15,5) NULL COMMENT 'Solar radii',
  `Temperature` INT NULL COMMENT 'Kelvin',
  `Description` TEXT NULL,
  `DiscoveryDate` DATE NULL,
  `NASA_ImageURL` VARCHAR(500) NULL,
  `IsVerified` TINYINT NOT NULL DEFAULT 0,
  `CreatedBy` INT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`BodyId`),
  INDEX `fk_CelestialBodies_Users1_idx` (`CreatedBy` ASC),
  CONSTRAINT `fk_CelestialBodies_Users1`
    FOREIGN KEY (`CreatedBy`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Discoveries`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Discoveries` (
  `DiscoveryId` INT NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(200) NOT NULL,
  `Description` TEXT NOT NULL,
  `Coordinates` VARCHAR(100) NULL,
  `DiscoveryDate` DATETIME NULL,
  `ReportedBy` INT NOT NULL,
  `CelestialBodyId` INT NULL,
  `Status` ENUM('Pending', 'Under Review', 'Verified', 'Rejected') NOT NULL DEFAULT 'Pending',
  `NASA_API_Data` JSON NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `VerifiedAt` DATETIME NULL,
  `VerifiedBy` INT NULL,
  PRIMARY KEY (`DiscoveryId`),
  INDEX `fk_Discoveries_Users1_idx` (`ReportedBy` ASC),
  INDEX `fk_Discoveries_CelestialBodies1_idx` (`CelestialBodyId` ASC),
  INDEX `fk_Discoveries_Users2_idx` (`VerifiedBy` ASC),
  CONSTRAINT `fk_Discoveries_Users1`
    FOREIGN KEY (`ReportedBy`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Discoveries_CelestialBodies1`
    FOREIGN KEY (`CelestialBodyId`)
    REFERENCES `BD_WatchTower`.`CelestialBodies` (`BodyId`)
    ON DELETE SET NULL
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Discoveries_Users2`
    FOREIGN KEY (`VerifiedBy`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE SET NULL
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Votes`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Votes` (
  `VoteId` INT NOT NULL AUTO_INCREMENT,
  `DiscoveryId` INT NOT NULL,
  `UserId` INT NOT NULL,
  `VoteType` ENUM('Up', 'Down') NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`VoteId`),
  INDEX `fk_Votes_Discoveries1_idx` (`DiscoveryId` ASC),
  INDEX `fk_Votes_Users1_idx` (`UserId` ASC),
  UNIQUE INDEX `Unique_Vote` (`DiscoveryId` ASC, `UserId` ASC),
  CONSTRAINT `fk_Votes_Discoveries1`
    FOREIGN KEY (`DiscoveryId`)
    REFERENCES `BD_WatchTower`.`Discoveries` (`DiscoveryId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Votes_Users1`
    FOREIGN KEY (`UserId`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`ExplorationHistory`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`ExplorationHistory` (
  `HistoryId` INT NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `CelestialBodyId` INT NOT NULL,
  `VisitedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `TimeSpent` INT NULL COMMENT 'Time spent in seconds',
  PRIMARY KEY (`HistoryId`),
  INDEX `fk_ExplorationHistory_Users1_idx` (`UserId` ASC),
  INDEX `fk_ExplorationHistory_CelestialBodies1_idx` (`CelestialBodyId` ASC),
  CONSTRAINT `fk_ExplorationHistory_Users1`
    FOREIGN KEY (`UserId`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_ExplorationHistory_CelestialBodies1`
    FOREIGN KEY (`CelestialBodyId`)
    REFERENCES `BD_WatchTower`.`CelestialBodies` (`BodyId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Articles`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Articles` (
  `ArticleId` INT NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(255) NOT NULL,
  `Content` TEXT NOT NULL,
  `Summary` VARCHAR(500) NULL,
  `AuthorId` INT NOT NULL,
  `Category` ENUM('News', 'Educational', 'Research', 'Event') NOT NULL,
  `IsPublished` TINYINT NOT NULL DEFAULT 0,
  `PublishedAt` DATETIME NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `UpdatedAt` DATETIME NULL,
  `ViewCount` INT NOT NULL DEFAULT 0,
  PRIMARY KEY (`ArticleId`),
  INDEX `fk_Articles_Users1_idx` (`AuthorId` ASC),
  CONSTRAINT `fk_Articles_Users1`
    FOREIGN KEY (`AuthorId`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Events`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Events` (
  `EventId` INT NOT NULL AUTO_INCREMENT,
  `Title` VARCHAR(255) NOT NULL,
  `Description` TEXT NULL,
  `EventType` ENUM('Eclipse', 'MeteorShower', 'Conjunction', 'Comet', 'Other') NOT NULL,
  `StartDate` DATETIME NOT NULL,
  `EndDate` DATETIME NULL,
  `Location` VARCHAR(100) NULL,
  `Visibility` VARCHAR(50) NULL,
  `CreatedBy` INT NOT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `IsActive` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`EventId`),
  INDEX `fk_Events_Users1_idx` (`CreatedBy` ASC),
  CONSTRAINT `fk_Events_Users1`
    FOREIGN KEY (`CreatedBy`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Favorites`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Favorites` (
  `FavoriteId` INT NOT NULL AUTO_INCREMENT,
  `UserId` INT NOT NULL,
  `CelestialBodyId` INT NULL,
  `ArticleId` INT NULL,
  `DiscoveryId` INT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`FavoriteId`),
  INDEX `fk_Favorites_Users1_idx` (`UserId` ASC),
  INDEX `fk_Favorites_CelestialBodies1_idx` (`CelestialBodyId` ASC),
  INDEX `fk_Favorites_Articles1_idx` (`ArticleId` ASC),
  INDEX `fk_Favorites_Discoveries1_idx` (`DiscoveryId` ASC),
  UNIQUE INDEX `Unique_Favorite` (`UserId` ASC, `CelestialBodyId` ASC, `ArticleId` ASC, `DiscoveryId` ASC),
  CONSTRAINT `fk_Favorites_Users1`
    FOREIGN KEY (`UserId`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Favorites_CelestialBodies1`
    FOREIGN KEY (`CelestialBodyId`)
    REFERENCES `BD_WatchTower`.`CelestialBodies` (`BodyId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Favorites_Articles1`
    FOREIGN KEY (`ArticleId`)
    REFERENCES `BD_WatchTower`.`Articles` (`ArticleId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Favorites_Discoveries1`
    FOREIGN KEY (`DiscoveryId`)
    REFERENCES `BD_WatchTower`.`Discoveries` (`DiscoveryId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

-- -----------------------------------------------------
-- Table `BD_WatchTower`.`Comments`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `BD_WatchTower`.`Comments` (
  `CommentId` INT NOT NULL AUTO_INCREMENT,
  `Content` TEXT NOT NULL,
  `UserId` INT NOT NULL,
  `DiscoveryId` INT NULL,
  `ArticleId` INT NULL,
  `ParentCommentId` INT NULL,
  `CreatedAt` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `IsActive` TINYINT NOT NULL DEFAULT 1,
  PRIMARY KEY (`CommentId`),
  INDEX `fk_Comments_Users1_idx` (`UserId` ASC),
  INDEX `fk_Comments_Discoveries1_idx` (`DiscoveryId` ASC),
  INDEX `fk_Comments_Articles1_idx` (`ArticleId` ASC),
  INDEX `fk_Comments_Comments1_idx` (`ParentCommentId` ASC),
  CONSTRAINT `fk_Comments_Users1`
    FOREIGN KEY (`UserId`)
    REFERENCES `BD_WatchTower`.`Users` (`UserId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Comments_Discoveries1`
    FOREIGN KEY (`DiscoveryId`)
    REFERENCES `BD_WatchTower`.`Discoveries` (`DiscoveryId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Comments_Articles1`
    FOREIGN KEY (`ArticleId`)
    REFERENCES `BD_WatchTower`.`Articles` (`ArticleId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE,
  CONSTRAINT `fk_Comments_Comments1`
    FOREIGN KEY (`ParentCommentId`)
    REFERENCES `BD_WatchTower`.`Comments` (`CommentId`)
    ON DELETE CASCADE
    ON UPDATE CASCADE)
ENGINE = InnoDB;

SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;