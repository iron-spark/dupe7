using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dupe7.common.Components
{

    public class FamilyNode : IDisposable
    {
        /* for tracking completion*/

        private static int _completedCount;
        public static int CompletedCount => _completedCount;

        public static void NodeCompleted(int nodeSize)
        {
            Interlocked.Add(ref _completedCount, nodeSize);
        }

        private FamilyTree _branch;
        private Dictionary<string, List<FileTracker>> _tempDict;
        public FamilyNode(FamilyTree branch)
        {
            _branch = branch;
            _tempDict = new Dictionary<string, List<FileTracker>>();
            Marker = "";
            Trackers = new List<FileTracker>();
        }
        public string Marker { get; set; }
        public List<FileTracker> Trackers { get; set; }
        public bool Complete { get; set; }

        public async Task Run()
        {
            while (Trackers.Count > 1 && await Next())
            {
                foreach (var kvp in _tempDict)
                {
                    if (kvp.Key == Marker)
                    {
                        // this is our family
                        Trackers = kvp.Value;
                    }
                    else
                    {
                        _branch.EnqueueNextGeneration(kvp.Value);
                    }
                }
            }
            Complete = true;
            NodeCompleted(Trackers.Count);
            Dispose();
        }

        public async Task<bool> Next()
        {
            Marker = "";
            _tempDict.Clear();

            await Task.WhenAll(Trackers.Where(x => x.CurrentMarker != null).Select(x => x.Next()));
          
            foreach (var tracker in Trackers)
            {
                if (tracker.CurrentMarker == "finished")
                {
                    // end of the line for this file, literally - set as finished
                    // the stay in this family as null
                    tracker.Finished = true;
                }
                // first marker for this family
                if (Marker == "")
                {
                    Marker = tracker.CurrentMarker;
                }

                if (!_tempDict.ContainsKey(tracker.CurrentMarker))
                {
                    _tempDict.Add(tracker.CurrentMarker, new List<FileTracker>());
                }

                _tempDict[tracker.CurrentMarker].Add(tracker);
            }
            return !Trackers.All(x => x.Finished);
        }
        public void Dispose()
        {
            foreach (var tracker in Trackers)
            {
                tracker.Dispose();
            }
        }
    }
}
