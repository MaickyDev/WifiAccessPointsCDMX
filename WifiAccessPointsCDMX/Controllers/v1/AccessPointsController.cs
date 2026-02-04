using Microsoft.AspNetCore.Mvc;
using WifiAccessPointsCDMX.Interfaces.Services;

namespace WifiAccessPointsCDMX.Controllers.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class AccessPointsController : ControllerBase
    {
        private readonly IAccessPointService _service;

        public AccessPointsController(IAccessPointService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetByExternalId(string id)
        {
            var ap = await _service.GetByExternalIdAsync(id);

            if (ap is null)
                return NotFound();

            return Ok(ap);
        }

        [HttpGet("alcaldia/{alcaldiaId}")]
        public async Task<IActionResult> GetByAlcaldia(int alcaldiaId, int page = 1, int pageSize = 20)
        {
            var result = await _service.GetByAlcaldiaAsync(alcaldiaId, page, pageSize);
            return Ok(result);
        }

        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearby(
            double latitude,
            double longitude,
            int page = 1,
            int pageSize = 20)
        {
            var result = await _service.GetNearbyAsync(latitude, longitude, page, pageSize);
            return Ok(result);
        }
    }
}
