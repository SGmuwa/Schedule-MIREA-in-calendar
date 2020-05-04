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
    /// Данный класс может видеть только расписание зачётной недели.
    /// </summary>
    public class DetectiveLastWeekS : IDetective
    {

        private DetectiveSemester detectiveSemester;

        public DetectiveLastWeekS(ExcelFileInterface file, DetectiveDate dateSettings)
            => detectiveSemester = new DetectiveSemester(file, dateSettings);

        /// <summary>
        /// Функция ищет занятия для seeker в файле File.
        /// </summary>
        /// <param name="start">Дата и время начала составления расписания.</param>
        /// <param name="finish">Дата и время конца составления расписания.</param>
        /// <returns>Список занятий.</returns>
        /// <exception cref="DetectiveException">Появилась проблема, связанная с обработкой Excel файла.</exception>
        /// <exception cref="System.IO.IOException">Во время работы с Excel file - файл стал недоступен.</exception>
        public IEnumerable<CoupleInCalendar> StartAnInvestigation(ZonedDateTime start, ZonedDateTime finish)
            => detectiveSemester.StartAnInvestigation(start, finish);

        /// <summary>
        /// Функция расчитывает рекомендуемое время начала построения текущего расписания.
        /// </summary>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Возвращает первую секунду, с которой можно принимать зачёт. Это гарантированно 00:00:00.</returns>
        /// <seealso cref="GetFinishTime(ZonedDateTime)"/>
        public ZonedDateTime GetStartTime(ZonedDateTime now)
            => GetStartTime(detectiveSemester.dateSettings, now);

        /// <summary>
        /// Данный метод является реализацией для <see cref="GetStartTime(ZonedDateTime)"/>.
        /// Используется для того, чтобы Детектив семестра узнал, до какой даты ему строить расписание.
        /// <para>Функция расчитывает рекомендуемое время начала построения текущего расписания.</para>
        /// </summary>
        /// <param name="dateSettings">Параметры дат, которые внесены ректором.</param>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Возвращает первую секунду, с которой можно принимать зачёт. Это гарантированно 00:00:00.</returns>
        /// <seealso cref="GetFinishTime(ZonedDateTime)"/>
        public static ZonedDateTime GetStartTime(DetectiveDate dateSettings, ZonedDateTime now)
        {
            ZonedDateTime current = GetFinishTime(dateSettings, now).PlusDays(-7);
            current = Detective.AddBusinessDaysToDate(current, 1);
            return new ZonedDateTime(
                    current.LocalDateTime.Date.At(LocalTime.MinValue),
                    current.Zone,
                    current.Offset
            );
        }

        /// <summary>
        /// Функция расчитывает рекомендуемое время конца построения текущего расписания.
        /// </summary>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Последняя доступная секунда для принятия зачёта.</returns>
        /// <seealso cref="GetStartTime(ZonedDateTime)"/>
        public ZonedDateTime GetFinishTime(ZonedDateTime now)
            => GetFinishTime(detectiveSemester.dateSettings, now);

        public static ZonedDateTime GetFinishTime(DetectiveDate dateSettings, ZonedDateTime now)
        {
            (ZonedDateTime? start, ZonedDateTime? finish) search;
            if (IsoMonth.January <= (IsoMonth)now.Month
                    && (IsoMonth)now.Month <= IsoMonth.June
            )
            { // У нас загружено расписание для весны. Ищем конец.
                search = dateSettings.searchBeforeAfter(
                    now.Zone.AtLeniently(new LocalDateTime(now.Year, (int)IsoMonth.May, 15, 12, 00)),
                    Duration.FromDays(35)
                );
            }
            else
            { // У нас загружено расписание для осени. Ищем конец.
                search = dateSettings.searchBeforeAfter(
                    now.Zone.AtLeniently(new LocalDateTime(now.Year, (int)IsoMonth.December, 10, 12, 00)),
                    Duration.FromDays(35)
                );
            }
            return search.finish.HasValue ? search.finish.Value : guessFinishTime(now);
        }

        /// <summary>
        /// Угадывает, в какой день будет закончена зачётная неделя
        /// </summary>
        /// <param name="now">Любая дата семестра, к которому прилегает зачёт.</param>
        /// <returns>День и время, в который зачёт проводить можно.</returns>
        protected static ZonedDateTime guessFinishTime(ZonedDateTime now)
        {
            ZonedDateTime current;
            if (IsoMonth.January <= (IsoMonth)now.Month
                    && (IsoMonth)now.Month <= IsoMonth.June
            )
            { // У нас загружено расписание для весны. Ищем конец.
                // Мы узнаём последний день мая защищённым способом =). Хотя я в курсе, что это 31 мая.
                current = now.Zone.AtStrictly(new LocalDateTime(now.Year, (int)IsoMonth.June, 1, 00, 00));
            }
            else
            { // У нас загружено расписание для осени. Ищем конец.
                // Производственный шестидневный календарь предполагает, что 30 декабря - последний рабочий день.
                // Выставим 31 декабря и отними секунду.
                current = now.Zone.AtStrictly(new LocalDateTime(now.Year, (int)IsoMonth.December, 31, 00, 00));
            }
            // Переходим на последнюю секунду доступного дня:
            current = current.PlusSeconds(-1);
            // Если мы попали на воскресенье, то сдвинемся на день назад:
            if (current.DayOfWeek == IsoDayOfWeek.Sunday)
                current = current.PlusDays(-1);
            return current;
        }

        public void Dispose()
        {
            detectiveSemester.Dispose();
        }
    }
}
