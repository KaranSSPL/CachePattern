using CachePattern.Data;
using CachePattern.Services;

namespace CachePattern.Extensions;
public static class CacheKeyServiceExtensions
{
    public static string GetCacheKey<TEntity>(this ICacheKeyService cacheKeyService, object id) where TEntity : IBaseEntity => cacheKeyService.GetCacheKey(typeof(TEntity).Name, id);
}