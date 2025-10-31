namespace Domain.DTOs;

public class ChequeDto
{
    public long Id { get; set; }
    public string? Number { get; set; }
    public decimal Amount { get; set; }
}