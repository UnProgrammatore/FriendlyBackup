using System.Security.Cryptography;
using Newtonsoft.Json;

namespace FriendlyBackup.BackupManagement;

public class BackedUpFile
{
    public string Path { get; }

    public byte[] Hash { get; }

    [JsonConstructor]
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
