package ru.mirea.xlsical.CouplesDetective.Detective;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.Closeable;
import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания. <p/>
 * Данный класс необходим, чтобы был общий класс для реализации
 * просмотра расписания как и для семестра, так и для экзаменов.
*/
public abstract class Detective implements Closeable {

    /**
     * Файл, в котором требуется искать пары занятий.
     */
    protected ExcelFileInterface file;

    /**
     * Создаёт экземпляр просмоторщика excel таблицы.
     * @param file Файл, в котором требуется искать пары занятий.
     */
    protected Detective(ExcelFileInterface file) {
        this.file = file;
    }

    /**
     * Функция ищет занятия для seeker в файлах files.
     * @param seeker критерий поиска.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    public static List<CoupleInCalendar> startAnInvestigations(Seeker seeker, Iterable<Detective> detectives) throws DetectiveException, IOException {
        List<CoupleInCalendar> output = new LinkedList<>();
        for (Detective d : detectives)
                output.addAll(d.startAnInvestigation(seeker));
        return output;
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     * @param seeker критерий поиска.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    public abstract List<CoupleInCalendar> startAnInvestigation(Seeker seeker) throws DetectiveException, IOException;

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
