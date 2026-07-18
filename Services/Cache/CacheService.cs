using CachingProxy.Services.Cache.Algorithm;
using CachingProxy.Services.Cache.Storage;

namespace CachingProxy.Services.Cache;

class CacheService<KType, VType> where KType : notnull
{

    
    private readonly ICacheStorageDriver<KType, VType> storage;
    private readonly ICacheAlgorithm<KType, VType> cache;

    public CacheService(ICacheAlgorithm<KType, VType> cache, ICacheStorageDriver<KType, VType> storage)
    {
        this.cache = cache;
        this.storage = storage;
    }

    public VType Read(KType key)
    {

        if (this.cache.TryGet(key, out var value) && value != null)
        {
            return value;
        }

        VType storedValue = this.storage.Read(key);

        this.cache.Put(key, storedValue);

        return storedValue;
    }

    public void Store(KType key, VType value)
    {
        this.storage.Store(key, value);
        this.cache.Put(key, value);
    }

    public void Clear()
    {
        this.cache.Clear();
        this.storage.Clear();
    }

    public void Clear (KType key)
    {
        this.cache.Remove(key);
        this.storage.Clear(key);
    }


}