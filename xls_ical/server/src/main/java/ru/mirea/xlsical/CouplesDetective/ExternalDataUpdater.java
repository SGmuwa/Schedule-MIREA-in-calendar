package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;

import java.io.*;
import java.net.URL;
import java.nio.file.Files;
import java.time.LocalDateTime;
import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.stream.Stream;

/**
 * Класс, который отвечает за синхронизацию с <a href="https://www.mirea.ru/education/">mirea.ru</a>.
 * Данный класс потокобезопасный.
 * @since 17.11.2018
 * @version 18.11.2018
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
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
        if(!pathToCache.isDirectory())
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
    private final Object monitorCacheIsReady = new Object();

    /**
     * Отвечает на вопрос, нужно ли ждать вообще. Идёт ли какой-то процесс?
     */
    private boolean isNeedWait = false;

    /**
     * Генератор случайных значений
     */
    private static final Random random = new Random();

    /**
     * Расположение доступных файлов Excel
     */
    private ArrayList<File> excelFiles;

    /**
     * Список преподавателей.
     */
    private ArrayList<Teacher> teachers;

    /**
     * Функция отвечает за то, чтобы получить таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     * Стоит отметить, что если в кэше есть не устаревшие таблицы, то функция
     * вернёт таблицы из кэша.
     * @return Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     */
    public ArrayList<ExcelFileInterface> openTablesFromExternal() {
        waitCache();
        ArrayList<ExcelFileInterface> files = new ArrayList<>(excelFiles.size());
        for (File path :
                excelFiles) {
            try {
                files.addAll(OpenFile.newInstances(path.getPath()));
            } catch (Exception e) {
                e.printStackTrace();
                System.out.println(e.getLocalizedMessage() + "\nfile: " + path);
            }
        }
        return files;
    }

    /**
     * Функция ведёт поиск полного имени преподавателя.
     * @param nameInExcel Краткое имя преподавателя.
     * @return Полное имя и должность преподавателя.
     */
    public String findTeacher(String nameInExcel) {
        // TODO: Необходимо реализовать функционал.
        return nameInExcel;
    }

    private void waitCache() {
        if (isNeedWait)
            try {
                monitorCacheIsReady.wait();
            } catch (InterruptedException e) {
                // ignore.
            }
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

    /**
     * Функция скачивает необходимый контент в папку кэша.
     */
    private synchronized void download() {
        isNeedWait = true;
        Stream<String> htmlExcels = downloadHTML("https://www.mirea.ru/education/schedule-main/schedule/");

        Collection<String> excelUrls = FindAllExcelURLs(htmlExcels);
        try {
            excelFiles = downloadHTMLsToPath(excelUrls);

            Stream<String> htmlNames = downloadHTML("https://www.mirea.ru/sveden/employees/");
            teachers = getTeachers(htmlNames);
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(e.getLocalizedMessage());
        }
        isNeedWait = false;
        monitorCacheIsReady.notifyAll();
    }

    /**
     * Функция получает преподавателей из HTML потока.
     * @param htmlNames HTML код, где преподаватели.
     * @return Список преподавателей.
     */
    private ArrayList<Teacher> getTeachers(Stream<String> htmlNames) throws Exception {
        if(htmlNames == null)
            return null;
        StringBuilder html = new StringBuilder();
        Pattern pFio = Pattern.compile("<td itemprop=\"fio\">");
        Pattern pPost = Pattern.compile("<td itemprop=\"post\">");
        Pattern pEnd = Pattern.compile("</td>");
        boolean needEnd = false;
        Iterator<String> iteratorHtmlNames = htmlNames.iterator();
        while (iteratorHtmlNames.hasNext()) {
            String str = iteratorHtmlNames.next();
            Matcher mFio = pFio.matcher(str);
            Matcher mPost = pPost.matcher(str);
            Matcher mEnd = pEnd.matcher(str);
            boolean isAdd = false; // Необходимо, чтобы дважды не добавить одну и ту же строку.
            if (mFio.find() || mPost.find() || needEnd) {
                html.append(' ');
                html.append(str);
                isAdd = true;
                needEnd = true;
            }
            if (mEnd.find()) {
                if (!isAdd) html.append(str);
                needEnd = false;
            }
        }
        Matcher mFio = pFio.matcher(html);
        List<String> namesOfTeachers = new ArrayList<>();
        while(mFio.find()) {
            int indexStart = mFio.end() + 1;
            int indexFinish = html.indexOf("</td", indexStart) - 1;
            namesOfTeachers.add(html.substring(indexStart, indexFinish));
        }
        Matcher mPost = pPost.matcher(html);
        List<String> posts = new ArrayList<>();
        while(mPost.find()) {
            int indexStart = mPost.end() + 1;
            int indexFinish = html.indexOf("</td", indexStart) - 1;
            posts.add(html.substring(indexStart, indexFinish));
        }
        if(namesOfTeachers.size() != posts.size())
            throw new Exception("size of Teachers " + namesOfTeachers.size() + " not equals size Posts " + posts.size() + " .");

        ArrayList<Teacher> teachers = new ArrayList<>(posts.size());
        Iterator<String> iPost = posts.iterator();
        Iterator<String> iName = namesOfTeachers.iterator();
        while (iPost.hasNext() && iName.hasNext()) {
            teachers.add(new Teacher(iName.next(), iPost.next()));
        }
        return teachers;
    }

    /**
     * Загружает все файлы по URL и помещает в дерикторию кэша.
     * @param excelUrls Коллекция ссылок на скачивание.
     */
    private ArrayList<File> downloadHTMLsToPath(Collection<String> excelUrls) throws IOException {
        ArrayList<File> excelFilesPaths = new ArrayList<>();
        for(String url : excelUrls) {
            Stream<String> fStream = downloadHTML(url);
            if(fStream != null) {
                File newFile = new File(this.pathToCache, LocalDateTime.now().toString() + "_" + url.substring(url.lastIndexOf("/")));
                Files.write(newFile.toPath(), (Iterable<String>) fStream::iterator);
                excelFilesPaths.add(newFile);
            }
            else
                throw new IOException("ExcelUrls can't download.");
        }
        return excelFilesPaths;
    }

    /**
     * Находит все ссылки на файлы excel из потока текста.
     * @param htmlExcels Поток текста html.
     * @return Лист на ссылки excel файлов.
     */
    private List<String> FindAllExcelURLs(Stream<String> htmlExcels) {
        // href ?= ?"http.+?\.[xX][lL][sS][xX]?"
        ArrayList<String> excelsUrls = new ArrayList<>();
        Pattern p = Pattern.compile("href ?= ?\"http.+?\\.[xX][lL][sS][xX]?\"");
        htmlExcels.forEach((str) -> {
            Matcher m = p.matcher(str);
            if(m.find())
                excelsUrls.add(m.group());

        });
        return excelsUrls;
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
