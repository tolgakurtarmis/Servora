using Servora.Domain.Abstractions;

namespace Servora.Domain.Entities;

public abstract class Entity : IAudited
{
    public Guid Id { get; protected set; } = Guid.NewGuid();

    public Guid? CreateBy { get; set; }

    public DateTime CreateDate { get; set; }

    public Guid? ModifyBy { get; set; }

    public DateTime? ModifyDate { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsDeleted { get; set; } = false;
}
