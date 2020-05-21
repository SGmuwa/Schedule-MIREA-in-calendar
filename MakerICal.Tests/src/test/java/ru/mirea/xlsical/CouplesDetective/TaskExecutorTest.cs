/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

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
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;
using ru.mirea.xlsical.CouplesDetective.xl;
using ru.mirea.xlsical.interpreter;
using ru.mirea.xlsical.Server;
using Xunit;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Тестирование конвейера задач.
    /// На вход конвейера поступают запросы, а на выходе — iCal файлы.
    /// </summary>
    public class TaskExecutorTest
    {
        [Fact]
        public void PullPollStep()
        {
            TaskExecutor te = GlobalTaskExecutor.taskExecutor;
            te.add(new PackageToMakerICal(null, null));
            te.step();
            PackageToProviderHTTP ptc = te.take();

            Assert.Null(ptc.CalFile);
            Assert.Equal(0, ptc.Count);
            Assert.Equal("Ошибка: отсутствуют критерии поиска.", ptc.Messages);
        }

        [Fact]
        public void SendSampleExcel()
        {
            TaskExecutor a = GlobalTaskExecutor.taskExecutor;
            a.add(new PackageToMakerICal(null,
                    new Seeker(
                            "ИКБО-04-16",
                            new LocalDate(2018, 9, 1),
                            new LocalDate(2018, 9, 3),
                            DateTimeZoneProviders.Tzdb["Europe/Minsk"]
                    )
            ));

            a.step();
            PackageToProviderHTTP b = a.take();
            Assert.NotNull(b.CalFile);
            System.Console.WriteLine(b.CalFile);
            Assert.Equal(3, b.Count);
        }

        [Fact]
        public void SendSampleExcelAllSem()
        {
            TaskExecutor a = new TaskExecutor(GlobalTaskExecutor.coupleHistorian);
            a.add(new PackageToMakerICal(null,
                    new Seeker(
                            "ИКБО-04-16",
                            new LocalDate(2018, 9, 1),
                            new LocalDate(2018, 12, 31),
                            DateTimeZoneProviders.Tzdb["Europe/Minsk"]
                    )
            ));

            a.step();
            PackageToProviderHTTP b = a.take();
            System.Console.WriteLine(b.CalFile);
            Assert.NotNull(b.CalFile);
            Assert.Equal(232, b.Count);
        }

        [Fact]
        public void SendExcelAllSem()
        {
            // В этом тесте надо уточнить, чтобы код думал, что сейчас 1 сентября 2018 года,
            // чтобы построил расписание на осенний семестр 2018 года.
            CoupleHistorian historian = GlobalTaskExecutor.coupleHistorian;

            TaskExecutor a = new TaskExecutor(historian);
            a.add(new PackageToMakerICal(null,
                    new Seeker(
                            "ИКБО-04-16",
                            new LocalDate(2018, 9, 1),
                            new LocalDate(2018, 12, 31),
                            DateTimeZoneProviders.Tzdb["Europe/Minsk"]
                    )
            ));

            a.step();
            PackageToProviderHTTP b = a.take();
            System.Console.WriteLine(b.CalFile);
            Assert.NotNull(b.CalFile);
            Assert.Equal(232, b.Count);
        }

        [Fact]
        public void SendExcelManual()
        {
            // В этом тесте надо уточнить, чтобы код думал, что сейчас 1 сентября 2018 года,
            // чтобы построил расписание на осенний семестр 2018 года.
            IList<ExcelFileInterface> files = OpenFile.NewInstances("tests/Zach_IIT-3k-18_19-osen.xlsx");
            IDetective det = new DetectiveLastWeekS(files[0], new DetectiveDate());

            ICollection<CoupleInCalendar> couples = det.StartAnInvestigation(
                DateTimeZoneProviders.Tzdb["Europe/Minsk"]
                .AtStartOfDay(new LocalDate(2018, (int)IsoMonth.December, 24)),
                DateTimeZoneProviders.Tzdb["Europe/Minsk"]
                .AtStartOfDay(new LocalDate(2018, (int)IsoMonth.December, 30))
            );
            var a = from c in couples where c.NameOfGroup.Equals("ИКБО-02-16") select c;
            FileInfo str = ExportCouplesToICal.start(couples, new PercentReady());
            System.Console.WriteLine($"SendExcelManual: {str.FullName}");
        }
    }
}
