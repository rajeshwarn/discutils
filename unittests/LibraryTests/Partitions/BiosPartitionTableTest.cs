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

namespace DiscUtils.Partitions
{
    [TestFixture]
    public class BiosPartitionTableTest
    {
        [Test]
        public void Initialize()
        {
            MemoryStream ms = new MemoryStream();
            DiskGeometry geom = DiskGeometry.FromCapacity(3 * 1024 * 1024);
            BiosPartitionTable table = BiosPartitionTable.Initialize(ms, geom);

            Assert.AreEqual(0, table.Count);
        }

        [Test]
        public void Create()
        {
            MemoryStream ms = new MemoryStream();
            DiskGeometry geom = DiskGeometry.FromCapacity(3 * 1024 * 1024);
            BiosPartitionTable table = BiosPartitionTable.Initialize(ms, geom);

            Assert.AreEqual(0, table.CreatePrimaryByCylinder(1, 4, 33, false));
            Assert.AreEqual(1, table.CreatePrimaryByCylinder(5, 9, 33, false));

            Assert.AreEqual(geom.ToLogicalBlockAddress(new ChsAddress(1, 0, 1)), table[0].FirstSector);
            Assert.AreEqual(geom.ToLogicalBlockAddress(new ChsAddress(5, 0, 1)) - 1, table[0].LastSector);
            Assert.AreEqual(geom.ToLogicalBlockAddress(new ChsAddress(5, 0, 1)), table[1].FirstSector);
            Assert.AreEqual(geom.ToLogicalBlockAddress(new ChsAddress(10, 0, 1)) - 1, table[1].LastSector);
        }
    }
}