namespace Observatorio.Core.Helpers;

public static class ValidationHelpers
{
    public static bool ValidateCoordinates(double ra, double dec) => ra >= 0 && ra <= 360 && dec >= -90 && dec <= 90;
    public static bool ValidateDistance(double distance) => distance >= 0;
    public static bool ValidateTemperature(double temperature) => temperature > 0;
    public static bool ValidateMass(double mass) => mass > 0;
    public static bool ValidateRadius(double radius) => radius > 0;
    public static bool ValidateOrbitalPeriod(double period) => period > 0;
    public static bool ValidateOrbitalDistance(double distance) => distance > 0;
    public static bool ValidateEccentricity(double eccentricity) => eccentricity >= 0 && eccentricity < 1;

    public static List<string> ValidateAstronomicalData(
        double? ra, double? dec, double? distance, double? temperature,
        double? mass, double? radius, double? orbitalPeriod, double? orbitalDistance)
    {
        var errors = new List<string>();

        if (ra.HasValue && !ValidateCoordinates(ra.Value, dec ?? 0))
            errors.Add("Ascensión recta inválida (debe estar entre 0 y 360)");
        
        if (dec.HasValue && !ValidateCoordinates(ra ?? 0, dec.Value))
            errors.Add("Declinación inválida (debe estar entre -90 y 90)");
        
        if (distance.HasValue && !ValidateDistance(distance.Value))
            errors.Add("Distancia inválida (debe ser mayor a 0)");
        
        if (temperature.HasValue && !ValidateTemperature(temperature.Value))
            errors.Add("Temperatura inválida (debe ser mayor a 0)");
        
        if (mass.HasValue && !ValidateMass(mass.Value))
            errors.Add("Masa inválida (debe ser mayor a 0)");
        
        if (radius.HasValue && !ValidateRadius(radius.Value))
            errors.Add("Radio inválida (debe ser mayor a 0)");
        
        if (orbitalPeriod.HasValue && !ValidateOrbitalPeriod(orbitalPeriod.Value))
            errors.Add("Período orbital inválido (debe ser mayor a 0)");
        
        if (orbitalDistance.HasValue && !ValidateOrbitalDistance(orbitalDistance.Value))
            errors.Add("Distancia orbital inválida (debe ser mayor a 0)");

        return errors;
    }

    public static bool IsValidUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return false;

        return Uri.TryCreate(url, UriKind.Absolute, out Uri uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}