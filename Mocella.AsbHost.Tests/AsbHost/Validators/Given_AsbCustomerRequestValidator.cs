using System.Text.Json;
using System.Text.Json.Serialization;
using Mocella.AsbHost.Configuration;
using Mocella.AsbHost.RequestObjects;
using Mocella.AsbHost.Validators;

namespace Mocella.AsbHost.Tests.AsbHost.Validators;

[Trait(Constants.TestCategory, Constants.UnitTestCategory)]
public class Given_AsbCustomerRequestValidator 
{
    private readonly MocellaCustomerRequestValidator _validator;
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    { 
        WriteIndented = true, 
        ReferenceHandler = ReferenceHandler.IgnoreCycles,
        PropertyNameCaseInsensitive = true,
        IncludeFields = true,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull //saves significantly on payload size 
    };

    public Given_AsbCustomerRequestValidator()
    {
        _validator = new MocellaCustomerRequestValidator();
    }
    
    [Fact]
    public void Should_DeserializeMessage()
    {
        var messageContents =
            @"{""Action"":""Add"",""RequestDateUtc"":""2024-08-16T11:50:24"",""SourceLastUpdatedDateUtc"":""2024-01-03T10:02:06"",""SourceSystem"":""Netsuite"",""CustomerEvent"":{""EntityId"":""1045"",""NetsuiteInternalId"":161,""CustomerName"":""Darden Restaurants, Inc. - Yard House"",""SubsidiaryId"":1,""EntityType"":""CUSTOMER"",""StatusName"":""Closed Won"",""CompanyName"":""Darden Restaurants, Inc. - Yard House"",""BrandName"":""Yard House"",""DefaultPhoneRaw"":""(407) 245-4000"",""DefaultPhoneFormatted"":""4072454000"",""CustomerType"":""Corporate"",""CustomFormName"":""Mocella - Customer Form - All"",""CustomerAddress"":{""EntityId"":""1045"",""AddressInternalId"":139,""Address1"":""1000 Darden Center Dr."",""City"":""Orlando"",""StateAbbr"":""FL"",""PostalCode"":""32837"",""Country"":""US"",""Addressee"":""Darden - Yard House""},""CompanyAddress"":{""EntityId"":""1045"",""AddressInternalId"":139,""Address1"":""1000 Darden Center Dr."",""City"":""Orlando"",""StateAbbr"":""FL"",""PostalCode"":""32837"",""Country"":""US"",""Addressee"":""Darden - Yard House""},""SalesRep"":{""EntityId"":""1045"",""SourceSystemId"":5197,""EmployeeId"":""1112"",""FirstName"":""Brian"",""LastName"":""Shafer"",""Email"":""brian@Mocella.com""},""Contacts"":[{""EntityId"":""1045"",""InternalId"":25926,""FirstName"":""Invoices"",""LastName"":""Not Provided"",""Email"":""Invoice@darden.com"",""isPrimary"":false}]}}";

        var customerRequest = JsonSerializer.Deserialize<MocellaCustomerRequest>(messageContents, _jsonSerializerOptions);
        Assert.NotNull(customerRequest);
    }
    
    
    [Fact]
    public void Should_Pass_If_Valid_Add_Request_Provided()
    {
        // arrange
        var customerEvent = TestData.CustomerEvent();
        var asbCustomerRequest = new MocellaCustomerRequest(customerEvent,
            RequestAction.Add,
            DateTime.UtcNow,
            DateTime.UtcNow,
            AsbHostConstants.SourceSystemPortal
        );

        // act
        var result = _validator.Validate(asbCustomerRequest);
        
        // assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public void Should_Pass_If_Valid_Update_Request_Provided()
    {
        // arrange
        var customerEvent = TestData.CustomerEvent();
        var asbCustomerRequest = new MocellaCustomerRequest(customerEvent,
            RequestAction.Update,
            DateTime.UtcNow,
            DateTime.UtcNow,
            AsbHostConstants.SourceSystemPortal
        );

        // act
        var result = _validator.Validate(asbCustomerRequest);
        
        // assert
        Assert.True(result.IsValid);
    }
    
    [Fact]
    public void Should_Pass_If_Valid_Delete_Request_Provided_With_CustomerRequest()
    {
        // arrange
        var customerEvent = TestData.CustomerEvent();
        var asbCustomerRequest = new MocellaCustomerRequest(customerEvent, 
            RequestAction.Delete,
            DateTime.UtcNow,
            DateTime.UtcNow,
            AsbHostConstants.SourceSystemPortal
        );

        // act
        var result = _validator.Validate(asbCustomerRequest);
        
        // assert
        Assert.True(result.IsValid);
    }
}