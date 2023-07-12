using System;
using System.IO;
using Core.Entities;
using Spectre.Console;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceConfigFile = new FileInfo(args[0]);
            var targetConfigFile = new FileInfo(args[1]);
            CompareConfigs(sourceConfigFile, targetConfigFile); 
        }
        
        private static void CompareConfigs(FileInfo sourceConfigFile, FileInfo targetConfigFile)
        {
            var source = new DeviceConfiguration(sourceConfigFile);
            var target = new DeviceConfiguration(targetConfigFile);
            var comparison = new Comparison(source, target);
            var results = comparison.Results;
            var table = new Table().Title("Configuration comparison results");
            table.AddColumn("Parameter");
            table.AddColumn("Source value");
            table.AddColumn("Target value");
            table.AddColumn("Comparison result");
            foreach (var result in results)
            {
                var row = CreateTableRow(result);
                table.AddRow(row.ParameterName, row.SourceValue, row.TargetValue, row.Result);
            }
            AnsiConsole.Write(table);
        }

        private static (string ParameterName, string SourceValue, string TargetValue, string Result) CreateTableRow(ConfigurationParameterComparison comparisonResult)
        {
            var parameterName = comparisonResult.Source?.Id.ToString() ?? comparisonResult.Target?.Id.ToString();
            var sourceValue = comparisonResult.Source?.Value.ToString() ?? "";
            var targetValue = comparisonResult.Target?.Value.ToString() ?? "";
            var comparisonResultString = comparisonResult.Result switch
            {
                ComparisonResult.Added => "[red]Added[/]",
                ComparisonResult.Removed => "[red]Removed[/]",
                ComparisonResult.Modified => "[yellow]Modified[/]",
                ComparisonResult.Unchanged => "[green]Unchanged[/]",
                _ => throw new ArgumentOutOfRangeException()
            };
            return (parameterName, sourceValue, targetValue, comparisonResultString);
        }
    }
}