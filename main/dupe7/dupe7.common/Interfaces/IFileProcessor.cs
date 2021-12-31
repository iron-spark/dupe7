using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Interfaces
{
    public interface IFileProcessor
    {
        bool KeepNewest { get; set; }
        IFileProvider FileProvider { get; set; }
        Task<DedupeResult> Process(List<string> files);
    }
}