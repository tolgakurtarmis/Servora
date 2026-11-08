namespace Servora.Application.Abstractions;

public interface IAppCache
{
    bool TryGetValue<T>(string key, out T? value);

    void Set<T>(string key, T value, TimeSpan duration);

    Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan duration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken);

    void Remove(string key);
}

public static class CacheKeys
{
    public static string Menus(Guid branchId) => $"menus:{branchId:D}";

    public static string Tables(Guid branchId) => $"tables:{branchId:D}";

    public static string Branch(Guid branchId) => $"branch:{branchId:D}";
}

public static class CacheDurations
{
    public static readonly TimeSpan Menus = TimeSpan.FromHours(1);

    public static readonly TimeSpan Tables = TimeSpan.FromHours(1);

    public static readonly TimeSpan Branch = TimeSpan.FromMinutes(5);
}

public sealed record CachedBranch(Guid Id, Guid TenantId, bool UsesKitchen, bool IsActive);

public sealed record CachedTable(Guid Id, string Name);
