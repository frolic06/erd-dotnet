using Npgsql;
using Dapper;
namespace erd_dotnet;

public record AssociatedTable(string table1, string table2);

public class PostgresReader
{
    private readonly string _connectionString;

    public PostgresReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<List<string>> GetErdData()
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var tables = await connection.QueryAsync<string>(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_type = 'BASE TABLE';"
        );

        var erdFile = new List<string>
        {
            "header {size: \"20\", color: \"#3366ff\"}",
            "entity {bgcolor: \"#ececfc\", size: \"20\"}",
            ""
        };

        var associatedTables = new List<AssociatedTable>();
        foreach (var tableName in tables)
        {
            erdFile.Add($"[{tableName}]");

            var columns = await connection.QueryAsync<string>(
                $"SELECT column_name FROM information_schema.columns WHERE table_name = @tableName;", new { tableName }
            );
            var primaryKeys = await GetPrimaryKeys(connection, tableName);
            var foreignKeys = await GetForeignKeys(connection, tableName);
            foreach (var column in columns)
            {
                var prefix = "";
                if (primaryKeys.Contains(column))
                {
                    prefix += "*";
                }
                if (foreignKeys.Any(fk => fk.ColumnName == column))
                {
                    prefix += "+";
                }
                erdFile.Add($"{prefix}{column}");
            }
            erdFile.Add("");
            associatedTables.AddRange(foreignKeys.Select(fk => new AssociatedTable(tableName, fk.ForeignTableName!)));
        }
        erdFile.AddRange(associatedTables.Select(t => $"{t.table1} 1--* {t.table2}"));
        return erdFile;
    }

    private static async Task<IEnumerable<string>> GetPrimaryKeys(NpgsqlConnection connection, string tableName)
    {
        return await connection.QueryAsync<string>(
                $@"SELECT kcu.column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage kcu
                  ON tc.constraint_name = kcu.constraint_name
                  AND tc.table_schema = kcu.table_schema
                WHERE tc.constraint_type = 'PRIMARY KEY'
                  AND tc.table_name = @tableName;", new { tableName });
    }

    private static async Task<IEnumerable<(string ColumnName, string ForeignTableName, string ForeignColumnName)>> GetForeignKeys(NpgsqlConnection connection, string tableName)
    {
        return await connection.QueryAsync<(string ColumnName, string ForeignTableName, string ForeignColumnName)>(
            $@"
                SELECT distinct kcu.column_name, ccu.table_name AS foreign_table_name, ccu.column_name AS foreign_column_name
                FROM information_schema.table_constraints AS tc
                JOIN information_schema.key_column_usage AS kcu
                  ON tc.constraint_name = kcu.constraint_name
                  AND tc.table_schema = kcu.table_schema
                JOIN information_schema.constraint_column_usage AS ccu
                  ON ccu.constraint_name = tc.constraint_name
                WHERE tc.constraint_type = 'FOREIGN KEY'
                  AND tc.table_name = @tableName;
                ", new { tableName }
        );
    }
}
