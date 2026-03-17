using FluentValidation;
using RealEstate.Application.Features.Brokers.Create;
using RealEstate.Application.Common.Normalizers;

namespace RealEstate.Application.Features.Brokers.Create;

public sealed class CreateBrokerRequestValidator : AbstractValidator<CreateBrokerRequest>
{
    public CreateBrokerRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .Must(value => PhoneNumberNormalizer.TryNormalize(value, out _, out _))
            .WithMessage("Invalid phone number format.");
    }
}