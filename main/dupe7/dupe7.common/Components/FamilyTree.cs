using dupe7.common.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Components
{

    public class FamilyTree : IDisposable
    {
        public FamilyTree(IFileProvider fileProv, ILogger logger)
        {
            Roots = new List<FamilyNode>();
            FileProvider = fileProv;
            Log = logger;
        }

        public IFileProvider FileProvider { get; set; }
        public ILogger Log { get; set; }
        public int FileCount { get; set; }
        public List<FamilyNode> Roots { get; set; }

        public void SetFiles(List<string> files)
        {
            FileCount = files.Count;
            var initialNode = new FamilyNode(this);
            foreach (var file in files)
            {
                initialNode.Trackers.Add(new FileTracker(file, FileProvider));
            }
            Roots.Add(initialNode);
        }

        public void EnqueueNextGeneration(List<FileTracker> newGroup)
        {
            if (newGroup.Count > 1)
            {
                var newNode = new FamilyNode(this);
                newNode.Trackers.AddRange(newGroup);
                lock (Roots)
                {
                    Roots.Add(newNode);
                }
                _ = newNode.Run().ConfigureAwait(false);
            }
            else
            {
                foreach (var tracker in newGroup)
                {
                    tracker.Dispose();
                }
            }
        }

        public async Task Run(bool verbose = true)
        {
            _ = Roots.First().Run().ConfigureAwait(false);
            bool completed = false;
            int lastCompletedCount = 0;
            do
            {
                await Task.Delay(3000).ConfigureAwait(true);
                FamilyNode[] roots = null;
                lock (Roots)
                {
                    roots = Roots.ToArray();
                }

                if (verbose)
                {
                    decimal perc = (decimal)FamilyNode.CompletedCount / (decimal)FileCount;
                    if (lastCompletedCount != FamilyNode.CompletedCount)
                    {
                        lastCompletedCount = FamilyNode.CompletedCount;
                    }
                    Log.LogInformation($" -- {string.Format("{0:P2}", perc)}%");
                }
                

                completed = roots.All(x => x.Complete);
            } while (!completed);
        }

        public void Dispose()
        {
            foreach (var root in Roots)
            {
                root.Dispose();
            }
        }
    }
}
