namespace WifiAccessPointsCDMX.Interfaces.Services
{
    public interface IExcelService
    {
        Task ImportExcelAsync(IFormFile file);
    }
}
