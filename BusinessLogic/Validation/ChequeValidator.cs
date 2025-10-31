using Domain.DTOs;
using FluentValidation;

namespace BusinessLogic.Validation;

public class ChequeValidator : AbstractValidator<ChequeDto>
{
    public ChequeValidator()
    {
        RuleFor(x => x.Number).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Amount).GreaterThanOrEqualTo(0);
    }
}