namespace Observatorio.Core.DTOs.Requests;

public class VoteDiscoveryRequest
{
    [Required]
    public bool Vote { get; set; }

    [StringLength(500)]
    public string Comment { get; set; }
}