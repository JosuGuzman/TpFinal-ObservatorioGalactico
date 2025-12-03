namespace Observatorio.Core.Constants;

public static class AppConstants
{
    public const string ROLE_ADMIN = "Admin";
    public const string ROLE_ASTRONOMER = "Astronomo";
    public const string ROLE_RESEARCHER = "Investigador";
    public const string ROLE_VISITOR = "Visitante";
    
    public const string STATE_PENDING = "Pendiente";
    public const string STATE_APPROVED = "Aprobado";
    public const string STATE_REJECTED = "Rechazado";
    public const string STATE_PUBLISHED = "Publicado";
    public const string STATE_DRAFT = "Borrador";
    
    public const string TYPE_GALAXY = "Galaxia";
    public const string TYPE_STAR = "Estrella";
    public const string TYPE_PLANET = "Planeta";
    public const string TYPE_ARTICLE = "Articulo";
    public const string TYPE_EVENT = "Evento";
    public const string TYPE_DISCOVERY = "Descubrimiento";
    
    public const int DEFAULT_PAGE_SIZE = 20;
    public const int MAX_PAGE_SIZE = 100;
    public const int MAX_EXPORT_RECORDS = 1000;
    public const int API_RATE_LIMIT = 100;
    public const int PASSWORD_MIN_LENGTH = 8;
    public const int API_KEY_LENGTH = 64;
    public const int LOG_RETENTION_DAYS = 90;
    
    public const string NASA_API_BASE = "https://api.nasa.gov/";
    public const string OPEN_SKY_API = "https://opensky-network.org/api/";
    public const string SKYVIEW_API = "https://skyview.gsfc.nasa.gov/";
    
    public const string MSG_SUCCESS = "Operación completada exitosamente";
    public const string MSG_NOT_FOUND = "Recurso no encontrado";
    public const string MSG_UNAUTHORIZED = "Acceso no autorizado";
    public const string MSG_FORBIDDEN = "Acceso prohibido";
    public const string MSG_VALIDATION_ERROR = "Error de validación";
    public const string MSG_SERVER_ERROR = "Error interno del servidor";
    
    public const string DATE_FORMAT = "yyyy-MM-dd";
    public const string DATETIME_FORMAT = "yyyy-MM-dd HH:mm:ss";
    public const string API_DATE_FORMAT = "yyyy-MM-ddTHH:mm:ss";
    
    public const string MIME_JSON = "application/json";
    public const string MIME_CSV = "text/csv";
    public const string MIME_PDF = "application/pdf";
    public const string MIME_ZIP = "application/zip";
}