USE BD_WatchTower;

-- =====================================================================
-- 1. GALAXIAS (usando CALLs)
-- =====================================================================
CALL sp_insert_galaxy('Vía Láctea', 'Espiral', 0, -20.5, 266.405, -29.006, 'Galaxia local.');
CALL sp_insert_galaxy('Andrómeda', 'Espiral', 2537000, 3.44, 10.684, 41.269, 'Galaxia vecina.');
CALL sp_insert_galaxy('Triángulo', 'Espiral', 3000000, 5.72, 23.462, 30.660, 'Miembro del Grupo Local.');
CALL sp_insert_galaxy('Centaurus-A', 'Eliptica', 13000000, 6.84, 201.365, -43.019, 'Activa y masiva.');
CALL sp_insert_galaxy('SagDEG', 'Irregular', 70000, 4.5, 283.830, -30.550, 'Galaxia satélite pequeña.');

-- =====================================================================
-- 2. ESTRELLAS (usando CALLs)
-- =====================================================================
CALL sp_insert_star(1, 'Sol', 'G2V', 5778, 1.00, 1.00, 1.0, 0.0000158, 0, 0);
CALL sp_insert_star(1, 'Helios-X', 'G1', 5900, 1.10, 1.05, 1.2, 100, 10.0, 20.0);
CALL sp_insert_star(2, 'Andros-17', 'F5', 6500, 1.30, 1.20, 1.5, 2537000, 10.684, 41.269);
CALL sp_insert_star(3, 'Tri-Star-09', 'K4V', 4200, 0.70, 0.80, 0.3, 3000000, 23.462, 30.660);
CALL sp_insert_star(1, 'Nova-S1', 'G0', 6000, 1.20, 1.10, 1.3, 200, 15.0, 25.0);

-- =====================================================================
-- 3. PLANETAS (usando CALLs)
-- =====================================================================
CALL sp_insert_planet(1, 'Tierra', 'Terrestre', 1.00, 1.00, 365.25, 1.00, 0.0167, 0.75);
CALL sp_insert_planet(1, 'Marte', 'Terrestre', 0.11, 0.53, 686.971, 1.52, 0.0934, 0.45);
CALL sp_insert_planet(2, 'Gaia-X', 'Terrestre', 1.20, 1.10, 365, 1.05, 0.01, 0.82);
CALL sp_insert_planet(3, 'Andros-B1', 'Terrestre', 1.80, 1.20, 400, 1.10, 0.02, 0.78);
CALL sp_insert_planet(4, 'Tri-Prime', 'Terrestre', 2.10, 1.40, 300, 0.90, 0.03, 0.65);
CALL sp_insert_planet(2, 'Kepler-X', 'Terrestre', 1.50, 1.20, 365, 1.10, 0.015, 0.80);

-- =====================================================================
-- 4. USUARIOS (INSERT directo - no hay SP para esto)
-- =====================================================================
INSERT INTO Roles (RoleName, Description) VALUES 
('Admin', 'Administrador del sistema'),
('User', 'Usuario regular');

INSERT INTO Users (UserName, Email, PasswordHash, RoleID) VALUES
('admin', 'admin@watchtower.com', 'HASH_ADMIN', 1),
('explorer1', 'exp1@watchtower.com', 'HASH1', 2),
('observer77', 'obs77@watchtower.com', 'HASH2', 2);

-- =====================================================================
-- 5. ARTÍCULOS (usando CALLs)
-- =====================================================================
CALL sp_create_article('La formación de galaxias', 'formacion-galaxias', 'Contenido técnico sobre formación galáctica...', 1);
CALL sp_create_article('Exoplanetas y habitabilidad', 'exoplanetas-habitabilidad', 'Datos y análisis sobre exoplanetas habitables...', 1);
CALL sp_create_article('Nuevas técnicas de detección', 'tecnicas-deteccion', 'Detalles del método de detección avanzada...', 2);
CALL sp_create_article('Impacto de las supernovas', 'impacto-supernovas', 'Contenido sobre efectos de supernovas...', 1);

-- =====================================================================
-- 6. EVENTOS (usando CALLs)
-- =====================================================================
CALL sp_create_event('Lluvia de Meteoros Perseidas', 'Lluvia meteoros', '2024-08-12 22:00:00', 'Máximo de las Perseidas este año', 1);
CALL sp_create_event('Eclipse Lunar Total', 'Eclipse', '2024-03-25 03:00:00', 'Eclipse lunar visible en América', 1);
CALL sp_create_event('Conjunción Venus-Júpiter', 'Conjuncion', '2024-07-15 05:30:00', 'Aproximación cercana de planetas', 2);
CALL sp_create_event('Oposición de Marte', 'Otro', '2024-12-08 00:00:00', 'Marte en su punto más cercano a la Tierra', 1);

