using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;

namespace TaskTrack.Repo.Repositories;

public class ProjectRepository(TaskManagementDbContext db)
{
    public Task<List<Project>> GetActiveAsync(CancellationToken ct = default) =>
        db.Projects.AsNoTracking().Include(x => x.Department).Where(x => x.IsActive).OrderByDescending(x => x.CreatedDate).ToListAsync(ct);

    public Task<Project?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.Projects.AsNoTracking().Include(x => x.Department).Include(x => x.Tasks.Where(t => t.IsActive)).ThenInclude(x => x.TaskTags).ThenInclude(x => x.Tag).FirstOrDefaultAsync(x => x.ProjectId == id, ct);

    public Task<List<Project>> GetByDepartmentAsync(int departmentId, CancellationToken ct = default) =>
        db.Projects.AsNoTracking().Include(x => x.Department).Where(x => x.IsActive && x.DepartmentId == departmentId).OrderByDescending(x => x.CreatedDate).ToListAsync(ct);

    public IQueryable<Project> Query() => db.Projects.AsNoTracking().Include(x => x.Department).Where(x => x.IsActive);
    public Task<bool> ExistsAsync(int id, CancellationToken ct = default) => db.Projects.AnyAsync(x => x.ProjectId == id, ct);
    public Task<bool> HasTasksAsync(int id, CancellationToken ct = default) => db.Tasks.AnyAsync(x => x.ProjectId == id, ct);
    public Task AddAsync(Project item, CancellationToken ct = default) => db.Projects.AddAsync(item, ct).AsTask();
    public void Remove(Project item) => db.Projects.Remove(item);
    public Task<Project?> FindTrackedAsync(int id, CancellationToken ct = default) => db.Projects.FirstOrDefaultAsync(x => x.ProjectId == id, ct);
    public Task SaveAsync(CancellationToken ct = default) => db.SaveChangesAsync(ct);
}
