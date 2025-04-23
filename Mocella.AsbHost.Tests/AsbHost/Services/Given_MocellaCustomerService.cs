using Microsoft.Extensions.Logging;
using Mocella.AsbHost.Services;

namespace Mocella.AsbHost.Tests.AsbHost.Services;

[Trait(Constants.TestCategory, Constants.UnitTestCategory)]
public class Given_MocellaCustomerService 
{
    private readonly MocellaCustomerService _MocellaCustomerService;
    
    /// <summary>
    /// NOTE: this test class has been gutted/generalized for purposes of this demo ASB application
    /// </summary>
    public Given_MocellaCustomerService() 
    {
        Mock<ILogger<MocellaCustomerService>> logger = new();
        _MocellaCustomerService = new MocellaCustomerService(logger.Object);
    }

    [Fact]
    public void Should_Create_When_Provided_Valid_AddRequest()
    {
        // arrange
        var addCustomerRequest = TestData.CustomerEvent();
        
        // act
        var result = _MocellaCustomerService.Add(addCustomerRequest);
        
        // assert
        Assert.NotNull(result);
    }

    [Fact]
    public void Should_Update_When_Provided_UpdateRequest()
    {
        // arrange
        var updateCustomerRequest = TestData.CustomerEvent();
        
        // act
        var result = _MocellaCustomerService.Update(updateCustomerRequest);
        
        // assert
        Assert.NotNull(result);
    }
    
    [Fact]
    public void Should_Delete_When_Provided_Valid_DeleteRequest_With_Customer_Name_Match()
    {
        // arrange
        var deleteCustomerRequest = TestData.CustomerEvent();
        
        // act
        var result = _MocellaCustomerService.Delete(deleteCustomerRequest);
        
        // assert
        Assert.NotNull(result);
    }
}