namespace Observatorio.Core.DTOs.Requests;

public class CreateEventRequest
{
    [Required, StringLength(250)]
    public string Name { get; set; }

    [Required]
    public string Type { get; set; }

    [Required]
    public DateTime EventDate { get; set; }

    public string Description { get; set; }

    public string Visibility { get; set; }

    public string Coordinates { get; set; }

    [Range(1, 10080)]
    public int? DurationMinutes { get; set; }

    public string Resources { get; set; }
}