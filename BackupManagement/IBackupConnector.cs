namespace FriendlyBackup.BackupManagement;

public interface IBackupConnector
{
    Task PerformBackupAsync(BackupSpec spec, ReadyBackupDetails details);
    Task<(bool IsSame, BackupSpec? RemoteSpec)> CompareBackupAsync(BackupSpec spec);
    Task FixBackupAsync(BackupSpec spec);
    Task RestoreBackupAsync(string path);
}