using FluentValidation;
using Order.Domain.Models;

namespace Order.Domain.Validations
{
    public class ProductValidation : AbstractValidator<ProductModel>
    {
        public ProductValidation()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Description)
                .NotNull()
                .NotEmpty()
                .Length(3, 30);

            RuleFor(x => x.SellValue)
                .GreaterThan(0).WithMessage("Sell value must be greater than zero.");

            RuleFor(x => x.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");
        }
    }
}
