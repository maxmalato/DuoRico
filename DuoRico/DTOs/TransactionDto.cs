using DuoRico.Models;

namespace DuoRico.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Category { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public bool IsPaid { get; set; }
    public DateTime CreatedAt { get; set; }
    public int InstallmentNumber { get; set; }
    public int TotalInstallments { get; set; }
    public Guid? InstallmentGroupId { get; set; }
}
