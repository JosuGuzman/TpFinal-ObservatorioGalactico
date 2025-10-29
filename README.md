<h1 align="center"> E.T. Nº12 D.E. 1º "Libertador Gral. José de San Martín" </h1>
<p align="center">
  <img src="https://et12.edu.ar/imgs/et12.gif">
</p>

# Computación : 2025

**Asignatura**: Desarrollo de Sistemas

**Nombre TP**: Trabajo Final 

**Apellidos y Nombres Alumnos**: Josu Guzman - Hernan Vazquez

**Curso**: 6 ° 7

# 🌌 Observatorio El Paranal – Proyecto Final  

Aplicación web interactiva para la **exploración y estudio de galaxias, estrellas, planetas y fenómenos astronómicos**.  
El sistema integra datos reales de **APIs astronómicas (NASA, ESA, Hubble, JPL)** y ofrece herramientas de visualización y análisis científico.

## 🚀 Características


## 🧱 Estructura de Clases


## 📦 Requisitos


## ▶️ Ejecución


# Diagrama de Clases del Proyecto

```mermaid
erDiagram
    Users {
        INT UserId PK
        VARCHAR Username
        VARCHAR Email
        VARCHAR PasswordHash
        VARCHAR FirstName
        VARCHAR LastName
        ENUM Role
        TINYINT IsActive
        DATETIME CreatedAt
        DATETIME LastLogin
    }

    CelestialBodies {
        INT BodyId PK
        VARCHAR Name
        ENUM Type
        VARCHAR SubType
        VARCHAR Constellation
        VARCHAR RightAscension
        VARCHAR Declination
        DECIMAL Distance
        DECIMAL ApparentMagnitude
        DECIMAL AbsoluteMagnitude
        DECIMAL Mass
        DECIMAL Radius
        INT Temperature
        TEXT Description
        DATE DiscoveryDate
        VARCHAR NASA_ImageURL
        TINYINT IsVerified
        INT CreatedBy FK
        DATETIME CreatedAt
    }

    Discoveries {
        INT DiscoveryId PK
        VARCHAR Title
        TEXT Description
        VARCHAR Coordinates
        DATETIME DiscoveryDate
        INT ReportedBy FK
        INT CelestialBodyId FK
        ENUM Status
        JSON NASA_API_Data
        DATETIME CreatedAt
        DATETIME VerifiedAt
        INT VerifiedBy FK
    }

    Votes {
        INT VoteId PK
        INT DiscoveryId FK
        INT UserId FK
        ENUM VoteType
        DATETIME CreatedAt
    }

    ExplorationHistory {
        INT HistoryId PK
        INT UserId FK
        INT CelestialBodyId FK
        DATETIME VisitedAt
        INT TimeSpent
    }

    Articles {
        INT ArticleId PK
        VARCHAR Title
        TEXT Content
        VARCHAR Summary
        INT AuthorId FK
        ENUM Category
        TINYINT IsPublished
        DATETIME PublishedAt
        DATETIME CreatedAt
        DATETIME UpdatedAt
        INT ViewCount
    }

    Events {
        INT EventId PK
        VARCHAR Title
        TEXT Description
        ENUM EventType
        DATETIME StartDate
        DATETIME EndDate
        VARCHAR Location
        VARCHAR Visibility
        INT CreatedBy FK
        DATETIME CreatedAt
        TINYINT IsActive
    }

    Favorites {
        INT FavoriteId PK
        INT UserId FK
        INT CelestialBodyId FK
        INT ArticleId FK
        INT DiscoveryId FK
        DATETIME CreatedAt
    }

    Comments {
        INT CommentId PK
        TEXT Content
        INT UserId FK
        INT DiscoveryId FK
        INT ArticleId FK
        INT ParentCommentId FK
        DATETIME CreatedAt
        TINYINT IsActive
    }

    Users ||--o{ CelestialBodies : "CreatedBy"
    Users ||--o{ Discoveries : "ReportedBy"
    Users ||--o{ Discoveries : "VerifiedBy"
    Users ||--o{ ExplorationHistory : "explores"
    Users ||--o{ Articles : "writes"
    Users ||--o{ Events : "creates"
    Users ||--o{ Votes : "casts"
    Users ||--o{ Favorites : "saves"
    Users ||--o{ Comments : "writes"
    
    CelestialBodies ||--o{ Discoveries : "referenced_in"
    CelestialBodies ||--o{ ExplorationHistory : "visited_in"
    CelestialBodies }o--o{ Favorites : "favorited_as"
    
    Discoveries ||--o{ Votes : "receives"
    Discoveries }o--o{ Favorites : "favorited_as"
    Discoveries ||--o{ Comments : "commented_on"
    
    Articles }o--o{ Favorites : "favorited_as"
    Articles ||--o{ Comments : "commented_on"
    
    Comments }o--|| Comments : "replies_to"
```