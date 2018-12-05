package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.IOException;
import java.time.*;
import java.time.temporal.ChronoUnit;
import java.util.List;

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
     *
     * @param now Момент времени, который считается настоящим.
     * @return Время начала занятий.
     * @see #getFinishTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getStartTime(ZonedDateTime now) {
        Detective.subtractBusinessDaysToDate(getFinishTime(), 7);
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
        DetectiveDate.TwoZonedDateTime search;
        if (Month.JANUARY.getValue() <= now.getMonth().getValue()
                && now.getMonth().getValue() <= Month.JUNE.getValue()
        ) { // У нас загружано расписание для весны. Ищем конец.
            search = detectiveSemester.dateSettings.searchBeforeAfter(
                    ZonedDateTime.of(
                            LocalDate.of(now.getYear(), Month.MAY, 15),
                            LocalTime.NOON,
                            now.getZone()
                    ),
                    Duration.of(35, ChronoUnit.DAYS)
            );
        }
        else { // У нас загружано расписание для осени. Ищем конец.
            search = detectiveSemester.dateSettings.searchBeforeAfter(
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
     * @param now
     * @return
     */
    protected static ZonedDateTime guessFinishTime(ZonedDateTime now) {
        if (Month.JANUARY.getValue() <= now.getMonth().getValue()
                && now.getMonth().getValue() <= Month.JUNE.getValue()
        ) { // У нас загружано расписание для весны. Ищем конец.
            ZonedDateTime current = ZonedDateTime.of(
                    // Мы узнаём последний день мая защищённым способом =). Хотя я в курсе, что это 31 мая.
                    LocalDate.of(now.getYear(), Month.JUNE, 1),
                    LocalTime.MIN,
                    now.getZone()
            );
        }
        else { // У нас загружано расписание для осени. Ищем конец.

        }
        throw new UnsupportedOperationException();
    }

    @Override
    public void close() throws IOException {
        detectiveSemester.close();
    }
}
