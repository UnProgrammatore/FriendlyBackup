using System.Security.Cryptography;
using FriendlyBackup.Repositories;

namespace FriendlyBackup.Encryption;

public class FileEncryptor
{
    private readonly int _blockSize = 4096;
    private readonly IKeysRepository _keysRepository;
    private readonly 
    public FileEncryptor(IKeysRepository keysRepository)
    {
        _keysRepository = keysRepository;
    }

    public IEnumerable<(byte[] EncryptedChunk, byte[] IV)> EncryptFile(string fileName)
    {
        using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        var fileBytes = new byte[_blockSize];
        using var aes = Aes.Create();
        
        while(fileStream.Read(fileBytes, 0, _blockSize) > 0)
        {   
            aes.GenerateIV();
            var iv = aes.IV;
            var encryptedBytes = Encrypt(fileBytes, aes);
            yield return (fileBytes, iv);
            fileBytes = new byte[_blockSize];
        }
    }

    private byte[] CreateIV()
        => RandomNumberGenerator.GetBytes(16);
    private byte[] Encrypt(byte[] bytes, Aes aes)
    {
        
    }
}