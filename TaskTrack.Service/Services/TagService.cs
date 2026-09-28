using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Dtos;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class TagService(TagRepository repo) : ITagService
{
    public async Task<IReadOnlyList<TagResponse>> GetAllAsync(CancellationToken ct) => (await repo.GetAllAsync(ct)).Select(Map).ToList();

    public async Task<TagResponse> CreateAsync(TagRequest request, CancellationToken ct)
    {
        var name = request.TagName.Trim();
        if (name.Length == 0) throw new InvalidOperationException("TagName is required.");
        if (await repo.NameExistsAsync(name, ct: ct)) throw new InvalidOperationException("TagName already exists.");
        var item = new Tag { TagName = name, Color = request.Color?.Trim() };
        await repo.AddAsync(item, ct); await repo.SaveAsync(ct); return Map(item);
    }

    public async Task<TagResponse> UpdateAsync(int id, TagRequest request, CancellationToken ct)
    {
        var item = await repo.FindTrackedAsync(id, ct) ?? throw new KeyNotFoundException("Tag not found.");
        var name = request.TagName.Trim();
        if (name.Length == 0) throw new InvalidOperationException("TagName is required.");
        if (await repo.NameExistsAsync(name, id, ct)) throw new InvalidOperationException("TagName already exists.");
        item.TagName = name; item.Color = request.Color?.Trim(); await repo.SaveAsync(ct); return Map(item);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var item = await repo.FindTrackedAsync(id, ct) ?? throw new KeyNotFoundException("Tag not found.");
        if (await repo.HasTasksAsync(id, ct)) throw new InvalidOperationException("Tag cannot be deleted while tasks use it.");
        repo.Remove(item); await repo.SaveAsync(ct);
    }

    private static TagResponse Map(Tag x) => new(x.TagId, x.TagName, x.Color);
}
