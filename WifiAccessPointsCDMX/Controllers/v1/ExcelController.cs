using Microsoft.AspNetCore.Mvc;
using WifiAccessPointsCDMX.Interfaces.Services;

namespace WifiAccessPointsCDMX.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExcelController : ControllerBase
    {
        private readonly IExcelService _service;

        public ExcelController(IExcelService service)
        {
            _service = service;
        }

        // Import Excel file information of access points into database
        [HttpPost("import")]
        public async Task<IActionResult> Import(IFormFile file)
        {
            await _service.ImportExcelAsync(file);
            return Ok("Excel imported successfully");
        }
    }
}