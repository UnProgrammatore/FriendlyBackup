public class ReadyBackupDetails
{
    public IEnumerable<string> ModifiedElements { get; }
    public IEnumerable<string> RemovableElements { get; }
    public ReadyBackupDetails(IEnumerable<string> modifiedElements, IEnumerable<string> removableElements)
    {
        ModifiedElements = modifiedElements;
        RemovableElements = removableElements;
    }
}