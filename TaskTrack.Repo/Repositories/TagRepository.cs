using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TagRepository(TaskManagementDbContext db)
{
    public Task<List<Tag>> GetAllAsync(CancellationToken ct = default) => db.Tags.AsNoTracking().OrderBy(x => x.TagName).ToListAsync(ct);
    public Task<Tag?> GetByIdAsync(int id, CancellationToken ct = default) => db.Tags.AsNoTracking().FirstOrDefaultAsync(x => x.TagId == id, ct);
    public Task<bool> ExistsAsync(int id, CancellationToken ct = default) => db.Tags.AnyAsync(x => x.TagId == id, ct);
    public Task<bool> HasTasksAsync(int id, CancellationToken ct = default) => db.TaskTags.AnyAsync(x => x.TagId == id, ct);
    public Task AddAsync(Tag item, CancellationToken ct = default) => db.Tags.AddAsync(item, ct).AsTask();
    public Task<Tag?> FindTrackedAsync(int id, CancellationToken ct = default) => db.Tags.FirstOrDefaultAsync(x => x.TagId == id, ct);
    public void Remove(Tag item) => db.Tags.Remove(item);
    public Task SaveAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
