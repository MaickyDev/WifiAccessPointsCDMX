namespace WifiAccessPointsCDMX.Models
{
    public class ExcelAccessPointModel
    {
        public string Code { get; set; } = string.Empty;
        public string Program { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Alcaldia { get; set; } = string.Empty;
    }
}