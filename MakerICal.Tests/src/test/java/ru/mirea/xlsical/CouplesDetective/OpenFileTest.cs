/*
    Schedule MIREA in calendar.
    Copyright (C) 2020
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)
    George Andreevich Falileev

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ru.mirea.xlsical.CouplesDetective.xl;
using Xunit;

namespace ru.mirea.xlsical.CouplesDetective
{

    /// <summary>
    /// Тестирование базовых инструментов работы с Excel файлами.
    /// </summary>
    public class OpenFileTest
    {
        private Assembly assembly = Assembly.GetExecutingAssembly();

        [Fact]
        public void TestOpenBadFile()
        {
            using (Stream test = assembly.GetManifestResourceStream("MakerICal.Tests.tests.badExcel.xlsx"))
                for (int i = 0; i < 1000; i++)
                    Assert.Throws<System.IO.FileFormatException>(() => OpenFile.NewInstances(test));
        }

        [Fact]
        public void TestOpenNormalFile()
        {
            using (Stream file = assembly.GetManifestResourceStream("MakerICal.Tests.tests.IIT-3k-18_19-osen.xlsx"))
            {
                for (int i = 0; i < 200; i++)
                {
                    IList<ExcelFileInterface> files = OpenFile.NewInstances(file);
                    foreach (ExcelFileInterface aFile in files)
                        aFile.Dispose();
                }
            }
        }

        [Fact]
        public void TestHeapSpace()
        {
            IEnumerable<Stream> heap = new EnumerableResourceStreams(new string[] { "MakerICal.Tests.tests.heap1.xlsx", "MakerICal.Tests.tests.heap2.xlsx" });
            for (int i = 0; i < 5; i++)
            {
                foreach (Stream aHeap in heap)
                {
                    IList<ExcelFileInterface> files =
                            OpenFile.NewInstances(aHeap);
                    foreach (ExcelFileInterface file in files)
                        file.Dispose();
                }
            }
            foreach (Stream s in heap)
                s.Dispose();
        }

        [Fact]
        public void TestOpenXLS()
        {
            using (Stream stream = assembly.GetManifestResourceStream("MakerICal.Tests.tests.small.xlsx"))
            {
                IList<ExcelFileInterface> list = OpenFile.NewInstances(stream);
                Assert.Equal(1, list.Count);
                using (ExcelFileInterface file = list[0])
                {
                    Assert.Equal("Груша и лягушка", file.GetCellData(1, 1));
                    Assert.Equal("12", file.GetCellData(2, 1));
                    Assert.Equal("Градусник", file.GetCellData(1, 2));
                    Assert.Equal("13.4", file.GetCellData(2, 2));
                    Assert.Equal("Груша и лягушка; 12; Градусник; 13,4", file.GetCellData(3, 3));
                }
            }
        }

        [Fact]
        public void TestOpenXLSColors()
        {
            using (Stream stream = assembly.GetManifestResourceStream("MakerICal.Tests.tests.small.xlsx"))
            {
                IList<ExcelFileInterface> list = OpenFile.NewInstances(stream);
                Assert.Equal(1, list.Count);
                using (ExcelFileInterface file = list[0])
                {
                    Assert.True(file.IsBackgroundColorsEquals(1, 1, 2, 1));
                    Assert.True(file.IsBackgroundColorsEquals(2, 1, 1, 1));
                    Assert.True(file.IsBackgroundColorsEquals(1, 1, 1, 1));
                    Assert.True(file.IsBackgroundColorsEquals(2, 1, 2, 1));

                    Assert.True(file.IsBackgroundColorsEquals(1, 2, 2, 2));
                    Assert.True(file.IsBackgroundColorsEquals(2, 2, 1, 2));
                    Assert.True(file.IsBackgroundColorsEquals(1, 2, 1, 2));
                    Assert.True(file.IsBackgroundColorsEquals(2, 2, 2, 2));

                    Assert.True(file.IsBackgroundColorsEquals(1, 3, 2, 3));
                    Assert.True(file.IsBackgroundColorsEquals(2, 3, 1, 3));
                    Assert.True(file.IsBackgroundColorsEquals(1, 3, 1, 3));
                    Assert.True(file.IsBackgroundColorsEquals(2, 3, 2, 3));

                    Assert.False(file.IsBackgroundColorsEquals(1, 1, 1, 2));
                    Assert.False(file.IsBackgroundColorsEquals(1, 2, 1, 1));
                    Assert.False(file.IsBackgroundColorsEquals(1, 1, 1, 3));
                    Assert.False(file.IsBackgroundColorsEquals(1, 3, 1, 1));

                    Assert.False(file.IsBackgroundColorsEquals(1, 2, 1, 3));
                    Assert.False(file.IsBackgroundColorsEquals(1, 3, 1, 2));
                }
            }
        }

        [Fact]
        public void SpreadsheetDocumentStreamTest()
        {
            using (Stream stream = assembly.GetManifestResourceStream("MakerICal.Tests.tests.small.xlsx"))
            {
                ICollection<ExcelFileInterface> files = OpenFile.NewInstances(stream);
                Assert.Equal(1, files.Count);
                using (ExcelFileInterface file = files.First())
                {
                    Assert.Equal("Груша и лягушка", file.GetCellData(1, 1));
                }
                stream.Seek(0, SeekOrigin.Begin);
                Assert.Equal(80, stream.ReadByte());
            }
        }

        [Theory]
        [InlineData(1, "A1")]
        [InlineData(2, "B1")]
        [InlineData(3, "C1")]
        [InlineData(4, "D1")]
        [InlineData(5, "E1")]
        [InlineData(6, "F1")]
        [InlineData(7, "G1")]
        [InlineData(8, "H1")]
        [InlineData(9, "I1")]
        [InlineData(10, "J1")]
        [InlineData(11, "K1")]
        [InlineData(12, "L1")]
        [InlineData(13, "M1")]
        [InlineData(14, "N1")]
        [InlineData(15, "O1")]
        [InlineData(16, "P1")]
        [InlineData(17, "Q1")]
        [InlineData(18, "R1")]
        [InlineData(19, "S1")]
        [InlineData(20, "T1")]
        [InlineData(21, "U1")]
        [InlineData(22, "V1")]
        [InlineData(23, "W1")]
        [InlineData(24, "X1")]
        [InlineData(25, "Y1")]
        [InlineData(26, "Z1")]
        [InlineData(27, "AA1")]
        [InlineData(28, "AB1")]
        [InlineData(29, "AC1")]
        public void AddressToCoordinateTest(int num, string address)
            => Assert.Equal(num, OpenFile.StaticTools.AddressToCoordinate(address).column);
    }
}
