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

using System;
using System.Threading;
using ru.mirea.xlsical.CouplesDetective.xl;
using Xunit;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Тестирование правильности получения Excel файлов из сайта университета.
    /// </summary>
    public class ExternalDataUpdaterTest
    {
        [Fact]
        public void run()
        {
            Console.WriteLine("ExternalDataUpdaterTest.java#run: start");
            ExternalDataUpdater edu = GlobalTaskExecutor.externalDataUpdater;
            Console.WriteLine("ExternalDataUpdaterTest.java#run: finish externalDataUpdater");
            Assert.Null(edu.pathToCache); // Теперь нет пути кэша в данном режиме тестов.
                                          //Assert.True(edu.pathToCache.canWrite());
            edu.Run();
            // Assert.True(edu.isAlive()); Он теперь ничего не скачивает во время тестов.
            Thread.Sleep(100);
            // Assert.True(edu.isAlive()); Он теперь ничего не скачивает во время тестов.
            // in future: assertEquals("Матчин Василий Тимофеевич, старший преподаватель кафедры инструментального и прикладного программного обеспечения.", edu.findTeacher("Матчин В.Т."));
            // 63 файла в бакалавре
            // 22 файла в магистратуре
            // 25 файла в аспирантуре
            // 1 файл в колледже
            // -10 pdf
            // .xls 101 файл.
            EnumeratorExcels files = edu.OpenTablesFromExternal();
            // Осторожно! Число со временем тестирования может меняться!
            /* <a ref="view-source:https://www.mirea.ru/education/schedule-main/schedule/">
             * сюда</a> и проверить количество соответствий с ".xls"*/
            //assertEquals(files.size(), 101);
            //for (ExcelFileInterface file : files) {
            //    file.close();
            //}
            Assert.True(files.MoveNext());
            Assert.True(files.MoveNext());
            edu.Stop();
            int count = 0;
            Console.WriteLine("ExternalDataUpdaterTest.java#run: start open files");
            files.Reset();
            while (files.MoveNext())
            {
                ExcelFileInterface efi = files.Current;
                Console.WriteLine(efi);
                efi.Dispose();
                count++;
            }
            Assert.True(count > 100);
            //assertEquals(101, count);
            Thread.Sleep(20);
        }
    }
}
