namespace Domain.DTOs;

public class ResponseMessage
{
    public bool Success { get; set; }
    public string Messages { get; set; } = string.Empty;
}
