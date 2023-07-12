using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            var table = new ParameterComparisonTable(comparison);
            //table.ApplyResultFilter(ComparisonResult.Removed);
            table.ApplyParameterNameFilter("10");
            var metadataTable = MetadataTable(comparison);
            AnsiConsole.Write(metadataTable);
            AnsiConsole.Write(table.BuildTable());
        }

        public static Grid MetadataTable(Comparison comparison)
        {
            var sourceTable = new Table().Title("Source");
            var targetTable = new Table().Title("Target");
            sourceTable.AddColumn("Parameter");
            sourceTable.AddColumn("Value");
            targetTable.AddColumn("Parameter");
            targetTable.AddColumn("Value");

            foreach (var parameter in comparison.Source.Parameters.Where(x => x.IsMetadata))
            {
                sourceTable.AddRow(parameter.Id.ToString() ?? string.Empty, parameter.Value.ToString() ?? string.Empty);
            }

            foreach (var parameter in comparison.Target.Parameters.Where(x => x.IsMetadata))
            {
                targetTable.AddRow(parameter.Id.ToString() ?? string.Empty, parameter.Value.ToString() ?? string.Empty);
            }
            
            // create panel with two tables
            var grid = new Grid();
            grid.AddColumn(new GridColumn().NoWrap());
            grid.AddColumn(new GridColumn().NoWrap());
            grid.AddRow(sourceTable, targetTable);
            return grid;
        }
    }
}