using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Spectre.Console;

namespace CLI
{
    public class TableBuilder
    {
        private Comparison Comparison {get; }
        private List<ParameterComparisonRow> FilteredRows {get; set;}

        public TableBuilder(Comparison comparison)
        {
            Comparison = comparison ?? throw new System.ArgumentNullException(nameof(comparison));
            FilteredRows = Comparison.Results.Select(CreateTableRow).ToList();
        }
        
        public void ApplyParameterNameFilter(string subStringToSearchFor)
        {
            FilteredRows = Comparison.Results.Where(x=> 
                    GetParameterName(x).StartsWith(subStringToSearchFor))
                .Select(CreateTableRow).ToList();
        }

        private string GetParameterName(ConfigurationParameterComparison parameterComparison)
        {
            return parameterComparison.Source?.Id.ToString() ?? parameterComparison.Target?.Id.ToString() ?? string.Empty;
        }
        
        public void ApplyResultFilter(ComparisonResult result)
        {
            FilteredRows = Comparison.Results.Where(x=>x.Result == result).Select(CreateTableRow).ToList();
        }

        public Table BuildComparisonTable()
        {
            var table = new Table().Title("Parameter comparison");
            table.AddColumn("Parameter");
            table.AddColumn("Source value");
            table.AddColumn("Target value");
            table.AddColumn("Comparison result");

            foreach (var row in FilteredRows)
            {
                table.AddRow(row.ParameterName, row.SourceValue, row.TargetValue, row.Result);
            }

            return table;
        }

        public (Table source, Table target) BuildMetadataTables()
        {
            var sourceTable = new Table().Title("Source");
            var targetTable = new Table().Title("Target");
            sourceTable.AddColumn("Parameter");
            sourceTable.AddColumn("Value");
            targetTable.AddColumn("Parameter");
            targetTable.AddColumn("Value");

            foreach (var parameter in Comparison.Source.Parameters.Where(x => x.IsMetadata))
            {
                sourceTable.AddRow(parameter.Id.ToString() ?? string.Empty, parameter.Value.ToString() ?? string.Empty);
            }

            foreach (var parameter in Comparison.Target.Parameters.Where(x => x.IsMetadata))
            {
                targetTable.AddRow(parameter.Id.ToString() ?? string.Empty, parameter.Value.ToString() ?? string.Empty);
            }
            
            return (sourceTable, targetTable);
        }

        public Table BuildComparisonSummary()
        {
            var table = new Table().Title("Comparison summary");
            table.AddColumn(new TableColumn("Unchanged").Centered());
            table.AddColumn(new TableColumn("[yellow]Modified[/]").Centered());
            table.AddColumn(new TableColumn("[green]Added[/]").Centered());
            table.AddColumn(new TableColumn("[red]Removed[/]").Centered());
            table.AddRow(
                Comparison.Results.Count(x=>x.Result==ComparisonResult.Unchanged).ToString(),
                Comparison.Results.Count(x=>x.Result==ComparisonResult.Modified).ToString(),
                Comparison.Results.Count(x=>x.Result==ComparisonResult.Added).ToString(),
                Comparison.Results.Count(x=>x.Result==ComparisonResult.Removed).ToString()).Centered();
            return table;
        }

        private ParameterComparisonRow CreateTableRow(ConfigurationParameterComparison comparisonResult)
        {
            var parameterName = comparisonResult.Source?.Id.ToString() ?? comparisonResult.Target?.Id.ToString();
            var sourceValue = comparisonResult.Source?.Value.ToString() ?? "";
            var targetValue = comparisonResult.Target?.Value.ToString() ?? "";
            var comparisonResultString = comparisonResult.Result.ToString();
            
            return new ParameterComparisonRow
            {
                ParameterName = ApplyFormatting(parameterName, comparisonResult.Result),
                SourceValue = ApplyFormatting(sourceValue, comparisonResult.Result),
                TargetValue = ApplyFormatting(targetValue, comparisonResult.Result),
                Result = ApplyFormatting(comparisonResultString, comparisonResult.Result)
            };
        }
        
        private string ApplyFormatting(string value, ComparisonResult result)
        {
            if (result == ComparisonResult.Added)
            {
                return $"[green]{value}[/]";
            }
            if (result == ComparisonResult.Removed)
            {
                return $"[red]{value}[/]";
            }
            if (result == ComparisonResult.Modified)
            {
                return $"[yellow]{value}[/]";
            }
            return value;
        }
    }
}