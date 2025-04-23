using FluentValidation;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.Validators;

public class MocellaCustomerRequestValidator : AbstractValidator<MocellaCustomerRequest>
{

    public MocellaCustomerRequestValidator()
    {
        RuleFor(x => x.Action).NotEmpty();
        RuleFor(x => x.RequestDateUtc).NotNull();
        RuleFor(x => x.SourceLastUpdatedDateUtc).NotNull();
        RuleFor(x => x.SourceSystem).NotEmpty();
        
        RuleFor(x => x.CustomerEvent).NotNull();
        RuleFor(x => x.CustomerEvent.CustomerName)
            .NotEmpty()
            .MaximumLength(ASBConstants.ShortNameMaxLength);
        RuleFor(x => x.CustomerEvent.DefaultPhoneRaw)
            .NotNull()
            .MaximumLength(ASBConstants.PhoneNumberMaxLength);
            // NOTE: below would be nice, but we don't know how other systems format phone numbers e.g. do they allow extensions/free-text?
            //.Matches(PortalConstants.RegexPattern_Phone); 
            
        RuleFor(x => x.CustomerEvent.InvoiceEmail)
                .NotEmpty()
                .Matches(ASBConstants.RegexPattern_Email)
                .MaximumLength(ASBConstants.EmailMaxLength);
            
        RuleFor(x => x.CustomerEvent.CustomerAddress).NotNull();
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        RuleFor(x => x.CustomerEvent.CustomerAddress.Address1)
            .NotNull()
            .MaximumLength(ASBConstants.NameMaxLength)
            .When(x => x.CustomerEvent.CustomerAddress != null);
        RuleFor(x => x.CustomerEvent.CustomerAddress.City)
            .NotNull()
            .MaximumLength(ASBConstants.NameMaxLength)
            .When(x => x.CustomerEvent.CustomerAddress != null);
        // RuleFor(x => x)
        //     .Must((customerRequest) => BeValidStateAbbr(customerRequest.CustomerEvent.CustomerAddress.StateAbbr))
        //     .WithMessage((request) => $"'CustomerEvent.CustomerAddress.StateAbbr' has invalid value: '{request.CustomerEvent.CustomerAddress.StateAbbr}'.")
        //     .When(x => x.CustomerEvent.CustomerAddress != null);
        // note: regex check for postalcode based on country, but again, unsure of other system formats here, like Phone fields
        RuleFor(x => x.CustomerEvent.CustomerAddress.PostalCode)
            .NotNull()
            .MaximumLength(ASBConstants.PostalCodeMaxLength)
            .When(x => x.CustomerEvent.CustomerAddress != null);
        // RuleFor(x => x)
        //     .Must((customerRequest) => BeValidCountry(customerRequest.CustomerEvent.CustomerAddress.Country))
        //     .WithMessage((request) => $"'CustomerEvent.CustomerAddress.Country' has invalid value: '{request.CustomerEvent.CustomerAddress.Country}'.")
        //     .When(x => x.CustomerEvent.CustomerAddress != null);
        // ReSharper enable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        
        RuleFor(x => x.CustomerEvent.CompanyName)
            .NotEmpty()
            .MaximumLength(ASBConstants.NameMaxLength);
        RuleFor(x => x.CustomerEvent.CompanyAddress).NotNull();
        // ReSharper disable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        RuleFor(x => x.CustomerEvent.CompanyAddress.Address1)
            .NotNull()
            .MaximumLength(ASBConstants.NameMaxLength)
            .When(x => x.CustomerEvent.CompanyAddress != null);
        RuleFor(x => x.CustomerEvent.CompanyAddress.City)
            .NotNull()
            .MaximumLength(ASBConstants.NameMaxLength)
            .When(x => x.CustomerEvent.CompanyAddress != null);
        // RuleFor(x => x)
        //     .Must((customerRequest) => BeValidStateAbbr(customerRequest.CustomerEvent.CompanyAddress.StateAbbr))
        //     .WithMessage((request) => $"'CustomerEvent.CustomerAddress.StateAbbr' has invalid value: '{request.CustomerEvent.CompanyAddress.StateAbbr}'.")
        //     .When(x => x.CustomerEvent.CustomerAddress != null);
        RuleFor(x => x.CustomerEvent.CompanyAddress.StateAbbr)
            .NotNull()
            .When(x => x.CustomerEvent.CompanyAddress != null);
        // note: regex check for postalcode based on country, but again, unsure of other system formats here, like Phone fields
        RuleFor(x => x.CustomerEvent.CompanyAddress.PostalCode)
            .NotNull()
            .MaximumLength(ASBConstants.PostalCodeMaxLength)
            .When(x => x.CustomerEvent.CompanyAddress != null);
        // RuleFor(x => x)
        //     .Must((customerRequest) => BeValidCountry(customerRequest.CustomerEvent.CompanyAddress.Country))
        //     .WithMessage((request) => $"'CustomerEvent.CompanyAddress.Country' has invalid value: '{request.CustomerEvent.CompanyAddress.Country}'.")
        //     .When(x => x.CustomerEvent.CompanyAddress != null);
        // ReSharper enable ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        
        RuleFor(x => x.CustomerEvent.BrandName).NotEmpty();
        
        // Netsuite specific checks
        RuleFor(x => x.CustomerEvent.NetsuiteEntityId)
            .NotNull()
            .MaximumLength(ASBConstants.EntityIdMaxLength)
            .When(x => x.SourceSystem.Equals(AsbHostConstants.SourceSystemNetsuite, StringComparison.InvariantCultureIgnoreCase));
        RuleFor(x => x.CustomerEvent.NetsuiteInternalId)
            .NotNull()
            .When(x => x.SourceSystem.Equals(AsbHostConstants.SourceSystemNetsuite, StringComparison.InvariantCultureIgnoreCase));
        
        // Contact Validation
        RuleForEach(x => x.CustomerEvent.Contacts).SetValidator(new MocellaContactRequestValidator());

        RuleFor(x => x.CustomerEvent.CustomerServiceMgr).SetValidator(new MocellaEmployeeRequestValidator()!);
        RuleFor(x => x.CustomerEvent.SalesRep).SetValidator(new MocellaEmployeeRequestValidator()!);
    }
}