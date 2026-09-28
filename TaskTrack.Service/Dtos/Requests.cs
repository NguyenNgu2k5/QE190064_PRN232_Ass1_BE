using System.ComponentModel.DataAnnotations;

namespace TaskTrack.Service.Dtos;

public class DepartmentRequest
{
    [Required, StringLength(100)] public string DepartmentName { get; set; } = string.Empty;
    [Required, StringLength(300)] public string DepartmentDescription { get; set; } = string.Empty;
}

public class ProjectRequest
{
    [Required, StringLength(200)] public string ProjectName { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required] public DateOnly StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    [Range(0, 3)] public short Status { get; set; }
    [Range(1, int.MaxValue)] public int DepartmentId { get; set; }
}

public class TaskRequest
{
    [Required, StringLength(300)] public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Range(0, 3)] public short Status { get; set; }
    [Range(0, 3)] public short Priority { get; set; } = 1;
    public DateOnly? DueDate { get; set; }
    [Range(1, int.MaxValue)] public int ProjectId { get; set; }
    public List<int> TagIds { get; set; } = [];
}

public class TagRequest
{
    [Required, StringLength(50)] public string TagName { get; set; } = string.Empty;
    [RegularExpression("^$|^#[0-9A-Fa-f]{6}$", ErrorMessage = "Color must be a 6-digit hex value.")] public string? Color { get; set; }
}