-- =====================================================================
-- 7. FAVORITOS (usando CALLs)
-- =====================================================================
CALL sp_add_favorite(2, 'Planet', 1);
CALL sp_add_favorite(2, 'Planet', 3);
CALL sp_add_favorite(3, 'Planet', 2);
CALL sp_add_favorite(2, 'Planet', 5);

-- =====================================================================
-- 8. BÚSQUEDAS GUARDADAS (usando CALLs)
-- =====================================================================
CALL sp_save_search(2, 'planetas habitables', JSON_OBJECT('minHabitability', 0.7, 'type', 'Terrestre'));
CALL sp_save_search(3, 'galaxias espirales', JSON_OBJECT('type', 'Espiral', 'maxDistance', 10000000));
CALL sp_save_search(2, 'estrellas cercanas', JSON_OBJECT('maxDistance', 50, 'spectralType', 'G'));
CALL sp_save_search(3, 'eventos 2024', JSON_OBJECT('year', 2024, 'types', JSON_ARRAY('Eclipse', 'Lluvia meteoros')));

-- =====================================================================
-- 9. HISTORIAL (usando CALLs)
-- =====================================================================
CALL sp_add_history(2, 'Planet', 1, 3600, JSON_OBJECT('search', 'tierra', 'notes', 'Exploración inicial'));
CALL sp_add_history(3, 'Planet', 3, 5400, JSON_OBJECT('search', 'gaia-x', 'notes', 'Análisis atmosférico detectado'));
CALL sp_add_history(2, 'Galaxy', 2, 4200, JSON_OBJECT('search', 'andromeda'));
CALL sp_add_history(3, 'Star', 1, 3000, JSON_OBJECT('search', 'sol'));

-- =====================================================================
-- 10. DESCUBRIMIENTOS (usando CALLs)
-- =====================================================================
CALL sp_create_discovery(2, 'Planeta', 'Atmósfera densa', 10.5, 20.3, 'Se detectó espectro compatible con atmósfera densa en Gaia-X');
CALL sp_create_discovery(3, 'Planeta', 'Anomalía térmica', 15.2, -10.8, 'Calor interno elevado detectado en Tri-Prime');
CALL sp_create_discovery(2, 'Estrella', 'Nova Cygni 2024', 299.59, 40.40, 'Posible nova en la constelación de Cygnus');
CALL sp_create_discovery(3, 'Galaxia', 'Enana Sculptor II', 15.25, -33.90, 'Posible galaxia enana satélite de la Vía Láctea');

-- =====================================================================
-- 11. VOTOS (INSERT directo - no hay SP para esto)
-- =====================================================================
-- Primero actualizamos el estado de algunos descubrimientos
CALL sp_update_discovery_status(1, 'ValidacionComunitaria');
CALL sp_update_discovery_status(2, 'ValidacionComunitaria');

-- Insertamos votos
INSERT INTO DiscoveryVotes (DiscoveryID, VoterUserID, Vote, Comment) VALUES
(1, 3, 1, 'Excelente descubrimiento'),
(2, 2, 1, 'Muy interesante'),
(1, 1, 1, 'Confirmado por datos adicionales'),
(2, 1, 0, 'Necesita más evidencia');

-- =====================================================================
-- 12. LOGS (usando CALLs)
-- =====================================================================
CALL sp_add_log(1, 'Init', 'Sistema iniciado', '192.168.1.100', 'OK');
CALL sp_add_log(2, 'Login', 'explorer1 inició sesión', '192.168.1.101', 'OK');
CALL sp_add_log(NULL, 'AutoScan', 'Exploración automática realizada', '192.168.1.1', 'OK');
CALL sp_add_log(1, 'PruebaManual', 'Test de inserción via SP', '192.168.1.100', 'OK');
CALL sp_add_log(3, 'Discovery', 'Nuevo descubrimiento reportado', '192.168.1.102', 'OK');

-- =====================================================================
-- 13. PRUEBAS DE ACTUALIZACIÓN (usando CALLs)
-- =====================================================================
-- Actualizar una galaxia
CALL sp_update_galaxy(5, 'Sagitarius Dwarf', 'Irregular', 65000, 4.8, 283.760, -30.480, 'Galaxia enana elíptica satélite de la Vía Láctea');

-- Actualizar una estrella
CALL sp_update_star(5, 'Nova-S1 Updated', 'G0V', 6100, 1.25, 1.15, 1.4, 210, 15.5, 25.5);

