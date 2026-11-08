using Microsoft.Extensions.Caching.Memory;
using Servora.Application.Abstractions;

namespace Servora.Infrastructure.Caching;

internal sealed class MemoryAppCache : IAppCache
{
    private readonly IMemoryCache _cache;

    public MemoryAppCache(IMemoryCache cache) => _cache = cache;

    public bool TryGetValue<T>(string key, out T? value)
    {
        return _cache.TryGetValue(key, out value);
    }

    public void Set<T>(string key, T value, TimeSpan duration)
    {
        _cache.Set(key, value, duration);
    }

    public async Task<T> GetOrCreateAsync<T>(
        string key,
        TimeSpan duration,
        Func<CancellationToken, Task<T>> factory,
        CancellationToken cancellationToken)
    {
        if (_cache.TryGetValue(key, out T? cached) && cached is not null)
        {
            return cached;
        }

        var value = await factory(cancellationToken);
        _cache.Set(key, value, duration);
        return value;
    }

    public void Remove(string key)
    {
        _cache.Remove(key);
    }
}
