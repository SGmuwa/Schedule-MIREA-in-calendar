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
package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;

import java.io.Closeable;
import java.io.IOException;
import java.time.ZonedDateTime;
import java.util.List;

/**
 * Интерфейс обозначает, что класс умеет строить календарное
 * расписание из расписания внешнего источника.
 */
public interface IDetective extends Closeable {

    /**
     * Функция ищет занятия для seeker в файле File.
     * @param start Дата и время начала составления расписания.
     * @param finish Дата и время конца составления раписания.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    List<CoupleInCalendar> startAnInvestigation(ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException;

    /**
     * Функция расчитывает рекомендуемое время начала построения текущего расписания.
     * @param now Момент времени, который считается настоящим.
     * @return Время начала занятий.
     * @see #getFinishTime(ZonedDateTime)
     */
    ZonedDateTime getStartTime(ZonedDateTime now);

    /**
     * Функция расчитывает рекомендуемое время конца построения текущего расписания.
     * @param now Момент времени, который считается настоящим.
     * @return Время конца занятий.
     * @see #getStartTime(ZonedDateTime)
     */
    ZonedDateTime getFinishTime(ZonedDateTime now);
}
