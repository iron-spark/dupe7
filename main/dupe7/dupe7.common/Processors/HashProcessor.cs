using dupe7.common.Interfaces;
using dupe7.common.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Processors
{
    public class HashProcessor : AbstractProcessor
    {
        public HashProcessor(bool keepNewest, IFileProvider fileProvider, ILogger log) : base(keepNewest, fileProvider, log)
        {
        }

        protected override async Task<List<List<FileResult>>> GroupFiles(List<string> files)
        {
            Dictionary<string, List<FileResult>> groupedFiles = new Dictionary<string, List<FileResult>>();

            using (var sha = SHA256.Create())
            {
                foreach (var file in files)
                {
                    var hashData = sha.ComputeHash(await FileProvider.ReadAllBytesAsync(file).ConfigureAwait(false));
                    var hash = BitConverter.ToString(hashData);

                    if (!groupedFiles.ContainsKey(hash))
                    {
                        groupedFiles.Add(hash, new List<FileResult>());
                    }

                    groupedFiles[hash].Add(new FileResult { Path = file, LastWrittenAt = FileProvider.GetLastWriteTimeUtc(file) });
                }
            }

            return groupedFiles.Select(x => x.Value).ToList();
        }
    }
}
