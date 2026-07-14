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

    public CacheService()
    {
        this.cache = new Dictionary<KType, VType>();
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
        this.cache.Add(key, value);
    }


}