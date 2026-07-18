namespace CachingProxy.Services.Cache.Algorithm;

interface ICacheAlgorithm<KType, VType> where KType : notnull
{
    public bool TryGet(KType key, out VType? value);
    public void Put(KType key, VType value);
    public void Remove(KType key);
    public void Clear();
}