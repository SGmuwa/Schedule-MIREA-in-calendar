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
using NodaTime;
using NodaTime.Text;

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{
    /// <summary>
    /// Класс отвечает за то, чтобы отвечать на вопрос,
    /// когда начинается и заканчивается семестр.
    /// </summary>
    public class DetectiveDate
    {

        public DetectiveDate() : this(new FileInfo("settings_DetectiveDate.cfg")) { }

        /// <summary>
        /// Запускаете загрузчик контрольных дат.
        /// </summary>
        /// <param name="filename">Файл, с которого надо прочитать данные. Отправьте null, чтобы создать класс-пустышку.</param>
        public DetectiveDate(FileInfo filename)
        {
            if (filename == null)
                return;
            if (filename.Exists)
                loadFile(filename);
            else
            {
                try
                {
                    using (StreamWriter bw = new StreamWriter(filename.FullName))
                    {
                        bw.WriteLine("! В этом файле надо указывать ключевые точки в расписании.");
                        bw.WriteLine("! Здесь надо указывать следующие даты:");
                        bw.WriteLine("! а) Если эта дата является началом семестра,");
                        bw.WriteLine("! б) Если эта дата является концом зачётной недели.");
                        bw.WriteLine("! Формат записи дат: \"год-месяц-день\" (без кавычек)");
                        bw.WriteLine("! Например, дата 2019-02-11 подскажет программе, что");
                        bw.WriteLine("! 11 февраля 2019 года - первый день весеннего семестра.");
                        bw.WriteLine("! Некоторые даты программа может угадать самостоятельно (не внося в этот файл)");
                        bw.WriteLine("! Например, 1 сентября 2018 года была суббота, программа");
                        bw.WriteLine("! автоматически перенесёт начало семестра на 3 сентября (пн).");
                        bw.WriteLine("! Данный файл необходим, так как первая дата учёбы влияет");
                        bw.WriteLine("! на номер недели в расписании.");
                        bw.WriteLine("! Восклицательный знак в начале строки признак комментария.");
                        bw.WriteLine("! Я заполню начала весенних семестров, которые мне известны:");
                        bw.WriteLine("2015-02-09");
                        bw.WriteLine("2017-02-06");
                        bw.WriteLine("2018-02-09");
                        bw.WriteLine("2019-02-11");
                        bw.WriteLine("! Заполните файл далее ниже самостоятельно.");
                    }
                }
                catch (IOException e)
                {
                    System.Console.WriteLine("Can't create default setting file (" + filename.FullName + "). Please, fix problem: " + e.Message);
                }
            }
        }

        private void loadFile(FileInfo settings)
        {
            try
            {
                using (StreamReader br = new StreamReader(settings.FullName))
                {
                    for (string str = br.ReadLine(); str != null; str = br.ReadLine())
                        if (str.Length > 6 && str[0] != '!')
                            if(!add(str))
                                System.Console.WriteLine(settings.FullName + ": can't read text: " + str);
                }
                points.Sort();
            }
            catch (IOException e)
            {
                System.Console.WriteLine(e.Message + " Can't open file settings for DetectiveDate: " + settings.FullName + "\nPlease, visit https://github.com/SGmuwa/Schedule-MIREA-in-the-calendar and look at example settings_DetectiveDate.cfg.");
            }
        }

        /// <summary>
        /// Осуществляет поиск контрольных точек.
        /// </summary>
        /// <param name="dateToNeed">Приблизительная дата, которая вам необходима</param>
        /// <param name="allow">Границы поиска. Если точка слева или справа будут
        /// вне границ, они не будут в результате.</param>
        /// <returns>Контрольная точка ранее <paramref name="dateToNeed"/> и контрольная точка после
        /// <paramref name="dateToNeed"/>.</returns>
        public (ZonedDateTime? start, ZonedDateTime? finish) searchBeforeAfter(ZonedDateTime dateToNeed, Duration allow)
        {
            (ZonedDateTime? start, ZonedDateTime? finish) @out = (null, null);
            int left = BinarySearch.BinarySearch_Iter_Wrapper(points, dateToNeed.LocalDateTime.Date);
            if (left >= 0)
            { // Нашёлся
                @out.start = points[left].AtStartOfDayInZone(dateToNeed.Zone);
                if (left + 1 < points.Count)
                {
                    ZonedDateTime rightValue = points[left + 1].AtStartOfDayInZone(dateToNeed.Zone);
                    if ((rightValue - dateToNeed) < allow)
                        @out.finish = rightValue;
                }
            }
            else
            {
                left = ~left;
                if (left < points.Count)
                {
                    ZonedDateTime leftValue;
                    do
                    {
                        leftValue = points[left].AtStartOfDayInZone(dateToNeed.Zone);
                        if (ZonedDateTime.Comparer.Local.Compare(leftValue, dateToNeed)  > 0)
                        {
                            System.Console.WriteLine("DetectiveDate.cs: left move");
                            left--;
                            continue;
                        }
                        break;
                    } while (true);
                    if (dateToNeed - leftValue < allow)
                        @out.start = leftValue;
                }
                if (0 <= left + 1 && left + 1 < points.Count)
                {
                    ZonedDateTime rightValue = points[left + 1].AtStartOfDayInZone(dateToNeed.Zone);
                    if (rightValue - dateToNeed < allow)
                        @out.finish = rightValue;
                }
            }
            return @out;
        }

        /// <summary>
        /// Ключевые точки.
        /// Начало первого семестра - 15 августа - 15 сентября
        /// Конец первого семестра - декабрь
        /// Начала второго семестра - с 15 января по 15 марта
        /// Конец второго семестра - с 15 мая по 15 июня.
        /// </summary>
        private List<LocalDate> points = new List<LocalDate>();

        private bool add(string str)
        {
            var parser = LocalDatePattern.Iso.Parse(str);
            if(parser.Success)
            {
                points.Add(parser.Value);
                return true;
            }
            return false;
        }
    }
}
