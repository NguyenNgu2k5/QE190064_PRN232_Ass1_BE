using Microsoft.AspNetCore.Mvc;
using TaskTrack.Service.Dtos;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.API.Controllers;

[ApiController, Route("api/tags")]
public class TagsController(ITagService service) : ApiControllerBase
{
    [HttpGet] public Task<ActionResult<IReadOnlyList<TagResponse>>> GetAll(CancellationToken ct) => Run(() => service.GetAllAsync(ct));
    [HttpPost] public Task<ActionResult<TagResponse>> Create(TagRequest request, CancellationToken ct) => Run(() => service.CreateAsync(request, ct));
    [HttpPut("{id:int}")] public Task<ActionResult<TagResponse>> Update(int id, TagRequest request, CancellationToken ct) => Run(() => service.UpdateAsync(id, request, ct));
    [HttpDelete("{id:int}")] public Task<IActionResult> Delete(int id, CancellationToken ct) => Run(() => service.DeleteAsync(id, ct));
}
