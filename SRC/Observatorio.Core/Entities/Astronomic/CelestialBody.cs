public abstract class CelestialBody
{
    public int Id { get; set; }
    public string Name { get; set; }
    public double RA { get; set; } // Right Ascension
    public double Dec { get; set; } // Declination
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    
    public virtual double CalculateAngularDistance(CelestialBody other)
    {
        double ra1 = RA * Math.PI / 180;
        double dec1 = Dec * Math.PI / 180;
        double ra2 = other.RA * Math.PI / 180;
        double dec2 = other.Dec * Math.PI / 180;
        
        return Math.Acos(
            Math.Sin(dec1) * Math.Sin(dec2) + 
            Math.Cos(dec1) * Math.Cos(dec2) * Math.Cos(ra1 - ra2)
        ) * 180 / Math.PI;
    }
}