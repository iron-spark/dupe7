using dupe7.common;
using dupe7.common.Interfaces;
using dupe7.common.Providers;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Core;
using Serilog.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dupe7.app
{
    class Program
    {
        static async Task Main(string[] args)
        {
            IFileProvider fileProv = new FileProvider();

            IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            IConfigurationSection section = config.GetSection("dupe-options");
            var options = section.Get<DupeOptions>();
            
            Logger Log = new LoggerConfiguration().ReadFrom.Configuration(config).CreateLogger();
            
            Log.Information("START OF PROGRAM");
            Log.Information($"ProcessMode = {options.ProcessMode}");
            Log.Information($"KeepNewest = {options.KeepNewest}");
            Log.Information($"Recursive = {options.Recursive}");
            Log.Information($"RemoveEmptyFolders = {options.RemoveEmptyFolders}");
            Log.Information($"Folders to scan:");
            options.Folders.ForEach(Log.Information);

            Dupe7Search search = new Dupe7Search(new SerilogLoggerFactory(Log).CreateLogger("dupe7"), fileProv);

            var result = await search.DupeFolders(options);
            List<string> foldersToCheck = new List<string>();

            Log.Information($"Found {result.FilesToDelete.Count} duplicates");
            
            foreach (var item in result.FilesToDelete)
            {
                string folder = fileProv.GetFolder(item);
                if (foldersToCheck.Contains(folder))
                {
                    foldersToCheck.Add(folder);
                }
                Log.Information("FILE: " + item);
            }

            Log.Information("Are you sure you want to delete these files? (y/n)");
            string resp = Console.ReadLine();
            Log.Information("Response: " + resp);

            if (resp.ToLower() == "y")
            {
                Log.Warning("Are you sure? (y/n)");
                resp = Console.ReadLine();
                Log.Information("Response: " + resp);

                if (resp.ToLower() == "y")
                {
                    foreach (var item in result.FilesToDelete)
                    {
                        fileProv.DeleteFile(item);
                    }

                    if (options.RemoveEmptyFolders)
                    {
                        foreach (var folder in foldersToCheck)
                        {
                            fileProv.DeleteFolderIfEmpty(folder);
                        }
                    }
                }
            }

            Log.Information("END OF PROGRAM");
            Console.ReadLine();
        }
    }
}
