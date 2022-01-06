using System.Security.Cryptography;
using System.Text;

public class BackupSpec {
    public string Path { get; }
    public IEnumerable<BackedUpFile> BackedUpElements => _backedUpElements.Values;
    private readonly Dictionary<string, BackedUpFile> _backedUpElements;

    public BackupSpec(string path) {
        Path = path;
        _backedUpElements = new Dictionary<string, BackedUpFile>();
    }

    public BackupSpec(string path, IEnumerable<BackedUpFile> backedUpElements) {
        Path = path;
        _backedUpElements = backedUpElements.ToDictionary(x => x.Path);
    }

    private static IEnumerable<BackedUpFile> CalculateBackup(string path) {
        foreach(var file in Directory.GetFiles(path))
            yield return new BackedUpFile(file);
        foreach(var folder in Directory.GetDirectories(path))
            foreach(var newFile in CalculateBackup(folder)) 
                yield return newFile;
    }
    public ReadyBackupDetails DiffBackup() {
        var newBackedUpFiles = new Dictionary<string, BackedUpFile>();
        var notFoundFiles = new HashSet<string>(_backedUpElements.Keys);
        foreach(var newElement in CalculateBackup(Path)) {
            if(!_backedUpElements.ContainsKey(newElement.Path))
            {
                newBackedUpFiles.Add(newElement.Path, newElement);
            }
            else
            {
                notFoundFiles.Remove(newElement.Path);
                if(!newElement.Hash.SequenceEqual(_backedUpElements[newElement.Path].Hash))
                {
                    newBackedUpFiles.Add(newElement.Path, newElement);
                }
            }
        }

        return new ReadyBackupDetails(newBackedUpFiles.Values, notFoundFiles);
    }

    public void Apply(ReadyBackupDetails readyBackupDetails) {
        foreach(var newFile in readyBackupDetails.ModifiedElements)
            _backedUpElements[newFile.Path] = newFile;
        foreach(var oldFile in readyBackupDetails.RemovableElements)
            _backedUpElements.Remove(oldFile);
    }

    public string GenerateUniqueID() {
        using var md5 = MD5.Create();
        return md5.ComputeHash(Encoding.UTF8.GetBytes(Path)).ToHexString();
    }
}