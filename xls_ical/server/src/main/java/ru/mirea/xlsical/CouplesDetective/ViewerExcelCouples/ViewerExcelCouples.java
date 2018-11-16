package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

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
 * В последствии будет переименован в {@code ViewerExcelCouples}.
*/
public abstract class ViewerExcelCouples implements Closeable {

    /**
     * Файл, в котором требуется искать пары занятий.
     */
    protected ExcelFileInterface file;

    /**
     * Создаёт экземпляр просмоторщика excel таблицы.
     * @param file Файл, в котором требуется искать пары занятий.
     */
    protected ViewerExcelCouples(ExcelFileInterface file) {
        this.file = file;
    }

    /**
     * Функция ищет занятия для seeker в файлах files.
     * @param seeker критерий поиска.
     * @throws ViewerExcelCouplesException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     * @deprecated Лучше делать через экземпляр.
     */
    public static List<CoupleInCalendar> startAnInvestigations(Iterable<ViewerExcelCouples> detectives) throws ViewerExcelCouplesException, IOException {
        List<CoupleInCalendar> output = new LinkedList<>();
        for (ViewerExcelCouples d : detectives)
                output.addAll(d.startAnInvestigation());
        return output;
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     * @param seeker критерий поиска.
     * @throws ViewerExcelCouplesException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    public abstract List<CoupleInCalendar> startAnInvestigation() throws ViewerExcelCouplesException, IOException;

    /**
     * Функция решает, какой именно требуется способ просмотра Excel таблицы.
     * TODO: Данная функция ещё не разработана.
     * @param file Входящий файл, к которому необходимо применить правило.
     * @return Детектив, который разбирается с данным файлом.
     */
    public static ViewerExcelCouples chooseDetective(ExcelFileInterface file) {
        return new ViewerExcelCouplesSemester(file);
    }

    @Override
    public void close() throws IOException {
        file.close();
    }
}
