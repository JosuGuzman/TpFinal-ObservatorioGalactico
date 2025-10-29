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
classDiagram
    direction TB

    class Users {
        +Int UserId (PK)
        +String Username (UNIQUE)
        +String Email (UNIQUE)
        +String PasswordHash
        +String FirstName
        +String LastName
        +Enum Role
        +Boolean IsActive
        +DateTime CreatedAt
        +DateTime LastLogin
    }

    class CelestialBodies {
        +Int BodyId (PK)
        +String Name
        +Enum Type
        +String SubType
        +String Constellation
        +String RightAscension
        +String Declination
        +Decimal Distance
        +Decimal ApparentMagnitude
        +Decimal AbsoluteMagnitude
        +Decimal Mass
        +Decimal Radius
        +Int Temperature
        +Text Description
        +Date DiscoveryDate
        +String NASA_ImageURL
        +Boolean IsVerified
        +Int CreatedBy (FK)
        +DateTime CreatedAt
    }

    class Discoveries {
        +Int DiscoveryId (PK)
        +String Title
        +Text Description
        +String Coordinates
        +DateTime DiscoveryDate
        +Int ReportedBy (FK)
        +Int CelestialBodyId (FK)
        +Enum Status
        +Json NASA_API_Data
        +DateTime CreatedAt
        +DateTime VerifiedAt
        +Int VerifiedBy (FK)
    }

    class Votes {
        +Int VoteId (PK)
        +Int DiscoveryId (FK)
        +Int UserId (FK)
        +Enum VoteType
        +DateTime CreatedAt
    }

    class ExplorationHistory {
        +Int HistoryId (PK)
        +Int UserId (FK)
        +Int CelestialBodyId (FK)
        +DateTime VisitedAt
        +Int TimeSpent
    }

    class Articles {
        +Int ArticleId (PK)
        +String Title
        +Text Content
        +String Summary
        +Int AuthorId (FK)
        +Enum Category
        +Boolean IsPublished
        +DateTime PublishedAt
        +DateTime CreatedAt
        +DateTime UpdatedAt
        +Int ViewCount
    }

    class Events {
        +Int EventId (PK)
        +String Title
        +Text Description
        +Enum EventType
        +DateTime StartDate
        +DateTime EndDate
        +String Location
        +String Visibility
        +Int CreatedBy (FK)
        +DateTime CreatedAt
        +Boolean IsActive
    }

    class Favorites {
        +Int FavoriteId (PK)
        +Int UserId (FK)
        +Int CelestialBodyId (FK)
        +Int ArticleId (FK)
        +Int DiscoveryId (FK)
        +DateTime CreatedAt
    }

    class Comments {
        +Int CommentId (PK)
        +Text Content
        +Int UserId (FK)
        +Int DiscoveryId (FK)
        +Int ArticleId (FK)
        +Int ParentCommentId (FK)
        +DateTime CreatedAt
        +Boolean IsActive
    }

    -- Relationships
    Users "1" -- "0..*" CelestialBodies : Creates (CreatedBy)
    Users "1" -- "0..*" Discoveries : Reports (ReportedBy)
    Users "0..1" -- "0..*" Discoveries : Verifies (VerifiedBy)
    Users "1" -- "0..*" Votes : Casts
    Users "1" -- "0..*" ExplorationHistory : Has
    Users "1" -- "0..*" Articles : Authors (AuthorId)
    Users "1" -- "0..*" Events : Creates (CreatedBy)
    Users "1" -- "0..*" Favorites : Has
    Users "1" -- "0..*" Comments : Writes

    CelestialBodies "0..1" -- "0..*" Discoveries : Linked to
    CelestialBodies "1" -- "0..*" ExplorationHistory : Is Explored
    CelestialBodies "0..1" -- "0..*" Favorites : Is Favorited

    Discoveries "1" -- "0..*" Votes : Receives
    Discoveries "0..1" -- "0..*" Favorites : Is Favorited
    Discoveries "0..1" -- "0..*" Comments : Has

    Articles "0..1" -- "0..*" Favorites : Is Favorited
    Articles "0..1" -- "0..*" Comments : Has

    Comments "0..1" -- "0..*" Comments : Replies to (Parent)
```