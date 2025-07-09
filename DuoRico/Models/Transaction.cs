using DuoRico.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DuoRico.Models;

public enum TransactionType
{
    Income,
    Expense
}

public class Transaction
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    [Required]
    public string Description { get; set; }

    [Required]
    [Column(TypeName = "numeric(12, 2)")]
    public decimal Amount { get; set; }

    [Required]
    public string Category { get; set; }

    [Required]
    public TransactionType Type { get; set; }

    public bool IsPaid { get; set; } = false;

    // Para recorrência/parcelamento
    public int? InstallmentNumber { get; set; }

    public int? TotalInstallments { get; set; }
    public Guid? RecurringGroupId { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public string UserId { get; set; }

    public virtual ApplicationUser? User { get; set; }
}