using System.Security.Cryptography;

public class BackedUpFile : IBackedUpElement
{
    public string Path { get; }

    public byte[] Hash { get; }

    public BackedUpFile(string path, byte[] hash) 
    {
        Path = path;
        Hash = hash;
    }
    public BackedUpFile(string path) 
    {
        Path = path;
        using var md5 = MD5.Create();
        using var stream = File.OpenRead(path);
        Hash = md5.ComputeHash(stream);
    }
}
