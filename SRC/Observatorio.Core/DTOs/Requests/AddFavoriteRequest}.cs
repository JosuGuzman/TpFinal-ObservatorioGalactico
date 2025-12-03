namespace Observatorio.Core.DTOs.Requests;

public class AddFavoriteRequest
{
    [Required]
    public string ObjectType { get; set; }

    [Required]
    public int ObjectID { get; set; }
}