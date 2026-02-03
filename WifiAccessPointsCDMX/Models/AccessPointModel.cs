namespace WifiAccessPointsCDMX.Models
{
    public class AccessPointModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // Program properties
        public int ProgramId { get; set; }
        public ProgramModel? Program { get; set; }
        // Alcaldia properties
        public int AlcaldiaId { get; set; }
        public AlcaldiaModel? Alcaldia { get; set; }

    }
}
