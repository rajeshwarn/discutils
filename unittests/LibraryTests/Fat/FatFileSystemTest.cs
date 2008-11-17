﻿//
// Copyright (c) 2008, Kenneth Bell
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System.IO;
using NUnit.Framework;

namespace DiscUtils.Fat
{
    [TestFixture]
    public class FatFileSystemTest
    {
        [Test]
        public void FormatFloppy()
        {
            MemoryStream ms = new MemoryStream();
            FatFileSystem fs = FatFileSystem.FormatFloppy(ms, FloppyDiskType.HighDensity, "KBFLOPPY   ");
        }

        [Test]
        public void FormatPartition()
        {
            MemoryStream ms = new MemoryStream();

            FatFileSystem fs = FatFileSystem.FormatPartition(ms, "KBPARTITION", DiskGeometry.FromCapacity(1024 * 1024 * 32), 0, 13);

            fs.CreateDirectory(@"DIRB\DIRC");

            FatFileSystem fs2 = new FatFileSystem(ms);
            Assert.AreEqual(1, fs2.Root.GetDirectories().Length);
        }

        [Test]
        public void CreateDirectory()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");

            fs.CreateDirectory(@"UnItTeSt");
            Assert.AreEqual("UNITTEST", fs.Root.GetDirectories("UNITTEST")[0].Name);

            fs.CreateDirectory(@"folder\subflder");
            Assert.AreEqual("FOLDER", fs.Root.GetDirectories("FOLDER")[0].Name);

            fs.CreateDirectory(@"folder\subflder");
            Assert.AreEqual("SUBFLDER", fs.Root.GetDirectories("FOLDER")[0].GetDirectories("SUBFLDER")[0].Name);

        }

        [Test]
        public void CanWrite()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");
            Assert.AreEqual(true, fs.CanWrite);
        }

        [Test]
        public void Label()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");
            Assert.AreEqual("FLOPPY_IMG ", fs.VolumeLabel);

            fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, null);
            Assert.AreEqual("NO NAME    ", fs.VolumeLabel);
        }

        [Test]
        public void FileInfo()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");
            DiscFileInfo fi = fs.GetFileInfo(@"SOMEDIR\SOMEFILE.TXT");
            Assert.IsNotNull(fi);
        }

        [Test]
        public void DirectoryInfo()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");
            DiscDirectoryInfo fi = fs.GetDirectoryInfo(@"SOMEDIR");
            Assert.IsNotNull(fi);
        }

        [Test]
        public void FileSystemInfo()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");
            DiscFileSystemInfo fi = fs.GetFileSystemInfo(@"SOMEDIR\SOMEFILE");
            Assert.IsNotNull(fi);
        }

        [Test]
        public void Root()
        {
            FatFileSystem fs = FatFileSystem.FormatFloppy(new MemoryStream(), FloppyDiskType.HighDensity, "FLOPPY_IMG ");
            Assert.IsNotNull(fs.Root);
            Assert.IsTrue(fs.Root.Exists);
            Assert.IsEmpty(fs.Root.Name);
            Assert.IsNull(fs.Root.Parent);
        }

    }
}