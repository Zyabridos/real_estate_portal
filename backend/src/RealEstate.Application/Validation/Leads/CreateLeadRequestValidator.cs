using FluentValidation;
using RealEstate.Application.DTOs.Leads;

namespace RealEstate.Application.Validation.Leads;

public sealed class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty()
            .MaximumLength(64);

        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(100);

        RuleFor(x => x.Phone)
            .MaximumLength(20);

        RuleFor(x => x.Message)
            .MaximumLength(2000);
    }
}