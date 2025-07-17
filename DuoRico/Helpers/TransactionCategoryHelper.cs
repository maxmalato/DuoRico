using DuoRico.Models;

namespace DuoRico.Helpers;

public static class TransactionCategoryHelper
{
    public static List<string> GetCategories(TransactionType type)
    {
        return type switch
        {
            TransactionType.Income => new List<string>
            {
                "Investimento",
                "Outros",
                "Presente",
                "Salário",
                "Serviço",
            },
            TransactionType.Expense => new List<string>
            {
                "Cartão de Crédito",
                "Comida",
                "Dívida",
                "Dízimo",
                "Empréstimo",
                "Entretenimento",
                "Moradia",
                "Outros",
                "Saúde",
                "Transporte",
            },
            _ => throw new ArgumentException("Tipo de transação inválido.")
        };
    }
}