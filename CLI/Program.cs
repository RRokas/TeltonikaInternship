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
            
            var tableBuilder = new TableBuilder(comparison);
            
            var comparisonTable = tableBuilder.BuildComparisonTable();
            var metadataTables = tableBuilder.BuildMetadataTables();
           
            var rootGrid = new Grid().AddColumn();
            var metadataGrid = new Grid().AddColumns(2).AddRow(metadataTables.source, metadataTables.target);
            rootGrid.AddRow(metadataGrid);
            rootGrid.AddRow(comparisonTable.Expand());

            AnsiConsole.Write(rootGrid);
        }
    }
}