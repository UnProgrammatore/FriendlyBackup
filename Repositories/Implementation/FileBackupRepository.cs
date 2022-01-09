using FriendlyBackup.BackupManagement;
using FriendlyBackup.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace FriendlyBackup.Repositories.Implementation;

public class FileBackupRepository : IBackupRepository
{
    private readonly string _repoPath;
    private readonly ILogger<FileBackupRepository> _logger;
    public FileBackupRepository(IOptions<LocalPathsConfig> localPathsConfig, ILogger<FileBackupRepository> logger)
    {
        _repoPath = localPathsConfig.Value.BackupRepositoryPath 
            ?? throw new ArgumentNullException($"{nameof(localPathsConfig)}.{nameof(localPathsConfig.Value.BackupRepositoryPath)}");

        _logger = logger;
    }
    public IEnumerable<BackupSpec> GetAllSpecs()
    {
        Directory.GetFiles(_repoPath);
        foreach(var file in Directory.GetFiles(_repoPath))
        {
            var obj = JsonConvert.DeserializeObject<BackupSpec>(File.ReadAllText(file));
            if(obj != null)
                yield return obj;
            else
                _logger.LogError($"Failed to deserialize BackupSpec {file}");
        }
    }
    public void SaveSpec(BackupSpec spec) 
    {
        File.WriteAllText(Path.Combine(_repoPath, spec.GenerateUniqueID()), JsonConvert.SerializeObject(spec));
    }
}