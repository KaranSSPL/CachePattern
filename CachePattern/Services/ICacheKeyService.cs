namespace CachePattern.Services;
public interface ICacheKeyService
{
    string GetCacheKey(string name, object id);
}