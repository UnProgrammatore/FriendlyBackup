using System.Security.Cryptography;
using System.Text;
using FriendlyBackup.Repositories;

namespace FriendlyBackup.Encryption;

public class FileEncryptor
{
    private readonly int _blockSize = 4096;
    private readonly IKeysRepository _keysRepository;
    private readonly Lazy<string> _key;
    public FileEncryptor(IKeysRepository keysRepository)
    {
        _keysRepository = keysRepository;
        _key = new Lazy<string>(() => _keysRepository.GetKey());
    }

    public IEnumerable<(byte[] EncryptedChunk, byte[] IV)> EncryptFile(string fileName)
    {
        using var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
        var fileBytes = new byte[_blockSize];
        using var aes = Aes.Create();
        aes.Key = Encoding.UTF8.GetBytes(_key.Value);

        while(fileStream.Read(fileBytes, 0, _blockSize) > 0)
        {
            aes.GenerateIV();
            var iv = aes.IV;
            var encryptedBytes = Encrypt(fileBytes, aes);
            yield return (fileBytes, iv);
            fileBytes = new byte[_blockSize];
        }
    }
    private byte[] Encrypt(byte[] bytes, Aes aes)
    {
        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        return encryptor.TransformFinalBlock(bytes, 0, bytes.Length);
    }
}