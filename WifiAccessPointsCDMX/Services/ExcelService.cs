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
        private readonly IValidator<ExcelAccessPointModel> _validator;

        public ExcelService(IUnitOfWork uow, IValidator<ExcelAccessPointModel> validator)
        {
            _uow = uow;
            _validator = validator;
        }

        public async Task ImportExcelAsync(IFormFile file)
        {
            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);
            stream.Position = 0;

            using var package = new ExcelPackage(stream);
            var sheet = package.Workbook.Worksheets[0];

            int rows = sheet.Dimension.Rows;

            var programsToInsert = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var alcaldiasToInsert = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var accessPointsToInsert = new List<ExcelAccessPointModel>();

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

                // VALIDATION
                var validation = await _validator.ValidateAsync(dto);
                if (!validation.IsValid)
                    throw new ValidationException(validation.Errors);

                programsToInsert.Add(dto.Program);
                alcaldiasToInsert.Add(dto.Alcaldia);
                accessPointsToInsert.Add(dto);
            }

            var existingPrograms = await _uow.Programs.GetAllAsync();
            var existingAlcaldias = await _uow.Alcaldias.GetAllAsync();

            var missingPrograms = programsToInsert
                .Where(p => !existingPrograms.Any(ep => ep.Name == p))
                .Select(p => new ProgramModel { Name = p })
                .ToList();

            var missingAlcaldias = alcaldiasToInsert
                .Where(a => !existingAlcaldias.Any(ea => ea.Name == a))
                .Select(a => new AlcaldiaModel { Name = a })
                .ToList();

            // BULK INSERT
            await _uow.Programs.BulkInsertAsync(missingPrograms);
            await _uow.Alcaldias.BulkInsertAsync(missingAlcaldias);

            existingPrograms = await _uow.Programs.GetAllAsync();
            existingAlcaldias = await _uow.Alcaldias.GetAllAsync();

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
    }
}