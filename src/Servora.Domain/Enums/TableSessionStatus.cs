namespace Servora.Domain.Enums;

public enum TableSessionStatus
{
    /// <summary>Guests are seated and the bill is still open.</summary>
    Open = 1,

    /// <summary>The bill is paid and the table is free.</summary>
    Closed = 2
}
