using FluentValidation;
using Order.Domain.Models;

namespace Order.Domain.Validations
{
    public class ProductValidation : AbstractValidator<ProductModel>
    {
        public ProductValidation()
        {
            RuleLevelCascadeMode = CascadeMode.Stop;
            RuleFor(x => x.Description).NotNull().NotEmpty().Length(3, 30);
            RuleFor(x => x.SellValue).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        }
    }
}
