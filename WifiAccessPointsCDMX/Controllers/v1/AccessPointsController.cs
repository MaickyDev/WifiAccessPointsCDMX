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

        // Obtain a paginated list and the total number of access points
        [HttpGet]
        public async Task<IActionResult> GetPaged(int page = 1, int pageSize = 20)
        {
            var result = await _service.GetPagedAsync(page, pageSize);
            return Ok(result);
        }

        // Get an access point by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetByExternalId(string id)
        {
            var ap = await _service.GetByExternalIdAsync(id);

            if (ap is null)
                return NotFound();

            return Ok(ap);
        }

        // Obtain a paginated list and the total number of access points by alcaldia
        [HttpGet("alcaldia/{alcaldiaId}")]
        public async Task<IActionResult> GetByAlcaldia(int alcaldiaId, int page = 1, int pageSize = 20)
        {
            var result = await _service.GetByAlcaldiaAsync(alcaldiaId, page, pageSize);
            return Ok(result);
        }

        // Obtain a paginated list and the total points ordered by proximity to a coordinate.
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