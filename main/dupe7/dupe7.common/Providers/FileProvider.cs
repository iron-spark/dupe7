using dupe7.common.Interfaces;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.common.Providers
{
    public class FileProvider : IFileProvider
    {
        public void DeleteFile(string item)
        {
            if (System.IO.File.Exists(item))
            {
                FileSystem.DeleteFile(item, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
            }
        }

        public void DeleteFolderIfEmpty(string folder)
        {
            if (System.IO.Directory.Exists(folder))
            {
                if (!GetFiles(folder).Any())
                {
                    FileSystem.DeleteDirectory(folder, UIOption.AllDialogs, RecycleOption.SendToRecycleBin, UICancelOption.DoNothing);
                }
            }
        }

        public bool FolderExists(string path)
        {
            return System.IO.Directory.Exists(path);
        }

        public string[] GetDirectories(string path)
        {
            return System.IO.Directory.GetDirectories(path);
        }

        public string GetFileName(string path)
        {
            return System.IO.Path.GetFileName(path);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            return System.IO.Directory.GetFiles(path);
        }

        public string GetFolder(string item)
        {
            return System.IO.Path.GetDirectoryName(item);
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            var createdAt = System.IO.File.GetCreationTimeUtc(path);
            var modifiedAt = System.IO.File.GetLastWriteTimeUtc(path);
            return createdAt > modifiedAt ? createdAt : modifiedAt;
        }

        public Stream OpenRead(string path)
        {
            return System.IO.File.OpenRead(path);
        }

        public Task<byte[]> ReadAllBytesAsync(string path)
        {
            return System.IO.File.ReadAllBytesAsync(path);
        }
    }
}
