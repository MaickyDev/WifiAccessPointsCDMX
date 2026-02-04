using WifiAccessPointsCDMX.Models;
using WifiAccessPointsCDMX.Interfaces;
using WifiAccessPointsCDMX.Models.Dto;
using WifiAccessPointsCDMX.Interfaces.Services;

namespace WifiAccessPointsCDMX.Services
{
    public class AccessPointService : IAccessPointService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<AccessPointService> _logger;

        public AccessPointService(IUnitOfWork uow, ILogger<AccessPointService> logger)
        {
            _uow = uow;
            _logger = logger;
        }

        public async Task<PagedResult<AccessPointModel>> GetPagedAsync(int page, int pageSize)
        {
            try
            {
                var skip = (page - 1) * pageSize;

                var total = await _uow.AccessPoints.CountAsync();
                var items = await _uow.AccessPoints.GetPagedAsync(skip, pageSize);

                return new PagedResult<AccessPointModel> { Total = total, Page = page, PageSize = pageSize, Items = items };
            }
            catch (Exception ex) 
            { 
                _logger.LogError(ex, "Error while getting access points. Page={Page}, PageSize={PageSize}", page, pageSize); 
                throw;
            }
        }

        public async Task<PagedResult<AccessPointModel>> GetByExternalIdAsync(string externalId)
        {
            try
            {
                var item = await _uow.AccessPoints.GetByExternalIdAsync(externalId);

                if (item is null)
                {
                    return new PagedResult<AccessPointModel>
                    {
                        Total = 0,
                        Page = 1,
                        PageSize = 1,
                        Items = Enumerable.Empty<AccessPointModel>()
                    };
                }
                return new PagedResult<AccessPointModel> { Total = 1, Page = 1, PageSize = 1, Items = new List<AccessPointModel> { item } };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting access points by Id. Id={Id}", externalId);
                throw;
            }
        }

        public async Task<PagedResult<AccessPointModel>> GetByAlcaldiaAsync(int alcaldiaId, int page, int pageSize)
        {
            try
            {
                var skip = (page - 1) * pageSize;

                var total = await _uow.AccessPoints.CountByAlcaldiaAsync(alcaldiaId);
                var items = await _uow.AccessPoints.GetPagedByAlcaldiaAsync(alcaldiaId, skip, pageSize);

                return new PagedResult<AccessPointModel> { Total = total, Page = page, PageSize = pageSize, Items = items };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting access points by alcaldia. AlcaldiaId={AlcaldiaId}, Page={Page}, PageSize={PageSize}", alcaldiaId, page, pageSize);
                throw;
            }
        }

        public async Task<PagedResult<AccessPointModel>> GetNearbyAsync(double lat, double lng, int page, int pageSize)
        {
            try
            {
                var skip = (page - 1) * pageSize;

                var total = await _uow.AccessPoints.CountAsync();
                var items = await _uow.AccessPoints.GetNearbyPagedAsync(lat, lng, skip, pageSize);

                return new PagedResult<AccessPointModel>
                {
                    Total = total,
                    Page = page,
                    PageSize = pageSize,
                    Items = items
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting access points by nearly. Latitud={Lat}, Longitude={Lng} Page={Page}, PageSize={PageSize}", lat, lng, page, pageSize);
                throw;
            }
        }
    }
}
