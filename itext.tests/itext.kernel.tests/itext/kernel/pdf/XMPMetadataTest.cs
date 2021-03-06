/*
This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
Authors: iText Software.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
using System;
using System.IO;
using iText.Kernel.Utils;
using iText.Kernel.XMP;
using iText.Kernel.XMP.Options;
using iText.Test;

namespace iText.Kernel.Pdf {
    public class XMPMetadataTest : ExtendedITextTest {
        public static readonly String sourceFolder = iText.Test.TestUtil.GetParentProjectDirectory(NUnit.Framework.TestContext
            .CurrentContext.TestDirectory) + "/resources/itext/kernel/pdf/XmpWriterTest/";

        public static readonly String destinationFolder = NUnit.Framework.TestContext.CurrentContext.TestDirectory
             + "/test/itext/kernel/pdf/XmpWriterTest/";

        [NUnit.Framework.OneTimeSetUp]
        public static void BeforeClass() {
            CreateOrClearDestinationFolder(destinationFolder);
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocumentWithXmp() {
            String filename = "emptyDocumentWithXmp.pdf";
            PdfWriter writer = new PdfWriter(destinationFolder + filename, new WriterProperties().AddXmpMetadata());
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 7").SetTitle("Empty iText 7 Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.Close();
            PdfReader reader = new PdfReader(destinationFolder + filename);
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            byte[] outBytes = pdfDocument.GetXmpMetadata();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(sourceFolder + "emptyDocumentWithXmp.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EmptyDocumentWithXmpAppendMode01() {
            String created = destinationFolder + "emptyDocumentWithXmpAppendMode01.pdf";
            String updated = destinationFolder + "emptyDocumentWithXmpAppendMode01_updated.pdf";
            String updatedAgain = destinationFolder + "emptyDocumentWithXmpAppendMode01_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(created));
            pdfDocument.AddNewPage();
            pdfDocument.GetXmpMetadata(true);
            // create XMP metadata
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(created), new PdfWriter(updated), new StampingProperties().UseAppendMode
                ());
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(updated), new PdfWriter(updatedAgain), new StampingProperties(
                ).UseAppendMode());
            pdfDocument.Close();
            PdfReader reader = new PdfReader(updatedAgain);
            pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata));
            PdfIndirectReference metadataRef = pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata).GetIndirectReference
                ();
            NUnit.Framework.Assert.AreEqual(6, metadataRef.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, metadataRef.GetGenNumber());
            byte[] outBytes = pdfDocument.GetXmpMetadata();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(sourceFolder + "emptyDocumentWithXmpAppendMode01.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        /// <exception cref="System.Exception"/>
        [NUnit.Framework.Test]
        public virtual void EmptyDocumentWithXmpAppendMode02() {
            String created = destinationFolder + "emptyDocumentWithXmpAppendMode02.pdf";
            String updated = destinationFolder + "emptyDocumentWithXmpAppendMode02_updated.pdf";
            String updatedAgain = destinationFolder + "emptyDocumentWithXmpAppendMode02_updatedAgain.pdf";
            PdfDocument pdfDocument = new PdfDocument(new PdfWriter(created));
            pdfDocument.AddNewPage();
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(created), new PdfWriter(updated), new StampingProperties().UseAppendMode
                ());
            pdfDocument.GetXmpMetadata(true);
            // create XMP metadata
            pdfDocument.Close();
            pdfDocument = new PdfDocument(new PdfReader(updated), new PdfWriter(updatedAgain), new StampingProperties(
                ).UseAppendMode());
            pdfDocument.Close();
            PdfReader reader = new PdfReader(updatedAgain);
            pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata));
            PdfIndirectReference metadataRef = pdfDocument.GetCatalog().GetPdfObject().GetAsStream(PdfName.Metadata).GetIndirectReference
                ();
            NUnit.Framework.Assert.AreEqual(6, metadataRef.GetObjNumber());
            NUnit.Framework.Assert.AreEqual(0, metadataRef.GetGenNumber());
            byte[] outBytes = pdfDocument.GetXmpMetadata();
            pdfDocument.Close();
            byte[] cmpBytes = ReadFile(sourceFolder + "emptyDocumentWithXmpAppendMode02.xml");
            cmpBytes = RemoveAlwaysDifferentEntries(cmpBytes);
            outBytes = RemoveAlwaysDifferentEntries(outBytes);
            NUnit.Framework.Assert.IsTrue(new CompareTool().CompareXmls(outBytes, cmpBytes));
        }

        /// <exception cref="System.IO.IOException"/>
        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        [NUnit.Framework.Test]
        public virtual void CreateEmptyDocumentWithAbcXmp() {
            MemoryStream fos = new MemoryStream();
            PdfWriter writer = new PdfWriter(fos);
            PdfDocument pdfDoc = new PdfDocument(writer);
            pdfDoc.GetDocumentInfo().SetAuthor("Alexander Chingarev").SetCreator("iText 7").SetTitle("Empty iText 7 Document"
                );
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.CreationDate);
            pdfDoc.GetDocumentInfo().GetPdfObject().Remove(PdfName.ModDate);
            PdfPage page = pdfDoc.AddNewPage();
            page.Flush();
            pdfDoc.SetXmpMetadata("abc".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1));
            pdfDoc.Close();
            PdfReader reader = new PdfReader(new MemoryStream(fos.ToArray()));
            PdfDocument pdfDocument = new PdfDocument(reader);
            NUnit.Framework.Assert.AreEqual(false, reader.HasRebuiltXref(), "Rebuilt");
            NUnit.Framework.Assert.AreEqual("abc".GetBytes(iText.IO.Util.EncodingUtil.ISO_8859_1), pdfDocument.GetXmpMetadata
                ());
            NUnit.Framework.Assert.IsNotNull(pdfDocument.GetPage(1));
            reader.Close();
        }

        /// <exception cref="iText.Kernel.XMP.XMPException"/>
        private byte[] RemoveAlwaysDifferentEntries(byte[] cmpBytes) {
            XMPMeta xmpMeta = XMPMetaFactory.ParseFromBuffer(cmpBytes);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.CreateDate, true, true);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.ModifyDate, true, true);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_XMP, PdfConst.MetadataDate, true, true);
            XMPUtils.RemoveProperties(xmpMeta, XMPConst.NS_PDF, PdfConst.Producer, true, true);
            cmpBytes = XMPMetaFactory.SerializeToBuffer(xmpMeta, new SerializeOptions(SerializeOptions.SORT));
            return cmpBytes;
        }
    }
}
