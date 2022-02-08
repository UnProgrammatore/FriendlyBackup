using FriendlyBackup.Configuration;
using Microsoft.Extensions.Options;

namespace FriendlyBackup.Repositories.Implementation
{
    public class FileKeysRepository : IKeysRepository
    {
        private readonly string _repoPath;
        private readonly ILogger<FileBackupRepository> _logger;
        public FileKeysRepository(IOptions<LocalPathsConfig> localPathsConfig, ILogger<FileBackupRepository> logger)
        {
            _repoPath = localPathsConfig.Value.EncryptionKeyPath
                ?? throw new ArgumentNullException($"{nameof(localPathsConfig)}.{nameof(localPathsConfig.Value.EncryptionKeyPath)}");

            _logger = logger;
        }

        public string GetKey() 
        {
            return File.ReadAllText(_repoPath);
        }
    }
}