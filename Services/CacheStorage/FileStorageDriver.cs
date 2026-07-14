namespace CachingProxy.Services.CacheStorage;

class FileStorageDriver<KType, VType> : ICacheStorageDriver<KType, VType> where KType : notnull
{
    public VType Read(KType key)
    {
        throw new NotImplementedException();
    }

    public void Store(KType key, VType value)
    {
        throw new NotImplementedException();
    }
}