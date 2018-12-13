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
