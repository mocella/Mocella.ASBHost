using System.Text.Json.Serialization;

namespace Mocella.AsbHost.RequestObjects;

public class MocellaAddressRequest
{
    [JsonConstructor]
    public MocellaAddressRequest(){ }

    public MocellaAddressRequest(string address1, string city, string stateAbbr, string postalCode, string country)
    {
        Address1 = address1;
        City = city;
        StateAbbr = stateAbbr;
        PostalCode = postalCode;
        Country = country;
    }
    
    public string? Attention { get; set; }
    public string? Addressee { get; set; }
    public string Address1 { get; set; } = null!;
    public string? Address2 { get; set; }
    public string? Address3 { get; set; }
    public string City { get; set; } = null!;
    public string StateAbbr { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string Country { get; set; } = null!;
    public float? Latitude { get; set; }
    public float? Longitude { get; set; }
    public string? NetsuiteEntityId { get; set; }
}