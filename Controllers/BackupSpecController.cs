using FriendlyBackup.BackupManagement;
using FriendlyBackup.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FriendlyBackup.Controllers;

[ApiController]
[Route("[controller]")]
public class BackupSpecController : ControllerBase
{
    private readonly BackupSpecs _backupSpecs;



    public BackupSpecController(BackupSpecs backupSpecs)
    {
        _backupSpecs = backupSpecs;
    }

    [HttpGet]
    public ActionResult<IDictionary<string, string>> Get()
    {
        return Ok(_backupSpecs.GetAllSpecs().ToDictionary(a => a.GenerateUniqueID(), a => a.Path));
    }

    [HttpGet("{uniqueId}")]
    public ActionResult<BackupSpec> Get([FromRoute] string uniqueId)
    {
        if(!_backupSpecs.SpecExists(uniqueId))
            return NotFound();
        return Ok(_backupSpecs.GetSpec(uniqueId));
    }

    [HttpPost]
    public ActionResult Post([FromBody] string path)
    {
        var uniqueId = BackupSpec.GenerateUniqueID(path);
        if(_backupSpecs.SpecExists(uniqueId))
            return BadRequest("Spec for the specified path already exists");
        var spec = new BackupSpec(path);
        spec.Initialize();
        _backupSpecs.AddSpec(spec);
        return Ok();
    }

    [HttpGet("{uniqueId}/diff")]
    public ActionResult<ReadyBackupDetails> GetDiff([FromRoute] string uniqueId)
    {
        if(!_backupSpecs.SpecExists(uniqueId))
            return NotFound();
        var spec = _backupSpecs.GetSpec(uniqueId);
        var diff = spec.GetDiffBackup();
        return Ok(diff);
    }
}
