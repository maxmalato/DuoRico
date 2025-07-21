using DuoRico.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DuoRico.Services;

public interface IDropdownService
{
    SelectList GetMonthOptions();

    SelectList GetYearOptions(int startYear, int numberOfYears);

    SelectList GetInstallmentOptions(int maxInstallments);
}