package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.*;
import java.net.MalformedURLException;
import java.net.URL;
import java.util.Collection;
import java.util.stream.Stream;

/**
 * Класс, который отвечает за синхронизацию с <a href="https://www.mirea.ru/education/">mirea.ru</a>.
 */
public class ExternalDataUpdater implements Runnable {

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     */
    public ExternalDataUpdater() throws IOException {
        this("cache/excel/");
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     * @param path Путь до каталога, где должны содержаться excel файлы.
     * @throws IOException Возникает тогда, когда при отсутсвии заданной папки
     * возникает неудачная попытка создать данную папку и её родительские каталоги.
     */
    public ExternalDataUpdater(String path) throws IOException {
        this(new File(path).getAbsoluteFile());
    }

    public ExternalDataUpdater(File path) throws IOException {
        pathToCache = path;
        createCacheDir();
    }

    private void createCacheDir() throws IOException {
        if(!pathToCache.exists()) {
            if (!pathToCache.mkdirs()) {
                throw new IOException("Can't create dir cache excel.");
            }
        }
        else if(!pathToCache.isDirectory())
            throw new IOException("Path " + pathToCache.toString() + " is not directory.");
        else if(!pathToCache.canWrite())
            throw new IOException("I can't write in path " + pathToCache.toString() + " directory.");

    }

    /**
     * Указывает путь до кэша.
     */
    public final File pathToCache;

    /**
     * Данный монитор выступает в роли синхронизации потоков.
     */
    private final Object monitorCacheIsRady = new Object();

    /**
     * Функция отвечает за то, чтобы получить таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     * Стоит отметить, что если в кэше есть не устаревшие таблицы, то функция
     * вернёт таблицы из кэша.
     * @return Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     */
    public Collection<? extends ExcelFileInterface> openTablesFromExternal() {
        waitCache();
        // TODO!
        return null;//getCache();
    }

    private void waitCache() {
        // TODO!
    }

    /**
     * Запускает механизм автоматического обновления, скачивания данных
     * из сайта <a href="https://www.mirea.ru/education/">mirea.ru</a>.
     *
     * @see Runnable#run()
     */
    @Override
    public void run() {
        try {
            while (!Thread.currentThread().isInterrupted()) {
                download();
                wait(1000 * 60 * 60 * 8); // Проверка три раза в день.
            }
        } catch (InterruptedException e) {
            // finish.
        }
    }

    private void download() {
        Stream<String> htmlExcels = downloadHTML("https://www.mirea.ru/education/schedule-main/schedule/");
        Stream<String> htmlNames = downloadHTML("https://www.mirea.ru/sveden/employees/");
        // TODO: Что теперь делать с HTML?
    }

    private Stream<String> downloadHTML(String s) {
        URL url;
        InputStream is = null;
        BufferedReader br;
        String line;

        try {
            url = new URL(s);
            is = url.openStream();  // throws an IOException
            br = new BufferedReader(new InputStreamReader(is));

            return br.lines();
        } catch (MalformedURLException mue) {
            mue.printStackTrace();
        } catch (IOException ioe) {
            ioe.printStackTrace();
        } finally {
            try {
                if (is != null) is.close();
            } catch (IOException ioe) {
                // nothing to see here
            }
        }
        return null;
    }
}
