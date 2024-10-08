using Cassandra.Mapping;
using Spectre.Console;

namespace Cassandra.Migrations;

public sealed class MigrationsReader(string keyspace, string scriptsPath)
{
    public async IAsyncEnumerable<(Migration migrations, bool applied)> ReadMigrationScripts(HashSet<string> appliedMigrations)
    {
        AnsiConsole.MarkupLine($"Reading migrations from scripts path: {scriptsPath}");
            
        var directory = new DirectoryInfo(scriptsPath);

        if (!directory.Exists)
        {
            var exception = new DirectoryNotFoundException(scriptsPath);
            AnsiConsole.WriteException(new DirectoryNotFoundException(scriptsPath));
            
            throw exception;
        }

        var files = directory
            .GetFiles("*.cql")
            .OrderBy(f => f.Name);

        foreach (var file in files)
        {
            AnsiConsole.MarkupLine($"Reading migration file: {file.FullName}");
            
            //TODO Add name validation
            var cql = await File.ReadAllTextAsync(file.FullName);

            var applied = appliedMigrations.Contains(file.Name);
            
            yield return new ValueTuple<Migration, bool>(new Migration(file.Name, cql), applied);
        }
    }
    
    public async Task<IEnumerable<Migration>> ReadAppliedMigrations(ISession session)
    {
        AnsiConsole.MarkupLine("Reading migrations table...");

        var keyspaceMetadata = session.Cluster.Metadata.GetKeyspace(keyspace);
        var tableNames = keyspaceMetadata.GetTablesNames();
        if (!tableNames.Contains(Constants.MigrationsTableName))
        {
            AnsiConsole.MarkupLine($"There is no migration table in keyspace, creating: {Constants.MigrationsTableName}");

            await session.ExecuteAsync(new SimpleStatement(
                $"CREATE TABLE {Constants.MigrationsTableName} (version text PRIMARY KEY, name text, cql text, time timestamp)"));

            return Enumerable.Empty<Migration>();
        }

        AnsiConsole.MarkupLine("Reading applied migrations...");

        var mapper = new Mapper(session);
       
        return await mapper.FetchAsync<Migration>($"SELECT * FROM {Constants.MigrationsTableName}");
    }
}