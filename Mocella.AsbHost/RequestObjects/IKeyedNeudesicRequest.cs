namespace Mocella.AsbHost.RequestObjects;

public interface IKeyedMocellaRequest
{
    Guid? PortalId { get; set; }
    int? NetsuiteInternalId { get; set; }
    string? NetsuiteEntityId { get; set; }
}