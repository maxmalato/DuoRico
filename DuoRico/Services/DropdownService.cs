using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace DuoRico.Services;

public class DropdownService : IDropdownService
{
    public SelectList GetMonthOptions()
    {
        var culture = new CultureInfo("pt-BR");
        var textInfo = culture.TextInfo;

        var months = Enumerable.Range(1, 12).Select(i => new
        {
            Value = i,
            Text = textInfo.ToTitleCase(
                new DateTime(2000, i, 1).ToString("MMMM", culture)
            )
        });

        return new SelectList(months, "Value", "Text");
    }

    public SelectList GetYearOptions(int startYear, int numberOfYears)
    {
        var years = Enumerable.Range(startYear, numberOfYears);
        return new SelectList(years);
    }

    public SelectList GetInstallmentOptions(int maxInstallments)
    {
        var installments = Enumerable.Range(1, maxInstallments);
        return new SelectList(installments);
    }
}