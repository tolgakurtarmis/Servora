using Servora.Domain.Entities;

namespace Servora.Domain.Models;

public sealed class Tenant : Entity
{
    public string Name { get; set; } = string.Empty;
}
