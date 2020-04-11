package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.Closeable;
import java.io.IOException;
import java.time.DayOfWeek;
import java.time.Duration;
import java.time.ZonedDateTime;
import java.time.temporal.ChronoUnit;
import java.util.LinkedList;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания. <p/>
 * Данный класс необходим, чтобы был общий класс для реализации
 * просмотра расписания как и для семестра, так и для экзаменов.
 * В последствии будет переименован в {@code Detective}.
*/
public abstract class Detective implements IDetective {

    /**
     * Файл, в котором требуется искать пары занятий.
     */
    protected final ExcelFileInterface file;
    /**
     * Используется для доступа к конфигураций дат.
     * Благодаря этому Детектив знает, когда начало или конец
     * семестра поставил ректор университета.
     */
    protected final DetectiveDate dateSettings;

    /**
     * Создаёт экземпляр просмоторщика excel таблицы.
     * @param file Файл, в котором требуется искать пары занятий.
     * @param dateSettings Настройки дат. Подсказывает начала и концы семестров.
     */
    protected Detective(ExcelFileInterface file, DetectiveDate dateSettings) {
        this.file = file;
        this.dateSettings = dateSettings;
    }

    /**
     * Создаёт экземпляр просмоторщика excel таблицы.
     * @param file Файл, в котором требуется искать пары занятий.
     * @see Detective#Detective(ExcelFileInterface, DetectiveDate) Требуется синхронизация с датами ректора?
     * @deprecated Используйте {@link #Detective(ExcelFileInterface, DetectiveDate)}
     * для синхронизации с изменениями ректора.
     */
    protected Detective(ExcelFileInterface file) {
        this.file = file;
        this.dateSettings = new DetectiveDate(null);
    }

    /**
     * Функция ищет занятия для seeker в файлах files.
     * @param detectives Здесь содержится список детективов,
     *                   которые получают excel пары и преобразуют в календарные пары.
     * @param start Время начала составления.
     * @param finish Время конца составления.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     * @see Detective#chooseDetective(ExcelFileInterface,DetectiveDate)
     */
    public static List<CoupleInCalendar> startAnInvestigations(Iterable<? extends IDetective> detectives, ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException {
        List<CoupleInCalendar> output = new LinkedList<>();
        for (IDetective d : detectives)
                output.addAll(d.startAnInvestigation(start, finish));
        return output;
    }

    /**
     * Функция решает, какой именно требуется способ просмотра Excel таблицы.
     * TODO: Данная функция ещё не разработана.
     * @param file Входящий файл, к которому необходимо применить правило.
     * @return Детектив, который разбирается с данным файлом.
     */
    public static Detective chooseDetective(ExcelFileInterface file, DetectiveDate dateSettings) {
        return new DetectiveSemester(file, dateSettings);
    }



    protected static int getCounts6Days(ZonedDateTime current, ZonedDateTime target) {
        int sundays = 0;
        if(current.compareTo(target) > 0)
            throw new IllegalArgumentException();
        Duration duration = Duration.between(current, target);
        long days = duration.toDays() + 1;
        long weeks = days / 7;
        current = current.plus(weeks, ChronoUnit.WEEKS);
        while(current.compareTo(target) <= 0) {
            if(current.getDayOfWeek() == DayOfWeek.SUNDAY)
                sundays++;
            current = current.plus(1, ChronoUnit.DAYS);
        }
        return (int)(-sundays + (days - weeks));
    }

    /**
     * Прибавляет к дате определённое количество будних дней.
     * Используется 6-тидневная рабочая неделя. (понедельник ... суббота)
     * @param current Исходная дата, к которой надо прибавить будние дни.
     * @param bDays Количество дней, которые надо подсчитать.
     * @return Возвращает сумму даты {@code current} и {@code bDays}.
     */
    protected static ZonedDateTime addBusinessDaysToDate(ZonedDateTime current, long bDays) {
        if(current == null)
            return null;
        if(bDays == 0)
            return current;
        if(bDays < 0)
            throw new IllegalArgumentException("bDays must be more or equals 0");

        if(current.getDayOfWeek() == DayOfWeek.SUNDAY)
            current = current.plus(1, ChronoUnit.DAYS);
        long weeks = bDays / 6;
        current = current.plus(weeks, ChronoUnit.WEEKS);
        bDays -= weeks * 6;
        for(long i = 0; i < bDays; i++) {
            current = current.plus(1, ChronoUnit.DAYS);
            if(current.getDayOfWeek() == DayOfWeek.SUNDAY)
                current = current.plus(1, ChronoUnit.DAYS);
        }
        return current;
    }

    @Override
    public void close() throws IOException {
        file.close();
    }
}
