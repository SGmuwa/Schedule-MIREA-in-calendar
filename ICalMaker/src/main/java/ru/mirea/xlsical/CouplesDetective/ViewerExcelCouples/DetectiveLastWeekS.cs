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

import java.io.IOException;
import java.time.*;
import java.time.temporal.ChronoUnit;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания.
 * Данный класс может видеть только расписание зачётной недели.
 */
public class DetectiveLastWeekS implements IDetective {

    private DetectiveSemester detectiveSemester;

    public DetectiveLastWeekS(ExcelFileInterface file, DetectiveDate dateSettings) {
        detectiveSemester = new DetectiveSemester(file, dateSettings);
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     *
     * @param start  Дата и время начала составления расписания.
     * @param finish Дата и время конца составления раписания.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException        Во время работы с Excel file - файл стал недоступен.
     */
    @Override
    public List<CoupleInCalendar> startAnInvestigation(ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException {
        return detectiveSemester.startAnInvestigation(start, finish);
    }

    /**
     * Функция расчитывает рекомендуемое время начала построения текущего расписания.
     * @param now Момент времени, который считается настоящим.
     * @return Возвращает первую секунду, с которой можно принимать зачёт. Это гаратированно 00:00:00.
     * @see #getFinishTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getStartTime(ZonedDateTime now) {
        return static_getStartTime(detectiveSemester.dateSettings, now);
    }

    /**
     * Данный метот является реализацией для {@link #getStartTime(ZonedDateTime)}.
     * Используется для того, чтобы Детектив симестра узнал, до какой даты ему строить расписание.
     *
     * Функция расчитывает рекомендуемое время начала построения текущего расписания.
     * @param dateSettings Параметры дат, которые внесены ректором.
     * @param now Момент времени, который считается настоящим.
     * @return Возвращает первую секунду, с которой можно принимать зачёт. Это гарантированно 00:00:00.
     * @see #getFinishTime(ZonedDateTime)
     */
    protected static ZonedDateTime static_getStartTime(DetectiveDate dateSettings, ZonedDateTime now) {
        ZonedDateTime current = static_getFinishTime(dateSettings, now).minus(7, ChronoUnit.DAYS);
        current = Detective.addBusinessDaysToDate(current, 1);
        return ZonedDateTime.of(
                current.toLocalDate(),
                LocalTime.MIN,
                current.getZone()
        );
    }

    /**
     * Функция расчитывает рекомендуемое время конца построения текущего расписания.
     *
     * @param now Момент времени, который считается настоящим.
     * @return Последняя доступная секунда для принятия зачёта.
     * @see #getStartTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getFinishTime(ZonedDateTime now) {
        return static_getFinishTime(detectiveSemester.dateSettings, now);
    }

    protected static ZonedDateTime static_getFinishTime(DetectiveDate dateSettings, ZonedDateTime now) {
        DetectiveDate.TwoZonedDateTime search;
        if (Month.JANUARY.getValue() <= now.getMonth().getValue()
                && now.getMonth().getValue() <= Month.JUNE.getValue()
        ) { // У нас загружано расписание для весны. Ищем конец.
            search = dateSettings.searchBeforeAfter(
                    ZonedDateTime.of(
                            LocalDate.of(now.getYear(), Month.MAY, 15),
                            LocalTime.NOON,
                            now.getZone()
                    ),
                    Duration.of(35, ChronoUnit.DAYS)
            );
        }
        else { // У нас загружано расписание для осени. Ищем конец.
            search = dateSettings.searchBeforeAfter(
                    ZonedDateTime.of(
                            LocalDate.of(now.getYear(), Month.DECEMBER, 10),
                            LocalTime.NOON,
                            now.getZone()
                    ),
                    Duration.of(35, ChronoUnit.DAYS)
            );
        }

        return search.getRight() == null ? guessFinishTime(now) : search.getRight();
    }


    /**
     * Угадывает, в какой день будет закончена зачётная неделя
     * @param now Любая дата семестра, к которому прилегает зачёт.
     * @return День и время, в который зачёт проводить можно.
     */
    protected static ZonedDateTime guessFinishTime(ZonedDateTime now) {
        ZonedDateTime current;
        if (Month.JANUARY.getValue() <= now.getMonth().getValue()
                && now.getMonth().getValue() <= Month.JUNE.getValue()
        ) { // У нас загружано расписание для весны. Ищем конец.
            current = ZonedDateTime.of(
                    // Мы узнаём последний день мая защищённым способом =). Хотя я в курсе, что это 31 мая.
                    LocalDate.of(now.getYear(), Month.JUNE, 1),
                    LocalTime.MIN,
                    now.getZone()
            );
        }
        else { // У нас загружано расписание для осени. Ищем конец.
            current = ZonedDateTime.of(
                    // Производственный шестидненвый календарь предполагает, что 30 декабря - последний рабочий день.
                    // Выставим 31 декабря и отнимим секунду.
                    LocalDate.of(now.getYear(), Month.DECEMBER, 31),
                    LocalTime.MIN,
                    now.getZone()
            );
        }
        // Переходим на последнию секунду доступного дня:
        current = current.minus(1, ChronoUnit.SECONDS);
        // Если мы попали на воскресенье, то сдвинемся на день назад:
        if(current.getDayOfWeek().equals(DayOfWeek.SUNDAY)) {
            current = current.minus(1, ChronoUnit.DAYS);
        }
        return current;
    }

    @Override
    public void close() throws IOException {
        detectiveSemester.close();
    }
}
