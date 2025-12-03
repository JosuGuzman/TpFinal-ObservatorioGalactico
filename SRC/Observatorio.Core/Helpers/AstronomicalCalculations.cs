namespace Observatorio.Core.Helpers;

public static class AstronomicalCalculations
{
    public static double Deg2Rad(double deg) => deg * Math.PI / 180.0;
    public static double Rad2Deg(double rad) => rad * 180.0 / Math.PI;

    public static double AngularDistance(double ra1, double dec1, double ra2, double dec2)
    {
        double ra1r = Deg2Rad(ra1);
        double dec1r = Deg2Rad(dec1);
        double ra2r = Deg2Rad(ra2);
        double dec2r = Deg2Rad(dec2);

        double cosAngDist = Math.Sin(dec1r) * Math.Sin(dec2r) +
                           Math.Cos(dec1r) * Math.Cos(dec2r) * Math.Cos(ra1r - ra2r);

        cosAngDist = Math.Max(-1.0, Math.Min(1.0, cosAngDist));
        return Rad2Deg(Math.Acos(cosAngDist));
    }

    public static double CalculateHabitability(double temp, double distanceAU, double mass, double radius)
    {
        const double idealTemp = 288;
        const double idealDistance = 1.0;
        const double idealMass = 1.0;
        const double idealRadius = 1.0;

        double tempScore = 1.0 / (Math.Abs(temp - idealTemp) + 1);
        double distScore = 1.0 / (Math.Abs(distanceAU - idealDistance) + 1);
        double massScore = 1.0 / (Math.Abs(mass - idealMass) + 1);
        double radiusScore = 1.0 / (Math.Abs(radius - idealRadius) + 1);

        return (tempScore * 0.4) + (distScore * 0.4) + (massScore * 0.1) + (radiusScore * 0.1);
    }

    public static double AbsoluteMagnitude(double apparentMagnitude, double distanceLy)
    {
        if (distanceLy <= 0)
            throw new ArgumentException("La distancia debe ser mayor a 0");

        return apparentMagnitude - 5 * (Math.Log10(distanceLy * 0.306601) - 1);
    }

    public static double ConvertParsecsToLightYears(double parsecs) => parsecs * 3.26156;
    public static double ConvertLightYearsToParsecs(double lightYears) => lightYears / 3.26156;

    public static string FormatCoordinates(double ra, double dec) => $"RA: {ra:0.000}°, DEC: {dec:0.000}°";
    public static bool IsValidRA(double ra) => ra >= 0 && ra <= 360;
    public static bool IsValidDec(double dec) => dec >= -90 && dec <= 90;
    public static bool IsValidCoordinates(double ra, double dec) => IsValidRA(ra) && IsValidDec(dec);
}