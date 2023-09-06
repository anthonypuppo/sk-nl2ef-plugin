using System.Data;
using System.Globalization;
using System.Text;
using CsvHelper;

namespace AnthonyPuppo.SemanticKernel.NL2EF.Extensions;

public static class DataTableExtensions
{
    public static string GetCsv(this DataTable dataTable)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream);
        using var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture);

        foreach (DataColumn column in dataTable.Columns)
        {
            csvWriter.WriteField(column.ColumnName);
        }

        csvWriter.NextRecord();

        foreach (DataRow row in dataTable.Rows)
        {
            for (var i = 0; i < dataTable.Columns.Count; i++)
            {
                csvWriter.WriteField(row[i]);
            }

            csvWriter.NextRecord();
        }

        streamWriter.Flush();

        var bytes = memoryStream.ToArray();
        var csv = Encoding.UTF8.GetString(bytes);

        return csv;
    }
}
