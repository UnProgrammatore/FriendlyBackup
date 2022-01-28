using FriendlyBackup.Repositories;

namespace FriendlyBackup.BackupManagement;

public class BackupSpecs 
{
    private readonly IBackupRepository _backupRepository;
    public IEnumerable<BackupSpec> AvailableSpecs 
    {
        get 
        {
            InitIfNeeded();
            return _specsByUniqueId!.Values;;
        }
    }
    private readonly object _initLockObject = new();
    private Dictionary<string, BackupSpec>? _specsByUniqueId;
    public BackupSpecs(IBackupRepository backupRepository)
    {
        _backupRepository = backupRepository;
    }

    private void InitIfNeeded()
    {
        if(_specsByUniqueId == null)
        {
            lock(_initLockObject) 
            {
                if(_specsByUniqueId == null) 
                {
                    _specsByUniqueId = _backupRepository.GetAllSpecs().ToDictionary(spec => spec.GenerateUniqueID());
                }
            }
        }
    }

    public IEnumerable<BackupSpec> GetAllSpecs()
    {
        InitIfNeeded();
        return _specsByUniqueId!.Values;
    }

    public BackupSpec GetSpec(string uniqueId) 
    {
        InitIfNeeded();
        if(!_specsByUniqueId!.ContainsKey(uniqueId))
            throw new ArgumentException($"Spec with uniqueId {uniqueId} does not exist");
        return _specsByUniqueId![uniqueId];
    }

    public void ReplaceSpec(BackupSpec spec)
    {
        InitIfNeeded();
        var uniqueId = spec.GenerateUniqueID();
        if(!_specsByUniqueId!.ContainsKey(uniqueId))
            throw new ArgumentException($"Spec with uniqueId {spec.GenerateUniqueID()} does not exist");
        _specsByUniqueId![uniqueId] = spec;
        _backupRepository.SaveSpec(spec);

    }
    public void AddSpec(BackupSpec spec)
    {
        InitIfNeeded();
        var uid = spec.GenerateUniqueID();
        if(_specsByUniqueId!.ContainsKey(uid))
            throw new ArgumentException("Spec for the specified path already exists");
        _specsByUniqueId.Add(uid, spec);
        _backupRepository.SaveSpec(spec);
    }

    public bool SpecExists(string uniqueId)
    {
        InitIfNeeded();
        return _specsByUniqueId!.ContainsKey(uniqueId);
    }
}