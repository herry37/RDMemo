namespace BackendManagement.Application.Common.Models.Requests;

public class CreateRecoveryPointRequest
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
} 