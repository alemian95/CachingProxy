using CachingProxy.Services.CacheStorage;

namespace CachingProxy.Services;

class CacheService<KType, VType> where KType : notnull
{
    private Dictionary<KType, VType> cache;

    // IDEA PER IL FUTURO: decidere se persistere la cache (driver: redis, database, file)
    // - Persistere significa mantenere comunque in memoria parte della cache (chiavi ad accesso più frequente)
    // - Passare la classe driver come dipendenza
    // - Oppure avere più CacheService
    //
    // private bool persist;

    private ICacheStorageDriver<KType, VType>? storage;

    public CacheService(ICacheStorageDriver<KType, VType>? storage = null)
    {
        this.cache = new Dictionary<KType, VType>();
        this.storage = storage;
    }

    public VType Read(KType key)
    {
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
        this.cache.Add(key, value);
    }


}