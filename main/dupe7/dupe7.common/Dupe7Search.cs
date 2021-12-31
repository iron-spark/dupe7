using dupe7.common.Enums;
using dupe7.common.Interfaces;
using dupe7.common.Processors;
using dupe7.common.Providers;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common
{
    public class Dupe7Search
    {
        public Dupe7Search(ILogger logger, IFileProvider fileProvider)
        {
            Logger = logger;
            FileProvider = fileProvider;
        }

        public ILogger Logger { get; set; }
        public IFileProvider FileProvider { get; set; }

        public async Task<DedupeResult> DupeFolders(DupeOptions options)
        {
            IFileProcessor processor = options.ProcessMode switch
            {
                FileProcessingMode.Name => new NameProcessor(options.KeepNewest, FileProvider, Logger),
                FileProcessingMode.Hash => new HashProcessor(options.KeepNewest, FileProvider, Logger),
                FileProcessingMode.Full => new FullProcessor(options.KeepNewest, FileProvider, Logger),
                _ => throw new NotImplementedException()
            };

            var files = await FindFiles(options.Folders, options.Recursive);

            Logger.LogInformation($"Found {files.Count} files.");

            return await processor.Process(files);
        }

        private async Task<List<string>> FindFiles(IEnumerable<string> folders, bool recursive)
        {
            List<string> files = new List<string>();

            var invalidFolders = folders.Where(x => !FileProvider.FolderExists(x)).ToList();

            if (invalidFolders.Count > 0)
            {
                Logger.LogWarning($"Some of the folders provided could not be found:");
                invalidFolders.ForEach((s) => Logger.LogWarning($"(skipping): '{s}'"));
            }

            foreach (var folder in folders.Where(FileProvider.FolderExists))
            {
                files.AddRange(FileProvider.GetFiles(folder));

                if (recursive)
                {
                    var innerFolders = FileProvider.GetDirectories(folder);

                    if (innerFolders.Length > 0)
                    {
                        files.AddRange(await FindFiles(innerFolders, recursive).ConfigureAwait(false));
                    }
                }
            }

            return files;
        }
    }
}
