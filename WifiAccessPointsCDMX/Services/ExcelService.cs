using OfficeOpenXml;
using FluentValidation;
using WifiAccessPointsCDMX.Models;
using WifiAccessPointsCDMX.Helpers;
using WifiAccessPointsCDMX.Interfaces;
using WifiAccessPointsCDMX.Interfaces.Services;

namespace WifiAccessPointsCDMX.Services
{
    public class ExcelService : IExcelService
    {
        private readonly IUnitOfWork _uow;
        private readonly ILogger<ExcelService> _logger;
        private readonly IValidator<ExcelAccessPointModel> _validator;

        public ExcelService(IUnitOfWork uow, ILogger<ExcelService> logger, IValidator<ExcelAccessPointModel> validator)
        {
            _uow = uow;
            _logger = logger;
            _validator = validator;
        }

        public async Task ImportExcelAsync(IFormFile file)
        {
            try
            {
                // Load the Excel file into memory
                using var stream = new MemoryStream();
                await file.CopyToAsync(stream);
                stream.Position = 0;

                // read the first sheet
                using var package = new ExcelPackage(stream);
                var sheet = package.Workbook.Worksheets[0];
                int rows = sheet.Dimension.Rows;

                // Prepare collections to track unique values
                var programsToInsert = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var alcaldiasToInsert = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                var accessPointsToInsert = new List<ExcelAccessPointModel>();

                // Loop through Excel rows and create DTO with cells value
                for (int row = 2; row <= rows; row++)
                {
                    var dto = new ExcelAccessPointModel
                    {
                        Code = sheet.Cells[row, 1].Text.Trim(),
                        Program = sheet.Cells[row, 2].Text.Trim(),
                        Latitude = NumericHelper.SanitizeDouble(sheet.Cells[row, 3].Text),
                        Longitude = NumericHelper.SanitizeDouble(sheet.Cells[row, 4].Text),
                        Alcaldia = sheet.Cells[row, 5].Text.Trim()
                    };

                    // Validate each DTO (row) information
                    var validation = await _validator.ValidateAsync(dto);
                    if (!validation.IsValid)
                        throw new ValidationException(validation.Errors);

                    // Collect unique Programs, Alcaldias, and AccessPoints
                    programsToInsert.Add(dto.Program);
                    alcaldiasToInsert.Add(dto.Alcaldia);
                    accessPointsToInsert.Add(dto);
                }

                // Load existing Programs and Alcaldias from DB
                var existingPrograms = await _uow.Programs.GetAllAsync();
                var existingAlcaldias = await _uow.Alcaldias.GetAllAsync();

                // Detect missing Programs and Alcaldias
                var missingPrograms = programsToInsert
                    .Where(p => !existingPrograms.Any(ep => ep.Name == p))
                    .Select(p => new ProgramModel { Name = p })
                    .ToList();

                var missingAlcaldias = alcaldiasToInsert
                    .Where(a => !existingAlcaldias.Any(ea => ea.Name == a))
                    .Select(a => new AlcaldiaModel { Name = a })
                    .ToList();

                // Bulk insert missing Programs and Alcaldias
                await _uow.Programs.BulkInsertAsync(missingPrograms);
                await _uow.Alcaldias.BulkInsertAsync(missingAlcaldias);

                // Reload Programs and Alcaldias (now including newly inserted ones)
                existingPrograms = await _uow.Programs.GetAllAsync();
                existingAlcaldias = await _uow.Alcaldias.GetAllAsync();

                // Load existing AccessPoint codes from DB
                var existingCodes = new HashSet<string>(
                    await _uow.AccessPoints.GetAllCodesAsync(),
                    StringComparer.OrdinalIgnoreCase
                );


                var excelCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                var missingAccessPoints = new List<AccessPointModel>();

                foreach (var dto in accessPointsToInsert)
                {
                    // Skip duplicates inside Excel
                    if (!excelCodes.Add(dto.Code))
                        continue;

                    // Skip AccessPoints that already exist in DB
                    if (existingCodes.Contains(dto.Code))
                        continue;

                    // Resolve foreign keys
                    var programId = existingPrograms
                        .First(p => p.Name.Equals(dto.Program, StringComparison.OrdinalIgnoreCase))
                        .Id;

                    var alcaldiaId = existingAlcaldias
                        .First(a => a.Name.Equals(dto.Alcaldia, StringComparison.OrdinalIgnoreCase))
                        .Id;

                    missingAccessPoints.Add(new AccessPointModel
                    {
                        Code = dto.Code,
                        Latitude = dto.Latitude,
                        Longitude = dto.Longitude,
                        ProgramId = programId,
                        AlcaldiaId = alcaldiaId
                    });
                }

                await _uow.AccessPoints.BulkInsertAsync(missingAccessPoints);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while import the Excel file.");
                throw;
            }
        }
    }
}