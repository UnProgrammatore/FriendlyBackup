public interface IBackupRepository
{
    IEnumerable<BackupSpec> GetAllSpecs();
    void SaveSpec(BackupSpec spec);
}