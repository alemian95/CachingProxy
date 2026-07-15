using CachingProxy.Services.Cache.Storage;

namespace CachingProxy.Services.Cache;

class CacheService<KType, VType> where KType : notnull
{
    private Dictionary<KType, VType> cache;

    // cache storage driver implementato: definire qui l'algoritmo per mantenere in memory solo gli accessi più frequenti
    private ICacheStorageDriver<KType, VType>? storage;

    public CacheService(ICacheStorageDriver<KType, VType>? storage = null)
    {
        this.cache = new Dictionary<KType, VType>();
        this.storage = storage;
    }

    public VType Read(KType key)
    {

        if (this.storage != null)
        {
            return this.storage.Read(key);
        }

        if (this.cache.ContainsKey(key))
        {
            return this.cache[key];
        }

        throw new Exception($"Missing Key: {key}");
    }

    public void Store(KType key, VType value)
    {
        if (this.storage != null)
        {
            this.storage.Store(key, value);
        }
        else
        {
            this.cache.Add(key, value);
        }
    }

    public void Clear()
    {
        if (this.storage != null)
        {
            this.storage.Clear();
        }
        else
        {
            this.cache.Clear();
        }
    }

    public void Clear (KType key)
    {
        if (this.storage != null)
        {
            this.storage.Clear(key);
        }
        else
        {
            this.cache.Remove(key);
        }
    }


}