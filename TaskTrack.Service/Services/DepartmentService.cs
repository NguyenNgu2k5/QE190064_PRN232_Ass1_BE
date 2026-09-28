using TaskTrack.Repo.Models;
using TaskTrack.Repo.Repositories;
using TaskTrack.Service.Dtos;
using TaskTrack.Service.Interfaces;

namespace TaskTrack.Service.Services;

public class DepartmentService(DepartmentRepository repo) : IDepartmentService
{
    public async Task<IReadOnlyList<DepartmentResponse>> GetAllAsync(CancellationToken ct) => (await repo.GetActiveAsync(ct)).Select(Map).ToList();
    public async Task<DepartmentResponse> GetByIdAsync(int id, CancellationToken ct) => Map(await repo.GetByIdAsync(id, ct) ?? throw new KeyNotFoundException("Department not found."));
    public async Task<IReadOnlyList<DepartmentResponse>> SearchAsync(string name, CancellationToken ct) => (await repo.SearchAsync(name, ct)).Select(Map).ToList();

    public async Task<DepartmentResponse> CreateAsync(DepartmentRequest request, CancellationToken ct)
    {
        var item = new Department { DepartmentName = request.DepartmentName.Trim(), DepartmentDescription = request.DepartmentDescription.Trim() };
        await repo.AddAsync(item, ct); await repo.SaveAsync(ct); return await GetByIdAsync(item.DepartmentId, ct);
    }

    public async Task<DepartmentResponse> UpdateAsync(int id, DepartmentRequest request, CancellationToken ct)
    {
        var item = await repo.FindTrackedAsync(id, ct) ?? throw new KeyNotFoundException("Department not found.");
        item.DepartmentName = request.DepartmentName.Trim(); item.DepartmentDescription = request.DepartmentDescription.Trim();
        await repo.SaveAsync(ct); return await GetByIdAsync(id, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct)
    {
        var item = await repo.FindTrackedAsync(id, ct) ?? throw new KeyNotFoundException("Department not found.");
        if (await repo.HasProjectsAsync(id, ct)) throw new InvalidOperationException("Department cannot be deleted while projects are linked.");
        repo.Remove(item); await repo.SaveAsync(ct);
    }

    private static DepartmentResponse Map(Department x) => new(x.DepartmentId, x.DepartmentName, x.DepartmentDescription, x.IsActive, x.Projects.Select(p => new ProjectSummary(p.ProjectId, p.ProjectName, p.Status, p.StartDate, p.EndDate)).ToList());
}
