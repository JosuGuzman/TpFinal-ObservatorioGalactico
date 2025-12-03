namespace Observatorio.Core.Constants;

public static class ErrorMessages
{
    public const string INVALID_CREDENTIALS = "Correo electrónico o contraseña no válidos";
    public const string ACCOUNT_INACTIVE = "La cuenta está inactiva";
    public const string EMAIL_EXISTS = "El correo electrónico ya existe";
    public const string USERNAME_EXISTS = "El nombre de usuario ya existe";
    public const string INVALID_TOKEN = "Token no válido o caducado";
    public const string INVALID_API_KEY = "Clave API no válida";
    
    public const string REQUIRED_FIELD = "{0} es obligatorio";
    public const string INVALID_EMAIL = "Formato de correo electrónico no válido";
    public const string PASSWORD_TOO_WEAK = "La contraseña es demasiado débil";
    public const string PASSWORDS_DONT_MATCH = "Las contraseñas no coinciden";
    public const string INVALID_COORDINATES = "Coordenadas no válidas";
    public const string INVALID_DISTANCE = "La distancia debe ser positiva";
    public const string INVALID_TEMPERATURE = "La temperatura debe ser positiva";
    
    public const string ACCESS_DENIED = "No tienes permiso para acceder a este recurso";
    public const string ADMIN_ONLY = "Esta acción requiere privilegios de administrador";
    public const string ASTRONOMER_ONLY = "Esta acción requiere privilegios de astrónomo";
    public const string RESEARCHER_ONLY = "Esta acción requiere privilegios de investigador";
    
    public const string GALAXY_NOT_FOUND = "Galaxia no encontrada";
    public const string STAR_NOT_FOUND = "Estrella no encontrada";
    public const string PLANET_NOT_FOUND = "Planeta no encontrado";
    public const string USER_NOT_FOUND = "Usuario no encontrado";
    public const string DISCOVERY_NOT_FOUND = "Descubrimiento no encontrado";
    public const string ARTICLE_NOT_FOUND = "Artículo no encontrado";
    public const string EVENT_NOT_FOUND = "Evento no encontrado";
    
    public const string ALREADY_VOTED = "Ya has votado sobre este descubrimiento";
    public const string ALREADY_FAVORITED = "Este elemento ya está en tus favoritos";
    public const string NOT_FAVORITED = "Este elemento no está entre tus favoritos";
    public const string CANNOT_DELETE = "No se puede eliminar este recurso";
    public const string CANNOT_UPDATE = "No se puede actualizar este recurso";
    
    public const string DATABASE_ERROR = "Se produjo un error en la base de datos";
    public const string EXTERNAL_API_ERROR = "Error al llamar a la API externa";
    public const string FILE_UPLOAD_ERROR = "Error al cargar el archivo";
    public const string EXPORT_ERROR = "Error al exportar datos";
}