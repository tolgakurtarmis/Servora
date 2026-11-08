namespace Servora.Domain.Enums;

public enum TableSessionStatus
{
    /// <summary>Guests are seated and the bill is still open.</summary>
    Open = 1,

    /// <summary>The bill is paid and the table is free.</summary>
    Closed = 2,

    /// <summary>Session was voided with no bill (e.g. opened by mistake).</summary>
    Cancelled = 3
}
