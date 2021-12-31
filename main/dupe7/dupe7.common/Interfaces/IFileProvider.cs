using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Interfaces
{
    public interface IFileProvider
    {
        Stream OpenRead(string path);
        string GetFileName(string path);
        DateTime GetLastWriteTimeUtc(string path);
        Task<byte[]> ReadAllBytesAsync(string path);
        string[] GetDirectories(string path);
        IEnumerable<string> GetFiles(string path);
        bool FolderExists(string path);
        string GetFolder(string item);
        void DeleteFile(string item);
        void DeleteFolderIfEmpty(string folder);
    }
}