-- Actualizar un planeta
CALL sp_update_planet(6, 'Kepler-X Updated', 'Terrestre', 1.55, 1.25, 370, 1.12, 0.02, 0.85);

-- Actualizar un artículo
CALL sp_update_article(1, 'La Formación y Evolución de Galaxias', 'Contenido técnico ampliado sobre formación y evolución galáctica...', 'Published');

-- Actualizar un evento
CALL sp_update_event(1, 'Lluvia de Meteoros Perseidas 2024', 'Lluvia meteoros', '2024-08-12 23:00:00', 'Máximo de las Perseidas - mejor visibilidad este año');

-- Actualizar estado de descubrimiento
CALL sp_update_discovery_status(3, 'RevisadoAstronomo');
CALL sp_update_discovery_status(4, 'Aprobado');

-- =====================================================================
-- 14. PRUEBAS DE CONSULTA (usando CALLs)
-- =====================================================================
-- Nota: Estas son solo demostraciones, no insertan datos
-- Para probar, puedes ejecutarlas por separado:

-- CALL sp_get_galaxy_by_id(1);
-- CALL sp_get_star_by_id(1);
-- CALL sp_get_planet_by_id(1);
-- CALL sp_get_discovery_by_id(1);
-- CALL sp_search_fulltext('galaxia', 10, 0);
-- CALL sp_objects_nearby(10.0, 20.0, 5.0, 10);

-- =====================================================================
-- 15. PRUEBAS DE ELIMINACIÓN (usando CALLs - cuidadoso)
-- =====================================================================
-- NOTA: Descomenta solo si quieres probar eliminaciones
/*
-- Eliminar un favorito (necesitas saber el FavoriteID primero)
-- CALL sp_delete_favorite(1);

-- Eliminar una búsqueda guardada
-- CALL sp_delete_saved_search(1);

-- Eliminar un artículo
-- CALL sp_delete_article(4);

-- Eliminar un evento
-- CALL sp_delete_event(4);

-- Eliminar un planeta
-- CALL sp_delete_planet(6);

-- Eliminar una estrella (eliminará sus planetas también)
-- CALL sp_delete_star(5);

-- Eliminar una galaxia (eliminará sus estrellas y planetas también)
-- CALL sp_delete_galaxy(5);

-- Eliminar un descubrimiento (eliminará sus votos también)
-- CALL sp_delete_discovery(4);
*/

-- =====================================================================
-- 16. VERIFICACIÓN DE DATOS INSERTADOS
-- =====================================================================
SELECT '=== VERIFICACIÓN DE DATOS INSERTADOS ===' AS Status;
SELECT 
    'Galaxias' AS Tabla, 
    COUNT(*) AS Registros 
FROM Galaxies
UNION ALL
SELECT 'Estrellas', COUNT(*) FROM Stars
UNION ALL
SELECT 'Planetas', COUNT(*) FROM Planets
UNION ALL
SELECT 'Usuarios', COUNT(*) FROM Users
UNION ALL
SELECT 'Artículos', COUNT(*) FROM Articles
UNION ALL
SELECT 'Eventos', COUNT(*) FROM Events
UNION ALL
SELECT 'Favoritos', COUNT(*) FROM UserFavorites
UNION ALL
SELECT 'Búsquedas', COUNT(*) FROM SavedSearches
UNION ALL
SELECT 'Historial', COUNT(*) FROM ExplorationHistory
UNION ALL
SELECT 'Descubrimientos', COUNT(*) FROM Discoveries
UNION ALL
SELECT 'Votos', COUNT(*) FROM DiscoveryVotes
UNION ALL
SELECT 'Logs', COUNT(*) FROM SystemLogs;

SELECT '=== EJEMPLOS DE DATOS ===' AS Status;
SELECT 'Galaxias (primeras 3):' AS Tipo, GalaxyID, Name, Type, DistanceLy FROM Galaxies LIMIT 3;
SELECT 'Estrellas (primeras 3):' AS Tipo, StarID, Name, SpectralType, DistanceLy FROM Stars LIMIT 3;
SELECT 'Planetas (primeras 3):' AS Tipo, PlanetID, Name, PlanetType, OrbitalDistanceAU FROM Planets LIMIT 3;
SELECT 'Descubrimientos:' AS Tipo, DiscoveryID, SuggestedName, State FROM Discoveries;
SELECT 'Artículos:' AS Tipo, ArticleID, Title, State FROM Articles;

SELECT '=== DATOS INSERTADOS CORRECTAMENTE ===' AS Final_Status;