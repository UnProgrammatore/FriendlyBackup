using FriendlyBackup.BackupManagement;

namespace FriendlyBackup.Repositories;

public interface IBackupRepository
{
    IEnumerable<BackupSpec> GetAllSpecs();
    void SaveSpec(BackupSpec spec);
}