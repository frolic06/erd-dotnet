using Npgsql;
using Dapper;
namespace erd_dotnet;

public class PostgresReader
{
    private readonly string _connectionString;

    public PostgresReader(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<Erd> FetchErdEntities(Erd erd)
    {
        using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        var tables = await connection.QueryAsync<string>(
            "SELECT table_name FROM information_schema.tables WHERE table_schema = 'public' and table_type = 'BASE TABLE';"
        );

        foreach (var tableName in tables)
        {
            var entity = erd.AddOrUpdateEntity(tableName);

            var columns = await connection.QueryAsync<string>(
                $"SELECT column_name FROM information_schema.columns WHERE table_name = @tableName;", new { tableName }
            );
            var primaryKeys = await GetPrimaryKeys(connection, tableName);
            var foreignKeys = await GetForeignKeys(connection, tableName);
            foreach (var column in columns)
            {
                var attribute = new Attribute(column);
                attribute.IsPK = primaryKeys.Contains(column);
                attribute.IsFK = foreignKeys.Any(fk => fk.ColumnName == column);
                entity.AddOrUpdateAttribute(attribute);
            }
            foreach (var (_, foreignTableName, _) in foreignKeys)
            {
                erd.AddMissingRelationships(new Relationship() { Name1 = tableName, Label1 = "*", Name2 = foreignTableName, Label2 = "*" });
            }
            var removed = entity.Fields.RemoveAll(f => !columns.Contains(f.Name));
        }
        var removedTbl = erd.Entities.RemoveAll(e => !tables.Contains(e.Title));
        erd.Relationships.RemoveAll(r => !tables.Contains(r.Name1) || !tables.Contains(r.Name2));
        return erd;
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
