using DuoRico.Data;
using DuoRico.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace DuoRico.Services;

public class TransactionService
{
    private readonly ApplicationDbContext _context;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UserManager<ApplicationUser> _userManager;

    public TransactionService(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _httpContextAccessor = httpContextAccessor;
        _userManager = userManager;
    }

    public async Task<List<Transaction>> GetCoupleTransactionsAsync()
    {
        var currentUser = await GetCurrentUserAsync();

        if (currentUser?.CoupleId == null) return new List<Transaction>();

        return await _context.Transactions
            .Where(t => t.User.CoupleId == currentUser.CoupleId)
            .ToListAsync();
    }

    public async Task<bool> CreateTransactionAsync(Transaction transaction)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null || currentUser.CoupleId == null)
            return false;

        transaction.Id = Guid.NewGuid();
        transaction.UserId = currentUser.Id;
        transaction.CreatedAt = DateTime.UtcNow;
        // Outras validações...

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UpdateTransactionAsync(Transaction transaction)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return false;

        var existing = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transaction.Id && t.User.CoupleId == currentUser.CoupleId);

        if (existing == null) return false;

        // Atualize apenas os campos permitidos
        existing.Description = transaction.Description;
        existing.Amount = transaction.Amount;
        existing.Category = transaction.Category;
        existing.Type = transaction.Type;
        existing.IsPaid = transaction.IsPaid;
        // Outras atualizações...

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteTransactionAsync(Guid transactionId)
    {
        var currentUser = await GetCurrentUserAsync();
        if (currentUser == null) return false;

        var transaction = await _context.Transactions
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.User.CoupleId == currentUser.CoupleId);

        if (transaction == null) return false;

        _context.Transactions.Remove(transaction);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<ApplicationUser?> GetCurrentUserAsync()
    {
        var principal = _httpContextAccessor.HttpContext?.User;

        if (principal == null) return null;

        return await _userManager.GetUserAsync(principal);
    }

    // Buscar transações do casal autenticado por filtro (mês e ano)
    public async Task<List<Transaction>> GetCoupleTransactionsForPeriodAsync(int month, int year)
    {
        var currentUser = await GetCurrentUserAsync();

        if (currentUser?.CoupleId == null)
            return new List<Transaction>();

        return await _context.Transactions
            .Where(t => t.User.CoupleId == currentUser.CoupleId &&
                        t.CreatedAt.Month == month &&
                        t.CreatedAt.Year == year)
            .ToListAsync();
    }

    // Soma dos valores das transações do casal autenticado por filtro (mês e ano)
    public class TransactionSummary
    {
        public decimal TotalIncome { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal Balance => TotalIncome - TotalExpense;
    }

    public async Task<TransactionSummary> GetSummaryForPeriodAsync(Guid coupleId, int month, int year)
    {
        // Calcula a soma das receitas diretamente no banco de dados
        var totalIncome = await _context.Transactions
            .Where(t => t.User.CoupleId == coupleId &&
                        t.Type == TransactionType.Income &&
                        t.Month == month &&
                        t.Year == year)
            .SumAsync(t => t.Amount);

        // Calcula a soma das despesas diretamente no banco de dados
        var totalExpense = await _context.Transactions
            .Where(t => t.User.CoupleId == coupleId &&
                        t.Type == TransactionType.Expense &&
                        t.Month == month &&
                        t.Year == year)
            .SumAsync(t => t.Amount);

        return new TransactionSummary
        {
            TotalIncome = totalIncome,
            TotalExpense = totalExpense
        };
    }
}