using dupe7.common.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dupe7.test
{
    public class TestStorageItem
    {
        public Stream DataStream { get; set; }
        public DateTime LastWriteTime { get; set; }
    }

    public class TestStorage : IFileProvider
    {
        private Dictionary<string, TestStorageItem> _storage;

        public TestStorage()
        {
            _storage = new Dictionary<string, TestStorageItem>();
        }

        public string GetResourceFile(string name)
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream("dupe7.test." + name))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
        public void AddFolder(string path)
        {
            if (!_storage.ContainsKey(path))
            {
                _storage.Add(path, new TestStorageItem());
            }
        }
        public void AddFile(string path, string content, DateTime lastWriteTime)
        {
            if (!_storage.ContainsKey(path))
            {
                byte[] data = Encoding.UTF8.GetBytes(content);

                var stream = new MemoryStream(data);

                _storage.Add(path, new TestStorageItem() { DataStream = stream, LastWriteTime = lastWriteTime });
            }
        }

        public List<string> GetAllFiles()
        {
            return _storage?.Keys.ToList();
        }

        public void DeleteFile(string item)
        {
            if (_storage.ContainsKey(item))
            {
                _storage.Remove(item);
            }
        }

        public void DeleteFolderIfEmpty(string folder)
        {
            var keys = _storage.Keys.Where(StartsWith(folder)).ToList();

            if (keys.Count == 1)
            {
                _storage.Remove(folder);
            }
        }

        public bool FolderExists(string path)
        {
            return _storage.Keys.Any(StartsWith(path));
        }

        private static Func<string, bool> StartsWith(string path)
        {
            return x => x.ToLower().StartsWith(path.ToLower());
        }

        public string[] GetDirectories(string path)
        {
            return _storage.Keys
                .Where(StartsWith(path))
                .Where(x => x.EndsWith('\\') && x != path)
                .ToArray();
        }

        public string GetFileName(string path)
        {
            return Path.GetFileName(path);
        }

        public IEnumerable<string> GetFiles(string path)
        {
            int slashes = path.Sum(SlashCount());

            return _storage.Keys
                .Where(StartsWith(path))
                .Where(x => x != path)
                .Where(x => x.Sum(SlashCount()) == slashes)
                .Where(x => !x.EndsWith("\\"));
        }

        private static Func<char, int> SlashCount()
        {
            return x => x == '\\' ? 1 : 0;
        }

        public string GetFolder(string item)
        {
            var pos = item.LastIndexOf('\\');

            if (pos >= 0)
                item = item.Substring(0, pos);

            return _storage.Keys.Contains(item) ? item : null;
        }

        public DateTime GetLastWriteTimeUtc(string path)
        {
            if (_storage.ContainsKey(path))
            {
                return _storage[path].LastWriteTime;
            }

            return default;
        }

        public Stream OpenRead(string path)
        {
            if (_storage.ContainsKey(path))
            {
                return _storage[path].DataStream;
            }

            return default;
        }

        public async Task<byte[]> ReadAllBytesAsync(string path)
        {
            if (_storage.ContainsKey(path))
            {
                byte[] result;
                var source = _storage[path].DataStream;
                result = new byte[source.Length];

                await source.ReadAsync(result, 0, (int)source.Length);
                return result;
            }

            return default;
        }
    }
}