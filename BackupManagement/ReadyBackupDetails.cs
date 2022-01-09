namespace FriendlyBackup.BackupManagement;

public class ReadyBackupDetails
{
    public IEnumerable<BackedUpFile> ModifiedElements { get; }
    public IEnumerable<string> RemovableElements { get; }
    public ReadyBackupDetails(IEnumerable<BackedUpFile> modifiedElements, IEnumerable<string> removableElements)
    {
        ModifiedElements = modifiedElements;
        RemovableElements = removableElements;
    }
}