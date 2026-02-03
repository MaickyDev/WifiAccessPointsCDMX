namespace WifiAccessPointsCDMX.Models
{
    public class AlcaldiaModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<AccessPointModel> AccessPoints { get; set; } = new List<AccessPointModel>();

    }
}