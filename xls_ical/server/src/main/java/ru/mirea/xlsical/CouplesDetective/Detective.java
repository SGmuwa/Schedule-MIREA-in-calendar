package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.IOException;
import java.util.List;

/**
 * Данный класс отвечает за просмотр пар из Excel расписания.
 * Данный класс необходим, чтобы был общий класс для реализации
 * просмотра расписания как и для семестра, так и для экзаменов.
*/
public abstract class Detective {
    //public abstract static List<Couple> startAnInvestigations(Seeker seeker, Iterable<ExcelFileInterface> files) throws DetectiveException, IOException; Разработка остановлена, так как требуется ручное изменение многих ссылок (более 10).
}
