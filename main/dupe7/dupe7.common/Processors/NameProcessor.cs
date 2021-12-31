using dupe7.common.Interfaces;
using dupe7.common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Processors
{
    /// <summary>
    /// Groups the list of files by filename (with extension)
    /// </summary>
    public class NameProcessor : AbstractProcessor
    {
        public NameProcessor(bool keepNewest, IFileProvider fileProvider, ILogger log) : base(keepNewest, fileProvider, log)
        {
        }

        protected override Task<List<List<FileResult>>> GroupFiles(List<string> files)
        {
            Dictionary<string, List<FileResult>> groupedFiles = new Dictionary<string, List<FileResult>>();

            foreach (var file in files)
            {
                var name = FileProvider.GetFileName(file);
                if (!groupedFiles.ContainsKey(name))
                {
                    groupedFiles.Add(name, new List<FileResult>());
                }

                groupedFiles[name].Add(new FileResult { Path = file, LastWrittenAt = FileProvider.GetLastWriteTimeUtc(file) });
            }

            return Task.FromResult(groupedFiles.Select(x => x.Value).ToList());
        }
    }
}
