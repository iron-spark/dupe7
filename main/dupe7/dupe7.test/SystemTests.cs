using dupe7.common;
using dupe7.common.Enums;
using dupe7.common.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dupe7.test
{
    /// <summary>
    /// Testing behaviours of the system as a whole
    /// </summary>
    public class SystemTests 
    {
        private Dupe7Search _search;
        private TestStorage _prov;
        private string _fileAText;
        private string _fileBText;
        private DateTime _now;

        [SetUp]
        public void Setup()
        {
            _prov = new TestStorage();
            var logger = new Mock<ILogger>();
            _search = new Dupe7Search(logger.Object, _prov);

            _fileAText = _prov.GetResourceFile("TextA.txt");
            _fileBText = _prov.GetResourceFile("TextB.txt");

            _now = System.DateTime.UtcNow;
        }

        [Test]
        [TestCase(FileProcessingMode.Name)]
        [TestCase(FileProcessingMode.Hash)]
        [TestCase(FileProcessingMode.Full)]
        public async Task Search_Should_Not_Find_Duplicates(FileProcessingMode mode)
        {
            _prov.AddFolder("root\\");
            _prov.AddFile("root\\foo_bar.txt", "foo bar", _now);
            _prov.AddFile("root\\foo_bar2.txt", "foo bar", _now);
            // ^^ these should not get detected in hash & full because of the path in options

            _prov.AddFolder("root\\folder-A\\");
            _prov.AddFile("root\\folder-A\\file-a.txt", _fileAText, _now);
            _prov.AddFile("root\\folder-A\\file-b.txt", _fileBText, _now);
            // No Duplicates to be found - so shouldn't
            
            var options = new DupeOptions()
            {
                Folders = new List<string>()
                {
                    "root\\folder-A\\"
                },
                ProcessMode = mode
            };

            var result = await _search.DupeFolders(options);

            Assert.AreEqual(0, result.FilesToDelete.Count);
        }

        [Test]
        public async Task Named_Process_Should_Find_3_Files()
        {
            _prov.AddFolder("root\\");
            _prov.AddFile("root\\foo_bar.txt", "foo bar", _now.AddDays(-1));

            _prov.AddFolder("root\\folder-A\\");
            _prov.AddFile("root\\folder-A\\foo_bar.txt", _fileAText, _now);
            _prov.AddFile("root\\folder-A\\bar-foo.txt", _fileAText, _now);

            _prov.AddFolder("root\\folder-B\\");
            _prov.AddFile("root\\folder-B\\foo_bar.txt", _fileBText, _now);

            _prov.AddFolder("root\\folder-C\\");
            _prov.AddFile("root\\folder-C\\foo_bar.txt", "fizz buzz", _now);

            var options = new DupeOptions()
            {
                Folders = new List<string>()
                {
                    "root\\"
                },
                ProcessMode = FileProcessingMode.Name,
                Recursive = true,
                KeepNewest = true
            };

            var result = await _search.DupeFolders(options);

            Assert.AreEqual(3, result.FilesToDelete.Count);
        }

        [Test]
        public async Task Hash_Process_Should_Find_3_Files()
        {
            _prov.AddFolder("root\\");
            _prov.AddFile("root\\foo_bar.txt", _fileAText, _now.AddDays(-1));

            _prov.AddFolder("root\\folder-A\\");
            _prov.AddFile("root\\folder-A\\foo_bar.txt", _fileAText, _now);
            _prov.AddFile("root\\folder-A\\bar-foo.txt", _fileAText, _now);

            _prov.AddFolder("root\\folder-B\\");
            _prov.AddFile("root\\folder-B\\foo_bar.txt", _fileBText, _now);

            _prov.AddFolder("root\\folder-C\\");
            _prov.AddFile("root\\folder-C\\foo_bar.txt", _fileBText, _now);

            var options = new DupeOptions()
            {
                Folders = new List<string>()
                {
                    "root\\"
                },
                ProcessMode = FileProcessingMode.Hash,
                Recursive = true,
                KeepNewest = true
            };

            var result = await _search.DupeFolders(options);

            Assert.AreEqual(3, result.FilesToDelete.Count);
        }

        [Test]
        public async Task Full_Process_Should_Find_3_Files()
        {
            _prov.AddFolder("root\\");
            _prov.AddFile("root\\foo_bar.txt", _fileAText, _now.AddDays(-1));

            _prov.AddFolder("root\\folder-A\\");
            _prov.AddFile("root\\folder-A\\foo_bar.txt", _fileAText, _now);
            _prov.AddFile("root\\folder-A\\bar-foo.txt", _fileAText, _now);

            _prov.AddFolder("root\\folder-B\\");
            _prov.AddFile("root\\folder-B\\foo_bar.txt", _fileBText, _now);

            _prov.AddFolder("root\\folder-C\\");
            _prov.AddFile("root\\folder-C\\foo_bar.txt", _fileBText, _now);

            var options = new DupeOptions()
            {
                Folders = new List<string>()
                {
                    "root\\"
                },
                ProcessMode = FileProcessingMode.Full,
                Recursive = true,
                KeepNewest = true
            };

            var result = await _search.DupeFolders(options);

            Assert.AreEqual(3, result.FilesToDelete.Count);
        }
    }
}