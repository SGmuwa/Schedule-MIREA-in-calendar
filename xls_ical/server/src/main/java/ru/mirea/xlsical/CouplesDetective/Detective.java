package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.IOException;
import java.util.LinkedList;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания.
 * Данный класс необходим, чтобы был общий класс для реализации
 * просмотра расписания как и для семестра, так и для экзаменов.
*/
public abstract class Detective {

    /**
     * Функция ищет занятия для seeker в файлах files.
     * @param seeker критерий поиска.
     * @param files список файлов, в которых требуется искать пары занятий.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    public List<CoupleInCalendar> startAnInvestigations(Seeker seeker, Iterable<ExcelFileInterface> files) throws DetectiveException, IOException {
        List<CoupleInCalendar> output = new LinkedList<>();
        int index = 0;
        for (ExcelFileInterface f : files)
            try {
                output.addAll(startAnInvestigation(seeker, f));
                index++;
            } catch (DetectiveException error) {
                throw new DetectiveException("Ошибка в файле с индексом " + index + ":" + error.getMessage(), error.excelFile);
            }
        return output;
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     * @param seeker критерий поиска.
     * @param file файл, в котором требуется искать пары занятий.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    public abstract List<CoupleInCalendar> startAnInvestigation(Seeker seeker, ExcelFileInterface file) throws DetectiveException, IOException;
}
