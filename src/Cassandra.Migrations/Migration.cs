namespace Cassandra.Migrations;

public sealed class Migration
{
    public string Version { get; set; }
    public string Name { get; set; }
    public string Cql { get; set; }

    public Migration()
    {
    }

    public Migration(string fullName, string cql)
    {
        Version = fullName[..fullName.IndexOf('_')];
        Name = fullName;
        Cql = cql;
    }
}