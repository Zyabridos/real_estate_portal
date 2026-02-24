using FluentValidation;
using RealEstate.Application.Features.Leads.Create;

namespace RealEstate.Application.Features.Leads.Create;

public sealed class CreateLeadRequestValidator : AbstractValidator<CreateLeadRequest>
{
    public CreateLeadRequestValidator()
    {
        RuleFor(x => x.PropertyId)
            .NotEmpty();

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        When(x => !string.IsNullOrWhiteSpace(x.Email), () =>
        {
            RuleFor(x => x.Email!)
                .EmailAddress()
                .MaximumLength(100);
        });

        When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber), () =>
        {
            RuleFor(x => x.PhoneNumber!)
                .MaximumLength(20);
        });

        RuleFor(x => x).Custom((req, ctx) =>
        {
            var hasEmail = !string.IsNullOrWhiteSpace(req.Email);
            var hasPhone = !string.IsNullOrWhiteSpace(req.PhoneNumber);

            if (!hasEmail && !hasPhone)
            {
                ctx.AddFailure(nameof(req.Email), "Provide either Email or PhoneNumber.");
                ctx.AddFailure(nameof(req.PhoneNumber), "Provide either Email or PhoneNumber.");
            }
        });
		
		RuleFor(x => x.Message)
            .MaximumLength(2000);
    }
}
