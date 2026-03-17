using FluentValidation;
using RealEstate.Application.Features.Properties.Create;
using RealEstate.Application.Common.Normalizers;

namespace RealEstate.Application.Features.Properties.Create;

public sealed class CreatePropertyRequestValidator : AbstractValidator<CreatePropertyRequest>
{
    public CreatePropertyRequestValidator()
    {
        RuleFor(x => x.BrokerId)
            .GreaterThan(0);
        
        RuleFor(x => x.Title)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);
        
        RuleFor(x => x.Address)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.City)
            .NotEmpty()
            .MaximumLength(50);
        
        RuleFor(x => x.Price)
            .GreaterThan(1m);
        
        RuleFor(x => x.Area)
            .GreaterThan(1m);
        
        RuleFor(x => x.Bedrooms)
            .GreaterThan(0);
        
        RuleFor(x => x.Bathrooms)
            .GreaterThan(0);
    }
}