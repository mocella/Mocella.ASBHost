using FluentValidation;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.Validators;

public class MocellaEmployeeRequestValidator : AbstractValidator<MocellaEmployeeRequest>
{
    public MocellaEmployeeRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(ASBConstants.NameMaxLength);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(ASBConstants.NameMaxLength);
        
        RuleFor(x => x.Email)
            .MaximumLength(ASBConstants.EmailMaxLength);
        
        RuleFor(x => x.EmployeeId)
            .MaximumLength(ASBConstants.EntityIdMaxLength);
    }
}