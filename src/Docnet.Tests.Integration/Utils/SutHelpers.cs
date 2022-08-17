using System;
using System.IO;
using Docnet.Core.Models;
using Docnet.Core.Readers;

namespace Docnet.Tests.Integration.Utils
{
    internal static class SutHelpers
    {
        public static IDocReader GetDocReader(this LibFixture fixture, Input type, string filePath, string password, int dimOne, int dimTwo)
        {
            return GetDocReader(fixture, type, filePath, password, new PageDimensions(dimOne, dimTwo));
        }

        public static IDocReader GetDocReader(this LibFixture fixture, Input type, string filePath, string password, double scaling)
        {
            return GetDocReader(fixture, type, filePath, password, new PageDimensions(scaling));
        }

        private static IDocReader GetDocReader(this LibFixture fixture, Input type, string filePath, string password, PageDimensions pageDimensions)
        {
            if (type == Input.FromFile)
            {
                return fixture.Lib.GetDocReader(filePath, password, pageDimensions);
            }

            if (type == Input.FromBytes)
            {
                var bytes = File.ReadAllBytes(filePath);
                return fixture.Lib.GetDocReader(bytes, password, pageDimensions);
            }

            if (type == Input.FromStream)
            {
                using var stream = new FileStream(filePath, FileMode.Open);
                return fixture.Lib.GetDocReader(stream, password, pageDimensions);
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }

        public static byte[] Split(this LibFixture fixture, Input type, string filePath, int fromIndex, int toIndex)
        {
            return Split(fixture, type, filePath, $"{fromIndex + 1} - {toIndex + 1}");
        }

        public static byte[] Split(this LibFixture fixture, Input type, string filePath, string pageRange)
        {
            if (type == Input.FromFile)
            {
                return fixture.Lib.Split(filePath, pageRange);
            }

            if (type == Input.FromBytes)
            {
                var bytes = File.ReadAllBytes(filePath);
                return fixture.Lib.Split(bytes, pageRange);
            }

            if (type == Input.FromStream)
            {
                using var stream = new FileStream(filePath, FileMode.Open);
                throw new NotImplementedException("TODO: implement Split with stream argument");
            }

            throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}