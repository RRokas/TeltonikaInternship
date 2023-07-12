using System.Collections.Generic;
using System.Linq;
using Core.Entities;
using Spectre.Console;

namespace CLI
{
    public class ParameterComparisonTable
    {
        private List<ParameterComparisonRow> Rows {get;}
        private List<ParameterComparisonRow> FilteredRows {get; set;}

        public ParameterComparisonTable(Comparison comparison)
        {
            Rows = comparison.Results.Select(CreateTableRow).ToList();
            FilteredRows = Rows;
        }
        
        public void ApplyParameterNameFilter(string subStringToSearchFor)
        {
            FilteredRows = Rows.Where(x=>x.ParameterName.StartsWith(subStringToSearchFor)).ToList();
        }
        
        public void ApplyResultFilter(ComparisonResult result)
        {
            FilteredRows = Rows.Where(x=>x.Result == result.ToString()).ToList();
        }

        public Table BuildTable()
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