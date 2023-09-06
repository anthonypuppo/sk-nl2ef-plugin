using System.Collections;
using System.Data;
using System.Data.Common;
using Dapper;

namespace AnthonyPuppo.SemanticKernel.NL2EF.Extensions;

public static class DbConnectionExtensions
{
    public static async Task<DataSet> QueryMultipleToDataSetAsync(this DbConnection dbConnection, string sql)
    {
        using var gridReader = await dbConnection.QueryMultipleAsync(sql);
        var dataSet = new DataSet();

        while (!gridReader.IsConsumed)
        {
            var dataTable = new DataTable();
            var rows = await gridReader.ReadAsync();

            if (rows is IEnumerable && rows.Any())
            {
                var firstRow = (IDictionary<string, object>)rows.First();

                foreach (var property in firstRow.Keys)
                {
                    var columnName = property;

                    if (dataTable.Columns.Contains(columnName))
                    {
                        columnName += $"_{Guid.NewGuid():N}";
                    }

                    dataTable.Columns.Add(property);
                }

                foreach (var row in rows.Cast<IDictionary<string, object>>())
                {
                    dataTable.Rows.Add(row.Values.ToArray());
                }
            }

            dataSet.Tables.Add(dataTable);
        }

        return dataSet;
    }
}
