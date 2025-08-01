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

    [Required(ErrorMessage = "A descrição é obrigatória.")]
    [Display(Name = "Descrição")]
    public string Description { get; set; }

    [Required(ErrorMessage = "O valor é obrigatório.")]
    [Display(Name = "Valor")]
    [Column(TypeName = "numeric(12, 2)")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    [Display(Name = "Categoria")]
    public string Category { get; set; }

    public TransactionType Type { get; set; }
    
    [Display(Name = "Pago?")]
    public bool IsPaid { get; set; } = false;

    public int TotalInstallments { get; set; }

    public int InstallmentNumber { get; set; }

    public Guid? InstallmentGroupId { get; set; }

    [Required(ErrorMessage = "O mês é obrigatório.")]
    [Display(Name = "Mês")]
    [Range(1, 12)]
    public int Month { get; set; }

    [Required(ErrorMessage = "O ano é obrigatório.")]
    [Display(Name = "Ano")]
    [Range(2025, 2100)]
    public int Year { get; set; }

    [Display(Name = "Data de criação")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public string? UserId { get; set; }

    public virtual ApplicationUser? User { get; set; }
}