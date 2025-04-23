using System.Text.Json.Serialization;

namespace Mocella.AsbHost.RequestObjects;

public class MocellaContactRequest : IKeyedMocellaRequest
{
    [JsonConstructor]
    public MocellaContactRequest(){ }
    
    public MocellaContactRequest(string firstName, string lastName)
    {
        FirstName = firstName;
        LastName = lastName;
    }
    
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Email { get; set; }
    public string? MobilePhoneRaw { get; set; }
    public string? MobilePhoneFormatted { get; set; }
    public string? OfficePhoneRaw { get; set; }
    public string? OfficePhoneFormatted { get; set; }
    public string? JobTitle { get; set; }
    
    public Guid? PortalId { get; set; }
    public int? NetsuiteInternalId { get; set; }
    public string? NetsuiteEntityId { get; set; }
}