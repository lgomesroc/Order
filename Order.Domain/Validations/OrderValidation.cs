using FluentValidation;
using Order.Domain.Models;

namespace Order.Domain.Validations
{
    public class OrderValidation : AbstractValidator<OrderModel>
    {
        public OrderValidation()
        {
            ClassLevelCascadeMode = CascadeMode.Stop;
            RuleLevelCascadeMode = CascadeMode.Stop;

            RuleFor(x => x.Client)
                .NotNull().WithMessage("Client is required.");

            RuleFor(x => x.Client.Id)
                .NotEmpty().WithMessage("Client id is required.")
                .When(x => x.Client != null);

            RuleFor(x => x.User)
                .NotNull().WithMessage("User is required.");

            RuleFor(x => x.User.Id)
                .NotEmpty().WithMessage("User id is required.")
                .When(x => x.User != null);

            RuleFor(x => x.Items)
                .NotNull().WithMessage("Items are required.")
                .NotEmpty().WithMessage("The order must have at least one item.");

            RuleForEach(x => x.Items)
                .ChildRules(item =>
                {
                    item.RuleFor(i => i.Product)
                        .NotNull().WithMessage("Product is required.");

                    item.RuleFor(i => i.Product.Id)
                        .NotEmpty().WithMessage("Product id is required.")
                        .When(i => i.Product != null);

                    item.RuleFor(i => i.Quantity)
                        .GreaterThan(0).WithMessage("Quantity must be greater than zero.");

                    item.RuleFor(i => i.SellValue)
                        .GreaterThanOrEqualTo(0).WithMessage("Sell value cannot be negative.");
                })
                .When(x => x.Items != null);
        }
    }
}
