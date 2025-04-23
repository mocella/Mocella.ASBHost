using System.Text.Json.Serialization;

namespace Mocella.AsbHost.RequestObjects;

public class CustomerEvent : IKeyedMocellaRequest
{
    [JsonConstructor]
    public CustomerEvent() { }

    public CustomerEvent(string customerName,
        MocellaAddressRequest customerAddress,
        string companyName,
        MocellaAddressRequest companyAddress,
        string brandName,
        string customFormName)
    {
        CustomerName = customerName;
        CustomerAddress = customerAddress;
        CompanyName = companyName;
        CompanyAddress = companyAddress;
        BrandName = brandName;
        CustomFormName = customFormName;
    }

    public Guid? PortalId { get; set; }
    public int? NetsuiteInternalId { get; set; }
    public string? NetsuiteEntityId { get; set; }
    
    public string CustomerName { get; set; } = null!;
    public MocellaAddressRequest CustomerAddress { get; set; } = null!;

    public string CompanyName { get; set; } = null!;
    public MocellaAddressRequest CompanyAddress { get; set; } = null!;

    public string BrandName { get; set; } = null!;
    
    public string? InvoiceEmail { get; set; }
    public string ? DefaultPhoneRaw { get; set; }
    public string ? DefaultPhoneFormatted { get; set; }
    public string? UniqueIdentifier { get; set; }
    public string? CustomerType { get; set; }
    public string? SubsidiaryName { get; set; }
    public string? StatusEntityType { get; set; }
    public string StatusName { get; set; } = null!;
    public string? Comments { get; set; }
    public string CustomFormName { get; set; } = null!;

    public MocellaContactRequest[]? Contacts { get; set; }
    public MocellaEmployeeRequest? SalesRep { get; set; }
    public MocellaEmployeeRequest? CustomerServiceMgr { get; set; }
}