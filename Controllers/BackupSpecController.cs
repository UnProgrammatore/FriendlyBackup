using FriendlyBackup.BackgroundWorkers;
using FriendlyBackup.BackupManagement;
using FriendlyBackup.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace FriendlyBackup.Controllers;

[ApiController]
[Route("[controller]")]
public class BackupSpecController : ControllerBase
{
    private readonly BackupSpecs _backupSpecs;
    private readonly ILongRunningRequestsRunner _longRunningRequestsRunner;
    private readonly IBackupConnector _backupConnector;


    public BackupSpecController(BackupSpecs backupSpecs, ILongRunningRequestsRunner longRunningRequestsRunner, IBackupConnector backupConnector)
    {
        _backupSpecs = backupSpecs;
        _longRunningRequestsRunner = longRunningRequestsRunner;
        _backupConnector = backupConnector;
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

    [HttpPost("{uniqueId}/alignment")]
    public ActionResult<string> PostAlignment([FromRoute] string uniqueId)
    {
        if(!_backupSpecs.SpecExists(uniqueId))
            return NotFound();
        var spec = _backupSpecs.GetSpec(uniqueId);
        var id = _longRunningRequestsRunner.RunLongRunningTask(cancellationToken => _backupConnector.PerformBackupAsync(spec, spec.GetDiffBackup()));
        return Ok(id);
    }
    [HttpGet("alignment/{taskId}")]
    public ActionResult<bool> GetAlignment([FromRoute] string taskId)
    {
        if(!_longRunningRequestsRunner.ExistsTask(taskId))
            return NotFound();
        (var executed, _) = _longRunningRequestsRunner.GetTaskResult(taskId);
        if(executed)
            return Ok(executed);
        else
            return Accepted();
    }

    [HttpPost("{uniqueId}/verification")]
    public ActionResult<string> PostVerification([FromRoute] string uniqueId)
    {
        if(!_backupSpecs.SpecExists(uniqueId))
            return NotFound();
        var spec = _backupSpecs.GetSpec(uniqueId);
        var id = _longRunningRequestsRunner.RunLongRunningTask(cancellationToken => _backupConnector.CompareBackupAsync(spec));
        return Ok(id);
    }

    [HttpGet("verification/{taskId}")]
    public ActionResult<bool> GetVerification([FromRoute] string taskId)
    {
        if(!_longRunningRequestsRunner.ExistsTask(taskId))
            return NotFound();
        var (executed, result) = _longRunningRequestsRunner.GetTaskResult(taskId);
        if(executed)
            return Ok(result);
        else
            return Accepted();
    }
}
