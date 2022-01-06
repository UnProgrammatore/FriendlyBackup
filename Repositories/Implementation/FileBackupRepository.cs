using log4net;
using Newtonsoft.Json;

public class FileBackupRepository : IBackupRepository
{
    private readonly string _repoPath;
    private readonly ILog _logger;
    public FileBackupRepository(string repoPath, ILog logger)
    {
        _repoPath = repoPath;
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
                _logger.Error($"Failed to deserialize BackupSpec {file}");
        }
    }
    public void SaveSpec(BackupSpec spec) 
    {
        File.WriteAllText(Path.Combine(_repoPath, spec.GenerateUniqueID()), JsonConvert.SerializeObject(spec));
    }
}