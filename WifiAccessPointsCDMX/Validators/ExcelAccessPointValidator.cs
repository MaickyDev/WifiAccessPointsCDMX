using FluentValidation;
using WifiAccessPointsCDMX.Models;

namespace WifiAccessPointsCDMX.Validators
{
    public class ExcelAccessPointValidator : AbstractValidator<ExcelAccessPointModel>
    {
        public ExcelAccessPointValidator()
        {
            RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required");

            RuleFor(x => x.Program)
                .NotEmpty().WithMessage("Program is required");

            RuleFor(x => x.Alcaldia)
                .NotEmpty().WithMessage("Alcaldia is required");

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90);

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180);
        }
    }
}
