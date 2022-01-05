public class BackupSpec {
    public string Path { get; }
    public IEnumerable<IBackedUpElement> BackedUpElements => _backedUpElements.Values;
    private readonly Dictionary<string, IBackedUpElement> _backedUpElements = new Dictionary<string, IBackedUpElement>();

    public BackupSpec(string path) {
        Path = path;
    }

    private static IEnumerable<IBackedUpElement> CalculateBackup(string path) {
        foreach(var file in Directory.GetFiles(path))
            yield return new BackedUpFile(file);
        foreach(var folder in Directory.GetDirectories(path))
            foreach(var newFile in CalculateBackup(folder)) 
                yield return newFile;
    }
    public ReadyBackupDetails DiffBackup() {
        var newBackedUpFiles = new Dictionary<string, IBackedUpElement>();
        var notFoundFiles = new HashSet<string>(_backedUpElements.Keys);
        var newFiles = new List<string>();
        foreach(var newElement in CalculateBackup(Path)) {
            if(!_backedUpElements.ContainsKey(newElement.Path))
            {
                newFiles.Add(newElement.Path);
                newBackedUpFiles.Add(newElement.Path, newElement);
            }
            else
            {
                notFoundFiles.Remove(newElement.Path);
                if(!newElement.Hash.SequenceEqual(_backedUpElements[newElement.Path].Hash))
                {
                    newFiles.Add(newElement.Path);
                    newBackedUpFiles.Add(newElement.Path, newElement);
                }
            }
        }

        return new ReadyBackupDetails(newFiles, notFoundFiles);
    }
}