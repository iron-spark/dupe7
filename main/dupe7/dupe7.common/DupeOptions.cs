using dupe7.common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common
{
    public class DupeOptions
    {
        public bool Recursive { get; set; }
        public bool KeepNewest { get; set; }
        public bool RemoveEmptyFolders { get; set; }
        public FileProcessingMode ProcessMode { get; set; }
        public List<string> Folders { get; set; }
    }
}
