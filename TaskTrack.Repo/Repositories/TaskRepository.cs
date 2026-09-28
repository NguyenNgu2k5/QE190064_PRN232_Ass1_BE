using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class TaskRepository(TaskManagementDbContext db)
{
    public IQueryable<TaskItem> Query() => db.Tasks.AsNoTracking().Include(x => x.Project).ThenInclude(x => x.Department).Include(x => x.TaskTags).ThenInclude(x => x.Tag).Where(x => x.IsActive);
    public Task<TaskItem?> GetByIdAsync(int id, CancellationToken ct = default) => Query().FirstOrDefaultAsync(x => x.TaskId == id, ct);
    public Task<List<TaskItem>> GetByProjectAsync(int projectId, CancellationToken ct = default) => Query().Where(x => x.ProjectId == projectId).OrderBy(x => x.DueDate).ToListAsync(ct);
    public Task<bool> ExistsAsync(int id, CancellationToken ct = default) => db.Tasks.AnyAsync(x => x.TaskId == id, ct);
    public Task AddAsync(TaskItem item, CancellationToken ct = default) => db.Tasks.AddAsync(item, ct).AsTask();
    public Task<TaskItem?> FindTrackedAsync(int id, CancellationToken ct = default) => db.Tasks.Include(x => x.TaskTags).FirstOrDefaultAsync(x => x.TaskId == id, ct);
    public Task<List<Tag>> GetTagsAsync(IEnumerable<int> ids, CancellationToken ct = default) => db.Tags.Where(x => ids.Contains(x.TagId)).ToListAsync(ct);
    public async Task ReplaceTagsAsync(int taskId, IEnumerable<int> tagIds, CancellationToken ct = default)
    {
        await db.TaskTags.Where(x => x.TaskId == taskId).ExecuteDeleteAsync(ct);
        db.TaskTags.AddRange(tagIds.Select(tagId => new TaskTag { TaskId = taskId, TagId = tagId }));
    }
    public Task SaveAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
