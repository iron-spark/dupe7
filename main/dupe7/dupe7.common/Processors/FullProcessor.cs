using dupe7.common.Components;
using dupe7.common.Interfaces;
using dupe7.common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Processors
{
    public class FullProcessor : AbstractProcessor
    {
        public FullProcessor(bool keepNewest, IFileProvider fileProvider, ILogger log) : base(keepNewest, fileProvider, log)
        {
        }

        protected override async Task<List<List<FileResult>>> GroupFiles(List<string> files)
        {
            List<List<FileResult>> results = new List<List<FileResult>>();
            using (var tree = new FamilyTree(FileProvider, Log))
            {
                tree.SetFiles(files);
                await tree.Run().ConfigureAwait(false);

                // only duplicates should be located
                foreach (var item in tree.Roots)
                {
                    results.Add(item.Trackers.Select(x => x.Result).ToList());
                }
            }

            return results;
        }
    }
}
