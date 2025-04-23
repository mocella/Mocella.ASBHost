using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;

namespace Mocella.AsbHost.Tests;

public static class TestData
{
    public const string CustomerSubsidiaryDisplayValue = "Mocella";
    public const string CustomerStatusDisplayValue = "Closed Won";
    public const string CustomFormDisplayValue = "Mocella - Customer Form - All";
    
    public static string StringOfLength(int length)
    {
        return new string('C', length);
    }

    public static AzureConfig AzureConfig(bool useRootNamespace = true)
    {
        return new AzureConfig
        {
            ServiceBus = new ServiceBusConfig
            {
                Namespace = useRootNamespace ? Faker.Lorem.GetFirstWord() : string.Empty,
                Topics =
                [
                    new TopicConfig
                    {
                        Namespace = useRootNamespace ? string.Empty : Faker.Lorem.GetFirstWord(),
                        TopicName = AsbHostConstants.CustomerEventsTopicName,
                        SubscriptionName = Faker.Lorem.GetFirstWord(),
                        ResponseTopicName = Faker.Lorem.GetFirstWord(),
                        RetryPolicy = new RetryPolicyConfig
                        {
                            MaxRetries = 5,
                            DelayInMinutes = 1
                        }
                    }
                ]
            }
        };
    }

    public static CustomerEvent CustomerEvent(Guid? portalId = null, 
        string? customerName = null, 
        int stateProvinceId = 1, 
        int countryId = 1)
    {
        return new CustomerEvent
        {
            CustomerName = customerName ?? Faker.Company.Name(),
            CustomerAddress = MocellaAddressRequest(stateProvinceId, countryId),
            CompanyName = Faker.Company.Name(),
            CompanyAddress = MocellaAddressRequest(stateProvinceId, countryId),
            BrandName = Faker.Company.Name(),
            CustomFormName = CustomFormDisplayValue,
            SubsidiaryName = CustomerSubsidiaryDisplayValue,
            StatusName = CustomerStatusDisplayValue,
            PortalId = portalId, // don't want to default this so null-expectant tests pass
            DefaultPhoneRaw = Faker.Phone.Number(),
            DefaultPhoneFormatted = Faker.Phone.Number(),
            NetsuiteInternalId = Faker.RandomNumber.Next(),
            NetsuiteEntityId = Faker.Lorem.GetFirstWord(),
            Contacts =
            [
                new MocellaContactRequest
                {
                    FirstName = Faker.Name.First(),
                    LastName = Faker.Name.Last(),
                    Email = Faker.Internet.Email(),
                    MobilePhoneRaw = Faker.Phone.Number(),
                    OfficePhoneRaw = Faker.Phone.Number(),
                    NetsuiteInternalId = Faker.RandomNumber.Next(),
                    NetsuiteEntityId = Faker.Lorem.GetFirstWord(),
                }
            ],
            CustomerServiceMgr = MocellaEmployeeRequest(),
            SalesRep = MocellaEmployeeRequest(),
            InvoiceEmail = Faker.Internet.Email()
        };
    }

    public static MocellaAddressRequest MocellaAddressRequest(int stateProvinceId = 1, int countryId = 1)
    {
        return new MocellaAddressRequest
        {
            Address1 = Faker.Address.StreetAddress(),
            City = Faker.Address.City(),
            StateAbbr = stateProvinceId == 1 ? "AL" : Faker.Address.UsStateAbbr(),
            PostalCode = Faker.Address.ZipCode(),
            Country = countryId == 1 ? "United States" : Faker.Address.Country()
        };
    }

    public static MocellaEmployeeRequest MocellaEmployeeRequest()
    {
        return new MocellaEmployeeRequest
        {
            FirstName = Faker.Name.First(),
            LastName = Faker.Name.Last(),
            Email = Faker.Internet.Email(),
            NetsuiteEntityId = Faker.Lorem.GetFirstWord(),
        };
    } 
    
}
