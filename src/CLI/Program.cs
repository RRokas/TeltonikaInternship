using System.IO;
using Core.Entities;
using Core.Enums;
using Spectre.Console;

namespace CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var sourceConfigFile = new FileInfo(args[0]);
            var targetConfigFile = new FileInfo(args[1]);
            
            var source = new DeviceConfiguration().LoadFromFile(sourceConfigFile);
            var target = new DeviceConfiguration().LoadFromFile(targetConfigFile);
            
            var comparison = new DeviceConfigurationComparison(source, target);
            
            var tableBuilder = new TableBuilder(comparison);
            
            var choice = AskComparisonType();
            switch (choice)
            {
                case MenuChoice.FilterById:
                {
                    var filterString = AnsiConsole.Ask<string>("Enter substring to search for");
                    tableBuilder.ApplyParameterNameFilter(filterString);
                    break;
                }
                case MenuChoice.FilterByResult:
                {
                    var filter = AskComparisonResultFilter();
                    tableBuilder.ApplyResultFilter(filter);
                    break;
                }
            }
            
            var comparisonTable = tableBuilder.BuildComparisonTable();
            var summaryTable = tableBuilder.BuildComparisonSummary();
            var metadataTables = tableBuilder.BuildMetadataTables();
           
            var rootGrid = new Grid().AddColumn();
            var metadataGrid = new Grid().AddColumns(2).AddRow(metadataTables.source, metadataTables.target);

            rootGrid.AddRow(metadataGrid).Expand();
            rootGrid.AddRow(summaryTable.Expand());
            rootGrid.AddRow(new Padder(comparisonTable.Expand()));

            AnsiConsole.Write(rootGrid);
        }

        static MenuChoice AskComparisonType()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<MenuChoice>()
                    .Title("Action")
                    .AddChoices(new[] {
                        MenuChoice.ShowFullComparison,
                        MenuChoice.FilterById,
                        MenuChoice.FilterByResult
                    }));
        }
        
        static ComparisonResult AskComparisonResultFilter()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<ComparisonResult>()
                    .Title("Select result to filter by")
                    .AddChoices(new[] {
                        ComparisonResult.Unchanged,
                        ComparisonResult.Modified,
                        ComparisonResult.Added,
                        ComparisonResult.Removed
                    }));
        }
        
        private enum MenuChoice
        {
            ShowFullComparison,
            FilterById,
            FilterByResult
        }
    }
}