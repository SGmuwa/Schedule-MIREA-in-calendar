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

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.Serialization;
using NodaTime;
using ru.mirea.xlsical.interpreter;

namespace ru.mirea.xlsical.CouplesDetective
{
    public static class ExportCouplesToICal
    {
        private static readonly Random ran = new Random();

        private static readonly CalendarSerializer calendarSerializer = new CalendarSerializer();

        /// <summary>
        /// Конвектирует список пар в "*.ical" формат.
        /// </summary>
        /// <param name="couples">Перечисление пар, которые необходимо перевести в .ical.</param>
        /// <param name="percentReady">Указатель, куда отправлять процент готовности.</param>
        /// <returns>Путь до .ical файла. Файл может быть удалён, если он старше 24 часов.</returns>
        public static FileInfo start(ICollection<CoupleInCalendar> couples, PercentReady percentReady)
        {
            PercentReady load = new PercentReady(percentReady, 0.05f);
            load.Ready = 0.01f;
            PercentReady renderIcalPercentReady = new PercentReady(percentReady, 0.9f);
            PercentReady finishPercentReady = new PercentReady(percentReady, 0.05f);
            if (ran.Next() % 1000 == 0)
                ClearCashOlder24H(); // Очистка кэша.
            load.Ready = 0.1f;
            Calendar cal = new Calendar
            {
                ProductId = "-//RTU Roflex Team//xls_ical//RU"
            };
            int ready = 0;
            float size = couples.Count;
            load.Ready = 1.0f;
            foreach (CoupleInCalendar c in couples)
            {
                CalendarEvent ev = new CalendarEvent
                {
                    Summary = c.ItemTitle + " (" + c.TypeOfLesson + ")",
                    Description = c.Audience + "\n" + c.NameOfGroup + "\n" + c.NameOfTeacher,
                    Location = c.Address,
                    Uid = $"{DateTime.UtcNow.ToFileTimeUtc()}_{ran.NextLong()}@ru.mirea.xlsical",
                    Start = c.DateAndTimeOfCouple.ToCalDateTime(),
                    End = c.DateAndTimeFinishOfCouple.ToCalDateTime()
                };
                cal.Events.Add(ev);
                ready++;
                renderIcalPercentReady.Ready = ready / size;
            }
            finishPercentReady.Ready = 0.1f;

            DirectoryInfo cachePath = new DirectoryInfo("cache/icals");

            cachePath.Create();

            finishPercentReady.Ready = 0.3f;
            FileInfo nameFile = new FileInfo(Path.Combine(cachePath.FullName, DateTime.UtcNow.ToFileTimeUtc() + "_" + ran.NextLong() + ".ics"));
            finishPercentReady.Ready = 0.4f;

            try
            {
                using FileStream file = nameFile.Create();
                finishPercentReady.Ready = 0.6f;
                calendarSerializer.Serialize(cal, file, Encoding.UTF8);
                finishPercentReady.Ready = 0.8f;
            }
            catch (IOException e)
            {
                finishPercentReady.Ready = 1.0f;
                Console.WriteLine($"{e.Message} {e.StackTrace}");
                return null;
            }
            finishPercentReady.Ready = 1.0f;
            return nameFile;
        }

        /// <summary>
        /// Очищает кэш, которому более 24 часа.
        /// </summary>
        /// <returns>Количество удалённых файлов.</returns>
        private static ushort ClearCashOlder24H()
        {
            IEnumerable<FileInfo> files = new DirectoryInfo("icals\\").EnumerateFiles("*.ics");
            ushort countDel = 0;
            DateTime now = DateTime.UtcNow.AddDays(-1);
            foreach (FileInfo f in files)
                if(f.CreationTimeUtc < now)
                {
                    f.Delete();
                    countDel++;
                }
            return countDel;
        }
    }
}
