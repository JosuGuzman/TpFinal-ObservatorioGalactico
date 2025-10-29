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
    Usuarios {
        INT IdUsuario PK
        VARCHAR NombreUsuario
        VARCHAR Email
        VARCHAR HashContraseña
        VARCHAR Nombre
        VARCHAR Apellido
        ENUM Rol
        TINYINT EstaActivo
        DATETIME CreadoEn
        DATETIME UltimoAcceso
    }

    CuerposCelestes {
        INT IdCuerpo PK
        VARCHAR Nombre
        ENUM Tipo
        VARCHAR Subtipo
        VARCHAR Constelacion
        VARCHAR AscensionRecta
        VARCHAR Declinacion
        DECIMAL Distancia
        DECIMAL MagnitudAparente
        DECIMAL MagnitudAbsoluta
        DECIMAL Masa
        DECIMAL Radio
        INT Temperatura
        TEXT Descripcion
        DATE FechaDescubrimiento
        VARCHAR URLImagenNASA
        TINYINT EstaVerificado
        INT CreadoPor FK
        DATETIME CreadoEn
    }

    Descubrimientos {
        INT IdDescubrimiento PK
        VARCHAR Titulo
        TEXT Descripcion
        VARCHAR Coordenadas
        DATETIME FechaDescubrimiento
        INT ReportadoPor FK
        INT IdCuerpoCeleste FK
        ENUM Estado
        JSON DatosAPI_NASA
        DATETIME CreadoEn
        DATETIME VerificadoEn
        INT VerificadoPor FK
    }

    Votos {
        INT IdVoto PK
        INT IdDescubrimiento FK
        INT IdUsuario FK
        ENUM TipoVoto
        DATETIME CreadoEn
    }

    HistorialExploracion {
        INT IdHistorial PK
        INT IdUsuario FK
        INT IdCuerpoCeleste FK
        DATETIME VisitadoEn
        INT TiempoDedicado
    }

    Articulos {
        INT IdArticulo PK
        VARCHAR Titulo
        TEXT Contenido
        VARCHAR Resumen
        INT IdAutor FK
        ENUM Categoria
        TINYINT EstaPublicado
        DATETIME PublicadoEn
        DATETIME CreadoEn
        DATETIME ActualizadoEn
        INT ContadorVistas
    }

    Eventos {
        INT IdEvento PK
        VARCHAR Titulo
        TEXT Descripcion
        ENUM TipoEvento
        DATETIME FechaInicio
        DATETIME FechaFin
        VARCHAR Ubicacion
        VARCHAR Visibilidad
        INT CreadoPor FK
        DATETIME CreadoEn
        TINYINT EstaActivo
    }

    Favoritos {
        INT IdFavorito PK
        INT IdUsuario FK
        INT IdCuerpoCeleste FK
        INT IdArticulo FK
        INT IdDescubrimiento FK
        DATETIME CreadoEn
    }

    Comentarios {
        INT IdComentario PK
        TEXT Contenido
        INT IdUsuario FK
        INT IdDescubrimiento FK
        INT IdArticulo FK
        INT IdComentarioPadre FK
        DATETIME CreadoEn
        TINYINT EstaActivo
    }

    Usuarios ||--o{ CuerposCelestes : "crea"
    Usuarios ||--o{ Descubrimientos : "reporta"
    Usuarios ||--o{ Descubrimientos : "verifica"
    Usuarios ||--o{ HistorialExploracion : "explora"
    Usuarios ||--o{ Articulos : "escribe"
    Usuarios ||--o{ Eventos : "crea"
    Usuarios ||--o{ Votos : "emite"
    Usuarios ||--o{ Favoritos : "guarda"
    Usuarios ||--o{ Comentarios : "escribe"
    
    CuerposCelestes ||--o{ Descubrimientos : "referenciado_en"
    CuerposCelestes ||--o{ HistorialExploracion : "visitado_en"
    CuerposCelestes }o--o{ Favoritos : "marcado_como_favorito"
    
    Descubrimientos ||--o{ Votos : "recibe"
    Descubrimientos }o--o{ Favoritos : "marcado_como_favorito"
    Descubrimientos ||--o{ Comentarios : "comentado_en"
    
    Articulos }o--o{ Favoritos : "marcado_como_favorito"
    Articulos ||--o{ Comentarios : "comentado_en"
    
    Comentarios }o--|| Comentarios : "responde_a"
```