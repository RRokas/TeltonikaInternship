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
            
            var source = new DeviceConfiguration(sourceConfigFile);
            var target = new DeviceConfiguration(targetConfigFile);
            
            var comparison = new Comparison(source, target);
            
            var tableBuilder = new TableBuilder(comparison);
            
            var choice = AskComparisonType();
            if (choice == MenuChoice.FilterById)
            {
                var filterString = AnsiConsole.Ask<string>("Enter substring to search for");
                tableBuilder.ApplyParameterNameFilter(filterString);
            } else if(choice == MenuChoice.FilterByResult)
            {
                var filterString = AnsiConsole.Prompt(
                    new SelectionPrompt<ComparisonResult>()
                        .Title("Select result to filter by")
                        .AddChoices(new[] {
                            ComparisonResult.Unchanged,
                            ComparisonResult.Modified,
                            ComparisonResult.Added,
                            ComparisonResult.Removed
                        }));
                tableBuilder.ApplyResultFilter(filterString);
            }
            
            var comparisonTable = tableBuilder.BuildComparisonTable();
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
        
        private enum MenuChoice
        {
            ShowFullComparison,
            FilterById,
            FilterByResult
        }
    }
}