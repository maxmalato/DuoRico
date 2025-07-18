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

    public TransactionType Type { get; set; }

    public bool IsPaid { get; set; } = false;

    [Required]
    public int InstallmentNumber { get; set; }

    [Required]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required]
    [Range(2025, 2100)]
    public int Year { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    public virtual ApplicationUser? User { get; set; }
}