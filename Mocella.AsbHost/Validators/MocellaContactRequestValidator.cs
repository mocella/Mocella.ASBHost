using FluentValidation;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.Validators;

public class MocellaContactRequestValidator : AbstractValidator<MocellaContactRequest>
{
    public MocellaContactRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(ASBConstants.NameMaxLength);
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(ASBConstants.NameMaxLength);
        
        RuleFor(x => x.Email)
            .MaximumLength(ASBConstants.EmailMaxLength);
        
        RuleFor(x => x.MobilePhoneRaw)
            .MaximumLength(ASBConstants.PhoneNumberMaxLength);
        
        RuleFor(x => x.OfficePhoneRaw)
            .MaximumLength(ASBConstants.PhoneNumberMaxLength);
        
        RuleFor(x => x.JobTitle)
            .MaximumLength(ASBConstants.JobTitleMaxLength);
    }
}