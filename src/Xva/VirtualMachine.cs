﻿//
// Copyright (c) 2008-2009, Kenneth Bell
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

using System.Collections.Generic;
using System.IO;
using System.Xml.XPath;

namespace DiscUtils.Xva
{
    /// <summary>
    /// Class representing the virtual machine stored in a Xen Virtual Appliance (XVA)
    /// file.
    /// </summary>
    /// <remarks>XVA is a VM archive, not just a disk archive.  It can contain multiple
    /// disk images.  This class provides access to all of the disk images within the
    /// XVA file.</remarks>
    public class VirtualMachine
    {
        private static readonly XPathExpression FindVDIsExpression = XPathExpression.Compile("/value/struct/member[child::name='objects']/value/array/data/value/struct[child::member/value='VDI']");
        private static readonly XPathExpression GetDiskId = XPathExpression.Compile("member[child::name='id']/value");
        private static readonly XPathExpression GetDiskUuid = XPathExpression.Compile("member[child::name='snapshot']/value/struct/member[child::name='uuid']/value");
        private static readonly XPathExpression GetDiskNameLabel = XPathExpression.Compile("member[child::name='snapshot']/value/struct/member[child::name='name_label']/value");
        private static readonly XPathExpression GetDiskCapacity = XPathExpression.Compile("member[child::name='snapshot']/value/struct/member[child::name='virtual_size']/value");

        private Stream _fileStream;
        private TarFile _archive;

        /// <summary>
        /// Creates a new instance from a stream.
        /// </summary>
        /// <param name="fileStream">The stream containing the .XVA file</param>
        public VirtualMachine(Stream fileStream)
        {
            _fileStream = fileStream;
            _fileStream.Position = 0;
            _archive = new TarFile(fileStream);
        }

        internal TarFile Archive
        {
            get { return _archive; }
        }

        /// <summary>
        /// Gets the disks in this XVA.
        /// </summary>
        /// <returns>An enumeration of disks</returns>
        public IEnumerable<Disk> Disks
        {
            get
            {
                using (Stream docStream = _archive.OpenFile("ova.xml"))
                {
                    XPathDocument ovaDoc = new XPathDocument(docStream);
                    XPathNavigator nav = ovaDoc.CreateNavigator();
                    foreach (XPathNavigator node in nav.Select(FindVDIsExpression))
                    {
                        XPathNavigator idNode = node.SelectSingleNode(GetDiskId);

                        // Skip disks which are only referenced, not present
                        if (_archive.DirExists(idNode.ToString()))
                        {
                            XPathNavigator uuidNode = node.SelectSingleNode(GetDiskUuid);
                            XPathNavigator nameLabelNode = node.SelectSingleNode(GetDiskNameLabel);
                            long capacity = long.Parse(node.SelectSingleNode(GetDiskCapacity).ToString());
                            yield return new Disk(this, uuidNode.ToString(), nameLabelNode.ToString(), idNode.ToString(), capacity);
                        }
                    }
                }
            }
        }
    }
}