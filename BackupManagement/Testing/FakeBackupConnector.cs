using FriendlyBackup.Repositories;

namespace FriendlyBackup.BackupManagement.Testing;
public class FakeBackupConnector : IBackupConnector
{
    private readonly IBackupRepository _backupRepository;

    public FakeBackupConnector(IBackupRepository backupRepository)
    {
        _backupRepository = backupRepository;
    }

    public Task<bool> CompareBackupAsync(BackupSpec spec)
    {
        return Task.FromResult(true);
    }

    public Task PerformBackupAsync(BackupSpec spec, ReadyBackupDetails details)
    {
        spec.Apply(details);
        _backupRepository.SaveSpec(spec);
        return Task.CompletedTask;
    }

    public Task RestoreBackupAsync(string path)
    {
        throw new NotImplementedException();
    }
}