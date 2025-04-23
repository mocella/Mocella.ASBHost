using System.Text.Json.Serialization;

namespace Mocella.AsbHost.RequestObjects;

public class MocellaEmployeeRequest
{
    [JsonConstructor]
    public MocellaEmployeeRequest(){ }
    
    public MocellaEmployeeRequest(string employeeId, string firstName, string lastName, string email)
    {
        EmployeeId = employeeId;
        FirstName = firstName;
        LastName = lastName;
        Email = email;
    }
    
    public int? SourceSystemId { get; set; }
    public string EmployeeId { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string? NetsuiteEntityId { get; set; }
}