package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.File;
import java.util.Collection;

public class ExternalData {

    static {

    }

    /**
     * Указывает путь до кэша.
     */
    private static File pathToCache =
            new File("cache/").getAbsoluteFile();


    /**
     * Функция отвечает за то, чтобы получить таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     * Стоит отметить, что если в кэше есть не устаревшие таблицы, то функция
     * вернёт таблицы из кэша.
     * @return Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     */
    public static Collection<? extends ExcelFileInterface> openTablesFromExternal() {
        if(isCacheFine()) {
            updateCache();
        }
        return getCache();
    }
}
