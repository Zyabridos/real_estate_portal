using FluentValidation;
using RealEstate.Application.Features.Brokers.List;

namespace RealEstate.Application.Features.Brokers.List;

public sealed class BrokerListQueryValidator : AbstractValidator<BrokerListQuery>
{
    private const int MaxPageSize = 100; // protect API

    public BrokerListQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .LessThanOrEqualTo(MaxPageSize);
    }
}