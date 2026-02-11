using UtilitiesService.Domain.Primitives;

namespace UtilitiesService.Domain.Entities;

public sealed class Utility : AggregateRoot
{
    public Guid Id { get; private set; }
    public Guid UserId { get; private set; }
    public string? Name { get; private set; }
    public string? Description { get; private set; }
    public decimal CostPerHour { get; private set; }

    private Utility() { } // required for ORM

    public Utility(Guid userId, string name, string description, decimal costPerHour)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        Name = name;
        Description = description;
        CostPerHour = costPerHour;

        //AddDomainEvent(new UtilityCreatedEvent(Id, UserId, Name, Description, CostPerHour));
    }
}
