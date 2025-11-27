namespace Domain.ViewModels.ApprovalStatuses;

public class ApprovalStatusIndexViewModel
{
    public List<ApprovalStatusListItemViewModel> Items { get; set; } = new();
}

public class ApprovalStatusListItemViewModel
{
    public long ApprovalStatusId { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}

public class ApprovalStatusFormViewModel
{
    public long? ApprovalStatusId { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public long? CreatedByUserId { get; set; }
}

public class ApprovalStatusDto
{
    public long ApprovalStatusId { get; set; }
    public string ApprovalStatusName { get; set; } = string.Empty;
    public string Created { get; set; } = string.Empty;
    public string? CreatedBy { get; set; }
}
