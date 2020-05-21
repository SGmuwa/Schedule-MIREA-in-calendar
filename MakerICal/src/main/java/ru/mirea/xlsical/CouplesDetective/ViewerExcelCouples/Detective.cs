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
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.xl;

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{
    /// <summary>
    /// Данный класс отвечает за просмотр пар из Excel расписания.
    /// Данный класс необходим, чтобы был общий класс для реализации
    /// просмотра расписания как и для семестра, так и для экзаменов.
    /// </summary>
    public abstract class Detective : IDetective
    {
        /// <summary>
        /// Файл, в котором требуется искать пары занятий.
        /// </summary>
        protected readonly ExcelFileInterface file;

        /// <summary>
        /// Используется для доступа к конфигураций дат.
        /// Благодаря этому Детектив знает, когда начало или конец
        /// семестра поставил ректор университета.
        /// </summary>
        public readonly DetectiveDate dateSettings;

        /// <summary>
        /// Создаёт экземпляр просмоторщика excel таблицы.
        /// </summary>
        /// <param name="file">Файл, в котором требуется искать пары занятий.</param>
        /// <param name="dateSettings">Настройки дат. Подсказывает начала и концы семестров.</param>
        protected Detective(ExcelFileInterface file, DetectiveDate dateSettings)
        {
            this.file = file;
            this.dateSettings = dateSettings;
        }

        /// <summary>
        /// Функция ищет занятия для seeker в файлах files.
        /// </summary>
        /// <param name="detectives">Здесь содержится список детективов,
        /// которые получают excel пары и преобразуют в календарные пары.</param>
        /// <param name="start">Время начала составления.</param>
        /// <param name="finish">Время конца составления.</param>
        /// <returns>Список занятий для seeker.</returns>
        /// <exception cref="DetectiveException">Появилась проблема, связанная с обработкой Excel файла.</exception>
        /// <exception cref="System.IO.IOException">Во время работы с Excel file - файл стал недоступен.</exception>
        /// <seealso cref="Detective.ChooseDetective(ExcelFileInterface, DetectiveDate)"/>
        public static IEnumerable<CoupleInCalendar> StartAnInvestigations(IEnumerable<IDetective> detectives, ZonedDateTime start, ZonedDateTime finish)
        {
            LinkedList<CoupleInCalendar> output = new LinkedList<CoupleInCalendar>();
            foreach (IDetective d in detectives)
                output.AddLastRange(d.StartAnInvestigation(start, finish));
            return output;
        }

        /// <summary>
        /// Функция решает, какой именно требуется способ просмотра Excel таблицы.
        /// </summary>
        /// <param name="file">Входящий файл, к которому необходимо применить правило.</param>
        /// <param name="dateSettings">Доступ к конфигурациям дат.</param>
        /// <returns>Детектив, который разбирается с данным файлом.</returns>
        public static Detective ChooseDetective(ExcelFileInterface file, DetectiveDate dateSettings)
        {
#warning Данная функция ещё не разработана.
            return new DetectiveSemester(file, dateSettings);
        }



        public static int GetCounts6Days(ZonedDateTime current, ZonedDateTime target)
        {
            int sundays = 0;
            if (ZonedDateTime.Comparer.Instant.Compare(current, target) > 0)
                throw new System.ArgumentException();
            Duration duration = target - current;
            int days = duration.Days + 1;
            int weeks = days / 7;
            current = current.PlusDays(weeks * 7);
            while (ZonedDateTime.Comparer.Instant.Compare(current, target) <= 0)
            {
                if (current.DayOfWeek == IsoDayOfWeek.Sunday)
                    sundays++;
                current = current.PlusDays(1);
            }
            return (int)(-sundays + (days - weeks));
        }

        /// <summary>
        /// Прибавляет к дате определённое количество будних дней.
        /// Используется 6-ти дневная рабочая неделя. (понедельник ... суббота)
        /// </summary>
        /// <param name="current">Исходная дата, к которой надо прибавить будние дни.</param>
        /// <param name="bDays">Количество дней, которые надо подсчитать.</param>
        /// <returns>Возвращает сумму даты <code>current</code> и <code>bDays</code>.</returns>
        public static ZonedDateTime AddBusinessDaysToDate(ZonedDateTime current, long bDays)
        {
            if (current == null)
                throw new System.ArgumentNullException(nameof(current));
            if (bDays == 0)
                return current;
            if (bDays < 0)
                throw new System.ArgumentException("bDays must be more or equals 0");

            if (current.DayOfWeek == IsoDayOfWeek.Sunday)
                current = current.PlusDays(1);
            checked
            {
                int weeks = (int)(bDays / 6);
                current = current.PlusDays(weeks * 7);
                bDays -= weeks * 6;
            }
            for (long i = 0; i < bDays; i++)
            {
                current = current.PlusDays(1);
                if (current.DayOfWeek == IsoDayOfWeek.Sunday)
                    current = current.PlusDays(1);
            }
            return current;
        }

        public void Dispose() => file.Dispose();
        
        public abstract ICollection<CoupleInCalendar> StartAnInvestigation(ZonedDateTime start, ZonedDateTime finish);
        public abstract ZonedDateTime GetStartTime(ZonedDateTime now);
        public abstract ZonedDateTime GetFinishTime(ZonedDateTime now);
    }
}
