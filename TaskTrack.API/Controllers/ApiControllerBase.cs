using Microsoft.AspNetCore.Mvc;

namespace TaskTrack.API.Controllers;

public abstract class ApiControllerBase : ControllerBase
{
    protected async Task<ActionResult<T>> Run<T>(Func<Task<T>> action)
    {
        try { return Ok(await action()); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }

    protected async Task<IActionResult> Run(Func<Task> action)
    {
        try { await action(); return NoContent(); }
        catch (KeyNotFoundException ex) { return NotFound(new { message = ex.Message }); }
        catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
    }
}
