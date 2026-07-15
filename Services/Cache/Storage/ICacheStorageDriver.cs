namespace CachingProxy.Services.Cache.Storage;

interface ICacheStorageDriver<KType, VType> where KType : notnull
{
    public void Store(KType key, VType value);

    public VType Read(KType key);

    public void Clear();

    public void Clear(KType key);
}