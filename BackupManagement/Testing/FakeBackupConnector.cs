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

    public Task FixBackupAsync(BackupSpec spec)
    {
        
        return Task.CompletedTask;
    }

    public async Task PerformBackupAsync(BackupSpec spec, ReadyBackupDetails details)
    {
        await Task.Delay(TimeSpan.FromSeconds(20));
        spec.Apply(details);
        _backupRepository.SaveSpec(spec);
    }

    public Task RestoreBackupAsync(string path)
    {
        throw new NotImplementedException();
    }
}