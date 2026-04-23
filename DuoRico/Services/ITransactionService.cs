using DuoRico.DTOs;

namespace DuoRico.Services;

public interface ITransactionService
{
    Task<List<TransactionDto>> GetCoupleTransactionsForPeriodAsync(int month, int year);
    Task<TransactionSummaryDto> GetSummaryForPeriodAsync(Guid coupleId, int month, int year);
}
