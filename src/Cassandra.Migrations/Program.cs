using System.CommandLine;
using Cassandra.Migrations.Handlers;
using Spectre.Console;

namespace Cassandra.Migrations;

public static class Program
{
    public static async Task Main(string[] args)
    {
        AnsiConsole.Write(
            new FigletText("Cassandra Migrations")
                .Centered()
                .Color(Color.Fuchsia));

        var rootCommand = new RootCommand("The tool to apply Apache Cassandra schema migrations")
            { MigrateCommandHandler.Build() };

        await rootCommand.InvokeAsync(args);
    }
}