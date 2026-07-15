using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace CachingProxy.Services.Cache.Storage;

class FileStorageDriver<KType, VType> : ICacheStorageDriver<KType, VType> where KType : notnull
{

    private readonly string cacheDirectory;

    public FileStorageDriver()
    {
        this.cacheDirectory = Path.Combine(AppContext.BaseDirectory, "storage/cache");

        if (! Directory.Exists(this.cacheDirectory))
        {
            Directory.CreateDirectory(this.cacheDirectory);
        }
    }

    public VType Read(KType key)
    {
        string filePath = this.getFilePath(key);

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Cache miss on key {key}");
        }

        string jsonString = File.ReadAllText(filePath, Encoding.UTF8);

        var deserialized = JsonSerializer.Deserialize<VType>(jsonString);

        if (deserialized == null)
        {
            throw new JsonException($"Error deserializing {key}");
        }

        return deserialized;
    }

    public void Store(KType key, VType value)
    {
        string filePath = this.getFilePath(key);

        string jsonString = JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        File.WriteAllText(filePath, jsonString, Encoding.UTF8);
    }

    public void Clear()
    {
        if (Directory.Exists(this.cacheDirectory))
        {
            var files = Directory.GetFiles(this.cacheDirectory, "*.json");
            foreach (var file in files)
            {
                File.Delete(file);
            }
        }
    }

    public void Clear(KType key)
    {
        string filePath = this.getFilePath(key);

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }

    private string getFilePath(KType key)
    {
        string keyString = key.ToString() ?? string.Empty;

        byte[] inputBytes = Encoding.UTF8.GetBytes(keyString);
        byte[] hashBytes = SHA256.HashData(inputBytes);

        string fileName = Convert.ToHexString(hashBytes) + ".json";

        return Path.Combine(this.cacheDirectory, fileName);
    }
}