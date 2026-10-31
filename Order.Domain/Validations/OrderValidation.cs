using FluentValidation;
using Order.Domain.Models;

namespace Order.Domain.Validations
{
    public class OrderValidation : AbstractValidator<OrderModel>
    {
        public OrderValidation()
        {
            RuleFor(order => order.Client).NotNull();
            RuleFor(order => order.User).NotNull();
            RuleFor(order => order.Items)
                .NotNull()
                .Must(items => items != null && items.Count > 0)
                .WithMessage("An order must contain at least one item.");

            RuleForEach(order => order.Items).ChildRules(item =>
            {
                item.RuleFor(x => x.Product).NotNull();
                item.RuleFor(x => x.Quantity).GreaterThan(0);
            });
        }
    }
}
