package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;
import sun.reflect.generics.reflectiveObjects.NotImplementedException;

import java.io.IOException;
import java.time.ZonedDateTime;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания.
 * Данный класс может видеть только экзаменационное расписание.
 */
public class DetectiveExams extends Detective {
    protected DetectiveExams(ExcelFileInterface file) {
        super(file);
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
        return null;
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
        throw new NotImplementedException();
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
        throw new NotImplementedException();
    }

}
