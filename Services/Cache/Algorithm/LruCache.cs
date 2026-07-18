namespace CachingProxy.Services.Cache.Algorithm;

class LruCache<KType, VType> : ICacheAlgorithm<KType, VType> where KType : notnull
{
    
    private readonly Dictionary<KType, VType> map;
    private readonly LinkedList<KType> list;
    private readonly int capacity;

    public LruCache(int capacity)
    {
        this.capacity = capacity;
        this.map = new Dictionary<KType, VType>();
        this.list = new LinkedList<KType>();
    }

    public bool TryGet(KType key, out VType? value)
    {
        if (this.map.TryGetValue(key, out var found))
        {
            this.list.Remove(key);
            this.list.AddFirst(key);
            value = found;
            return true;
        }

        value = default;
        return false;
    }

    public void Put(KType key, VType value)
    {
        if (this.map.ContainsKey(key))
        {
            this.map[key] = value;
            this.list.Remove(key);
            this.list.AddFirst(key);
            return;
        }

        if (this.map.Count >= this.capacity && this.list.Last != null)
        {
            KType oldestKey = this.list.Last.Value;
            this.map.Remove(oldestKey);
            this.list.RemoveLast();
        }

        this.map.Add(key, value);
        this.list.AddFirst(key);
    }

    public void Remove(KType key)
    {
        this.map.Remove(key);
        this.list.Remove(key);
    }

    public void Clear()
    {
        this.map.Clear();
        this.list.Clear();
    }
}