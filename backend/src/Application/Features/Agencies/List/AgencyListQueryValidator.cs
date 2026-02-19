using FluentValidation;
using RealEstate.Application.Features.Agencies.List;

namespace RealEstate.Application.Features.Agencies.List;

public sealed class AgencyListQueryValidator : AbstractValidator<AgencyListQuery>
{
    private const int MaxPageSize = 100; // protect API
    public AgencyListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(MaxPageSize);
    }
}