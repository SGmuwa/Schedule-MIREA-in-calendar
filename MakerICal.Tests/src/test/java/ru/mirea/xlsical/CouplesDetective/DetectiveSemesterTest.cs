/*
    Schedule MIREA in calendar.
    Copyright (C) 2020
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)
    Marina Romanovna Kuzmina

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
using System.Drawing;
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;
using ru.mirea.xlsical.CouplesDetective.xl;
using ru.mirea.xlsical.interpreter;
using Xunit;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Тестирование правильности распознавания информации из Excel файлов.
    /// </summary>
    public class DetectiveSemesterTest
    {
        [Fact]
        public void GetMinutesFromTimeStringTest()
        {
            DetectiveSemester ds = new DetectiveSemester(null, new DetectiveDate());
            Assert.Equal(728, ds.GetMinutesFromTimeString("12:08"));
            Assert.Equal(13, ds.GetMinutesFromTimeString("00-13"));
            Assert.Equal(78, ds.GetMinutesFromTimeString("01-18"));
            Assert.Equal(860, ds.GetMinutesFromTimeString("14:20"));
            Assert.Equal(1230, ds.GetMinutesFromTimeString("20-30"));
            Assert.Equal(0, ds.GetMinutesFromTimeString("00:00"));
        }

        [Fact]
        public void IsStringNumberTest()
        {
            Assert.True(DetectiveSemester.IsStringNumber("8"));
            Assert.False(DetectiveSemester.IsStringNumber(""));
            Assert.True(DetectiveSemester.IsStringNumber("85"));
            Assert.True(DetectiveSemester.IsStringNumber("0"));
            Assert.True(DetectiveSemester.IsStringNumber("-3"));
            Assert.False(DetectiveSemester.IsStringNumber("f"));
            Assert.False(DetectiveSemester.IsStringNumber("."));
        }

        [Fact]
        public void IsEqualsInListTest()
        {
            string a = "a";
            string b = "b";
            List<string> list = new List<string>();
            list.Add(a);
            Assert.True(DetectiveSemester.IsEqualsInList(list, a));
            Assert.False(DetectiveSemester.IsEqualsInList(list, b));
        }

        [Fact]
        public void StartAnInvestigationTest()
        {
            ICollection<ExcelFileInterface> files = null;

            files = OpenFile.NewInstances("tests/IIT-3k-18_19-osen.xlsx");

            Assert.NotNull(files);
            Assert.Equal(1, files.Count);
            foreach (ExcelFileInterface file in files)
                file.Dispose();
        }


        [Fact]
        public void GetCouplesFromDayTest()
        {
            IList<Point> list = new List<Point>();
            IList<ExcelFileInterface> files;
            ICollection<DetectiveSemester.CoupleInExcel> col;
            int[] times = { 540, 630, 640, 730, 780, 870, 880, 970, 980, 1070, 1080, 1170 };

            files = OpenFile.NewInstances("tests/test-01.xlsx");
            Assert.Equal(1, files.Count);
            Seeker seeker = new Seeker("ИКБО-04-16", new LocalDate(2018, 9, 1), new LocalDate(2018, 10, 1), DateTimeZoneProviders.Tzdb["UTC+3"]);
            using (ExcelFileInterface file = files[0])
            {
                col = new DetectiveSemester(file, new DetectiveDate()).GetCouplesFromDay(
                    6,
                    3,
                    "ИКБО-04-16",
                    IsoDayOfWeek.Monday,
                    list,
                    times,
                    "пр-т Вернадского, 78"
                );
            }
#warning Отсутсвуют Asserts
            System.Console.WriteLine(string.Join('\n', col));
        }

        [Fact]
        public void OpenFirstXls()
        {

            IList<ExcelFileInterface> files = null;
            files = OpenFile.NewInstances("tests/IIT-3k-18_19-osen.xlsx");
            Assert.NotNull(files);
            Assert.Equal(1, files.Count);
            ExcelFileInterface file = files[0];

            System.Console.WriteLine(file.GetCellData(1, 1));
            System.Console.WriteLine(file.GetCellData(2, 1));
            System.Console.WriteLine(file.GetCellData(1, 2));
            System.Console.WriteLine(file.GetCellData(2, 2));

#warning Должен поддерживаться using для закрытия.
            file.Dispose();
        }
#warning Отключен тест.
        /*
            [Fact]
            public void GetTimesTest() {
                Point point = new Point(5,3);
                List<ExcelFileInterface> files;
                int [] mas = {0,0};

                files = OpenFile.NewInstances("tests/IIT-3k-18_19-osen.xlsx");
                assertNotNull(files);
                Assert.Equal(1, files.Count);
                ExcelFileInterface file = files[0];

                mas = GetTimes(point, file);

            }*/
    }
}
