namespace CachingProxy.Services;

class ArgsReader
{
    private readonly string[] args;

    public ArgsReader(string[] args)
    {
        this.args = args;
    }

    public bool HasFlag(string flag)
    {
        return this.args.Contains(flag);
    }

    public string? Read(string key)
    {
        for (int i = 0; i < this.args.Length; i++)
        {
            if (this.args[i] == key && i + 1 < this.args.Length)
            {
                return this.args[i + 1];
            }
        }

        return null;
    }
}