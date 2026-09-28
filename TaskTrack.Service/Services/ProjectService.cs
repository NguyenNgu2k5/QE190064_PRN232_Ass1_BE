using Microsoft.EntityFrameworkCore;
using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Dtos;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class ProjectService(ProjectRepository repo, DepartmentRepository departments) : IProjectService
{
    public async Task<IReadOnlyList<ProjectResponse>> GetAllAsync(CancellationToken ct) => (await repo.GetActiveAsync(ct)).Select(Map).ToList();
    public async Task<ProjectResponse> GetByIdAsync(int id, CancellationToken ct) => Map(await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Project not found."));
    public async Task<IReadOnlyList<ProjectResponse>> GetByDepartmentAsync(int departmentId, CancellationToken ct) => (await repo.GetByDepartmentAsync(departmentId, ct)).Select(Map).ToList();

    public async Task<IReadOnlyList<ProjectResponse>> SearchAsync(string? name, short? status, int? departmentId, CancellationToken ct)
    {
        var query = repo.Query();
        if (!string.IsNullOrWhiteSpace(name)) query = query.Where(x => EF.Functions.ILike(x.ProjectName, $"%{name.Trim()}%"));
        if (status.HasValue) query = query.Where(x => x.Status == status.Value);
        if (departmentId.HasValue) query = query.Where(x => x.DepartmentId == departmentId.Value);
        return (await query.OrderByDescending(x => x.CreatedDate).ToListAsync(ct)).Select(Map).ToList();
    }

    public async Task<ProjectResponse> CreateAsync(ProjectRequest request, CancellationToken ct)
    {
        if (!await departments.ExistsAsync(request.DepartmentId, ct)) throw new InvalidOperationException("Department does not exist.");
        ValidateDates(request.StartDate, request.EndDate);
        var item = new Project { ProjectName = request.ProjectName.Trim(), Description = request.Description?.Trim(), StartDate = request.StartDate, EndDate = request.EndDate, Status = request.Status, DepartmentId = request.DepartmentId };
        await repo.AddAsync(item, ct); await repo.SaveAsync(ct); return await GetByIdAsync(item.ProjectId, ct);
    }

    public async Task<ProjectResponse> UpdateAsync(int id, ProjectRequest request, CancellationToken ct)
    {
        var item = await repo.FindTrackedAsync(id, ct) ?? throw new KeyNotFoundException("Project not found.");
        if (!await departments.ExistsAsync(request.DepartmentId, ct)) throw new InvalidOperationException("Department does not exist.");
        ValidateDates(request.StartDate, request.EndDate);
        item.ProjectName = request.ProjectName.Trim(); item.Description = request.Description?.Trim(); item.StartDate = request.StartDate; item.EndDate = request.EndDate; item.Status = request.Status; item.DepartmentId = request.DepartmentId;
        await repo.SaveAsync(ct); return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var item = await repo.FindTrackedAsync(id, ct) ?? throw new KeyNotFoundException("Project not found.");
        if (await repo.HasTasksAsync(id, ct)) throw new InvalidOperationException("Project cannot be deleted while tasks are linked.");
        repo.Remove(item); await repo.SaveAsync(ct);
    }

    private static void ValidateDates(DateOnly start, DateOnly? end) { if (end.HasValue && end < start) throw new InvalidOperationException("EndDate cannot be before StartDate."); }
    private static ProjectResponse Map(Project x) => new(x.ProjectId, x.ProjectName, x.Description, x.StartDate, x.EndDate, x.Status, x.DepartmentId, x.Department?.DepartmentName ?? string.Empty, x.IsActive, x.CreatedDate, x.Tasks.Select(t => new TaskSummary(t.TaskId, t.Title, t.Status, t.Priority, t.DueDate)).ToList());
}
