using System.Text.Json.Serialization;

namespace WifiAccessPointsCDMX.Models
{
    public class AccessPointModel
    {
        public int Id { get; set; }
        public string Code { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        // Program properties
        [JsonIgnore]
        public int ProgramId { get; set; }
        public virtual ProgramModel? Program { get; set; }
        // Alcaldia properties
        [JsonIgnore]
        public int AlcaldiaId { get; set; }
        public virtual AlcaldiaModel? Alcaldia { get; set; }
    }
}
