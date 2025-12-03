namespace Observatorio.Mvc.Models.Account;

public class UserFavoriteViewModel
{
    public int FavoriteID { get; set; }
    public string ObjectType { get; set; }
    public int ObjectID { get; set; }
    public string ObjectName { get; set; }
    public DateTime CreatedAt { get; set; }
}