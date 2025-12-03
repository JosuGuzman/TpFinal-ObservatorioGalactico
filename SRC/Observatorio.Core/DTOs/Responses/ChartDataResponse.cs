namespace Observatorio.Core.DTOs.Responses;

public class DateCountResponse
{
    public string Date { get; set; }
    public int Count { get; set; }
}

public class ChartDataResponse
{
    public List<DateCountResponse> UserActivity { get; set; }
    public List<TypeCountResponse> GalaxyTypes { get; set; }
    public List<TypeCountResponse> DiscoveryStates { get; set; }
    public List<MonthCountResponse> EventsByMonth { get; set; }
}

public class TypeCountResponse
{
    public string Type { get; set; }
    public int Count { get; set; }
}

public class MonthCountResponse
{
    public int Month { get; set; }
    public int Count { get; set; }
}