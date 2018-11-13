package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.IOException;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания.
 * Данный класс может видеть только экзаменационное расписание.
 */
public class ViewerExcelCouplesExams extends ViewerExcelCouples {
    protected ViewerExcelCouplesExams(ExcelFileInterface file) {
        super(file);
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     * // TODO: Данная функция ещё не разработана.
     *
     * @param seeker критерий поиска.
     * @throws ViewerExcelCouplesException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException        Во время работы с Excel file - файл стал недоступен.
     */
    @Override
    public List<CoupleInCalendar> startAnInvestigation(Seeker seeker) throws ViewerExcelCouplesException, IOException {
        return null;
    }
}
