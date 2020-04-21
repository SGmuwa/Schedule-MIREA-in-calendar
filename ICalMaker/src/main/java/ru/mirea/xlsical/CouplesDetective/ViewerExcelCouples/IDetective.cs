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

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{
    /// <summary>
    /// Интерфейс обозначает, что класс умеет строить календарное
    /// расписание из расписания внешнего источника.
    /// </summary>
    public interface IDetective : System.IDisposable
    {
        /// <summary>
        /// Функция ищет занятия для seeker в файле File.
        /// </summary>
        /// <param name="start">Дата и время начала составления расписания.</param>
        /// <param name="finish">Дата и время конца составления расписания.</param>
        /// <returns>Перечисление занятий для seeker.</returns>
        /// <exception cref="DetectiveException">Появилась проблема, связанная с обработкой Excel файла.</exception>
        /// <exception cref="System.IO.IOException">Во время работы с Excel file — файл стал недоступен.</exception>
        IEnumerable<CoupleInCalendar> StartAnInvestigation(ZonedDateTime start, ZonedDateTime finish);

        /// <summary>
        /// Функция расчитывает рекомендуемое время начала построения текущего расписания.
        /// </summary>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Время начала занятий.</returns>
        /// <seealso cref="GetFinishTime(ZonedDateTime)"/>
        ZonedDateTime GetStartTime(ZonedDateTime now);

        /// <summary>
        /// Функция расчитывает рекомендуемое время конца построения текущего расписания.
        /// </summary>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Время конца занятий.</returns>
        /// <seealso cref="GetStartTime(ZonedDateTime)"/>
        ZonedDateTime GetFinishTime(ZonedDateTime now);
    }
}
