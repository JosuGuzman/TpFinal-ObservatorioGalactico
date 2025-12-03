namespace Observatorio.Core.Entities.System;

public class UserFavorite
{
    public int FavoriteID { get; set; }
    public int UserID { get; set; }
    public ObjectType ObjectType { get; set; }
    public int ObjectID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public virtual User.User User { get; set; }
}