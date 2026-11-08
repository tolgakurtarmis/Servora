namespace Servora.Domain.Abstractions;

public interface IAudited
{
    Guid? CreateBy { get; set; }

    DateTime CreateDate { get; set; }

    Guid? ModifyBy { get; set; }

    DateTime? ModifyDate { get; set; }

    bool IsActive { get; set; }

    bool IsDeleted { get; set; }
}
