using dupe7.common.Interfaces;
using dupe7.common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace dupe7.common.Components
{

    public class FileTracker : IDisposable
    {
        /* for debugging */
        static int _globalId;
        private int _id;

        public override string ToString()
        {
            return $"({_id})";
        }

        private byte[] _buffer;

        public FileTracker(string path, IFileProvider fileProv)
        {
            _id = _globalId++;
            Result = new FileResult()
            {
                Path = path,
                LastWrittenAt = fileProv.GetLastWriteTimeUtc(path)
            };
            DataStream = fileProv.OpenRead(path);
            _buffer = new byte[8192];
            CurrentMarker = "";
        }
        public FileResult Result { get; set; }
        public Stream DataStream { get; set; }
        public string CurrentMarker { get; set; }
        public bool Finished { get; set; }

        public async Task Next()
        {
            try
            {
                var bytesRemaining = DataStream.Length - DataStream.Position;
                if (bytesRemaining == 0)
                {
                    CurrentMarker = "finished";
                    
                    return;
                }

                if (bytesRemaining < _buffer.Length)
                {
                    _buffer = new byte[bytesRemaining];

                    await DataStream.ReadAsync(_buffer, 0, (int)bytesRemaining);
                    CurrentMarker = Encoding.UTF8.GetString(_buffer);
                    return;
                }

                await DataStream.ReadAsync(_buffer, 0, _buffer.Length);
                CurrentMarker = Encoding.UTF8.GetString(_buffer);
            }
            catch (Exception ex)
            {
                CurrentMarker = "error";
            }

        }

        ~FileTracker()
        {
            Dispose();
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            DataStream?.Dispose();
        }
    }
}
