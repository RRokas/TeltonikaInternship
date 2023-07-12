using System;
using System.Collections.Generic;
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
            var table = new ParameterComparisonTable(comparison);
            //table.ApplyResultFilter(ComparisonResult.Removed);
            table.ApplyParameterNameFilter("10");
            AnsiConsole.Write(table.BuildTable());
        }
    }
}