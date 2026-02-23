using FluentValidation;
using RealEstate.Application.Features.Brokers.Create;
using RealEstate.Application.Common.Normalizers;

namespace RealEstate.Application.Features.Agencies.Create;

public sealed class CreateAgencyRequestValidator : AbstractValidator<CreateAgencyRequest>
{
    public CreateAgencyRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.OrgNumber)
            .NotEmpty()
            .MinimumLength(6)
            .MaximumLength(20);
    }
}