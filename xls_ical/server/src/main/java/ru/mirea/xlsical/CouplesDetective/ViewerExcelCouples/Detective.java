package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.Closeable;
import java.io.IOException;
import java.time.ZonedDateTime;
import java.util.LinkedList;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания. <p/>
 * Данный класс необходим, чтобы был общий класс для реализации
 * просмотра расписания как и для семестра, так и для экзаменов.
 * В последствии будет переименован в {@code Detective}.
*/
public abstract class Detective implements Closeable {

    /**
     * Файл, в котором требуется искать пары занятий.
     */
    protected final ExcelFileInterface file;

    /**
     * Создаёт экземпляр просмоторщика excel таблицы.
     * @param file Файл, в котором требуется искать пары занятий.
     */
    protected Detective(ExcelFileInterface file) {
        this.file = file;
    }

    /**
     * Функция ищет занятия для seeker в файлах files.
     * @param detectives Здесь содержится список детективов,
     *                   которые получают excel пары и преобразуют в календарные пары.
     * @param start Время начала составления.
     * @param finish Время конца составления.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     * @see Detective#chooseDetective(ExcelFileInterface)
     */
    public static List<CoupleInCalendar> startAnInvestigations(Iterable<? extends Detective> detectives, ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException {
        List<CoupleInCalendar> output = new LinkedList<>();
        for (Detective d : detectives)
                output.addAll(d.startAnInvestigation(start, finish));
        return output;
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     * @param start Дата и время начала составления расписания.
     * @param finish Дата и время конца составления раписания.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    public abstract List<CoupleInCalendar> startAnInvestigation(ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException;

    /**
     * Функция решает, какой именно требуется способ просмотра Excel таблицы.
     * TODO: Данная функция ещё не разработана.
     * @param file Входящий файл, к которому необходимо применить правило.
     * @return Детектив, который разбирается с данным файлом.
     */
    public static Detective chooseDetective(ExcelFileInterface file) {
        return new DetectiveSemester(file);
    }

    @Override
    public void close() throws IOException {
        file.close();
    }
}
