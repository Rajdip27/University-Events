using Microsoft.AspNetCore.Http;
using OfficeOpenXml;
using System.Reflection;

namespace UniversityEvents.Application.Imports;

public interface IExcelImportService
{
    Task<List<T>> ReadExcelAsync<T>(IFormFile file) where T : class, new();
}
public class ExcelImportService : IExcelImportService
{
    public async Task<List<T>> ReadExcelAsync<T>(IFormFile file) where T : class, new()
    {
        using var stream = new MemoryStream();
        await file.CopyToAsync(stream);

        using var package = new ExcelPackage(stream);
        var ws = package.Workbook.Worksheets.First();

        var headers = Enumerable.Range(1, ws.Dimension.Columns)
                                .Select(c => ws.Cells[1, c].Text.Trim())
                                .ToArray();

        return Enumerable.Range(2, ws.Dimension.Rows - 1)
                         .Select(r => {
                             var obj = new T();
                             headers.Select((h, i) => {
                                 var prop = typeof(T).GetProperty(h, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                                 if (prop != null && !string.IsNullOrWhiteSpace(ws.Cells[r, i + 1].Text))
                                 {
                                     var targetType = Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType;
                                     var value = Convert.ChangeType(ws.Cells[r, i + 1].Text, targetType);
                                     prop.SetValue(obj, value);
                                 }
                                 return 0;
                             }).ToArray();
                             return obj;
                         }).ToList();
    }
}