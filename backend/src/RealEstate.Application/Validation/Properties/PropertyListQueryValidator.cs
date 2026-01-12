using FluentValidation;
using RealEstate.Application.DTOs.Properties;

namespace RealEstate.Application.Validation.Properties;

public sealed class PropertyListQueryValidator : AbstractValidator<PropertyListQuery>
{
    private const int MaxPageSize = 100; // protect API from request of, for example, pageSize=1000000000

    public PropertyListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(MaxPageSize);

        RuleFor(x => x.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MinPrice.HasValue);

        RuleFor(x => x.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MaxPrice.HasValue);

        RuleFor(x => x)
            .Must(x => !x.MinPrice.HasValue || !x.MaxPrice.HasValue || x.MinPrice <= x.MaxPrice)
            .WithMessage("minPrice must be less than or equal to maxPrice.");
    }
}