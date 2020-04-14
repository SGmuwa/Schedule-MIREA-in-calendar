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
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.IOException;
import java.time.ZonedDateTime;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания.
 * Данный класс может видеть только экзаменационное расписание.
 */
public class DetectiveExams extends Detective {
    protected DetectiveExams(ExcelFileInterface file, DetectiveDate dateSettings) {
        super(file, dateSettings);
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     * // TODO: Данная функция ещё не разработана.
     *
     * @param start  Дата и время начала составления расписания.
     * @param finish Дата и время конца составления раписания.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException        Во время работы с Excel file - файл стал недоступен.
     * @deprecated Не разработана ещё функция.
     */
    @Override
    public List<CoupleInCalendar> startAnInvestigation(ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException {
        throw new UnsupportedOperationException("Not implemented");
    }

    /**
     * Функция расчитывает рекомендуемое время начала построения текущего расписания.
     *
     * @param now Момент времени, который считается настоящим.
     * @return Время начала занятий.
     * @see #getFinishTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getStartTime(ZonedDateTime now) {
        // TODO
        throw new UnsupportedOperationException("Not implemented");
    }

    /**
     * Функция расчитывает рекомендуемое время конца построения текущего расписания.
     *
     * @param now Момент времени, который считается настоящим.
     * @return Время конца занятий.
     * @see #getStartTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getFinishTime(ZonedDateTime now) {
        // TODO
        throw new UnsupportedOperationException("Not implemented");
    }

}
