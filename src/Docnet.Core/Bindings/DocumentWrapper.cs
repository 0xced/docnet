using System;
using System.IO;
using System.Runtime.InteropServices;
using Docnet.Core.Exceptions;

namespace Docnet.Core.Bindings
{
    internal sealed class DocumentWrapper : IDisposable
    {
        private readonly IntPtr _ptr;
        private readonly FPDF_FILEACCESS _fileAccess;

        public FpdfDocumentT Instance { get; private set; }

        public DocumentWrapper(string filePath, string password)
        {
            Instance = fpdf_view.FPDF_LoadDocument(filePath, password);

            if (Instance == null)
            {
                throw new DocnetLoadDocumentException("unable to open the document", fpdf_view.FPDF_GetLastError());
            }
        }

        public DocumentWrapper(byte[] bytes, string password)
        {
            _ptr = Marshal.AllocHGlobal(bytes.Length);

            Marshal.Copy(bytes, 0, _ptr, bytes.Length);

            Instance = fpdf_view.FPDF_LoadMemDocument(_ptr, bytes.Length, password);

            if (Instance == null)
            {
                throw new DocnetLoadDocumentException("unable to open the document", fpdf_view.FPDF_GetLastError());
            }
        }

        public DocumentWrapper(Stream stream, string password)
        {
            // See https://github.com/pvginkel/PdfiumViewer/blob/b253afcfa00bb2f94ef3e8e15efc066e6b3af0f1/PdfiumViewer/NativeMethods.Pdfium.cs#L425-L440
            var fileAccessInternal = default(FPDF_FILEACCESS.__Internal);
            fileAccessInternal.m_FileLen = (uint)stream.Length;
            fileAccessInternal.m_GetBlock = IntPtr.Zero;
            fileAccessInternal.m_Param = IntPtr.Zero;
            _fileAccess = FPDF_FILEACCESS.__CreateInstance(fileAccessInternal);
            Instance = fpdf_view.FPDF_LoadCustomDocument(_fileAccess, password);

            if (Instance == null)
            {
                throw new DocnetLoadDocumentException("unable to open the document", fpdf_view.FPDF_GetLastError());
            }
        }

        public DocumentWrapper(FpdfDocumentT instance)
        {
            Instance = instance;

            if (Instance == null)
            {
                throw new DocnetLoadDocumentException("unable to open the document");
            }
        }

        public void Dispose()
        {
            if (Instance == null)
            {
                return;
            }

            fpdf_view.FPDF_CloseDocument(Instance);

            Marshal.FreeHGlobal(_ptr);

            _fileAccess?.Dispose();

            Instance = null;
        }
    }
}
