using dupe7.common.Interfaces;
using dupe7.common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Processors
{
    public abstract class AbstractProcessor : IFileProcessor
    {
        protected AbstractProcessor(bool keepNewest, IFileProvider fileProvider, ILogger log)
        {
            KeepNewest = keepNewest;
            FileProvider = fileProvider;
            Log = log;
        }

        public bool KeepNewest { get; set; }
        public IFileProvider FileProvider { get; set; }
        public ILogger Log { get; set; }

        public async Task<DedupeResult> Process(List<string> files)
        {
            DedupeResult result = new DedupeResult
            {
                FilesToDelete = new List<string>()
            };

            var groups = await GroupFiles(files);

            foreach (var group in groups)
            {
                if (group.Count > 1)
                {
                    FileResult itemToSave = null;

                    if (KeepNewest)
                    {
                        itemToSave = group.OrderByDescending(x => x.LastWrittenAt).First();
                    }
                    else
                    {
                        itemToSave = group.OrderBy(x => x.LastWrittenAt).First();
                    }

                    var filesToDelete = group.Where(x => x != itemToSave);

                    result.FilesToDelete.AddRange(filesToDelete.Select(x => x.Path));
                }
            }

            return result;
        }

        protected abstract Task<List<List<FileResult>>> GroupFiles(List<string> files);
    }
}