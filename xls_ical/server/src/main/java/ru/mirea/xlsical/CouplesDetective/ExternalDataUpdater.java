package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;

import java.io.*;
import java.net.URL;
import java.net.URLConnection;
import java.nio.channels.Channels;
import java.nio.channels.ReadableByteChannel;
import java.time.LocalDateTime;
import java.time.ZonedDateTime;
import java.util.*;
import java.util.regex.Matcher;
import java.util.regex.Pattern;
import java.util.stream.Stream;

import static ru.mirea.xlsical.CouplesDetective.Teacher.ConvertNameFromStrToArray;

/**
 * Класс, который отвечает за синхронизацию с <a href="https://www.mirea.ru/education/">mirea.ru</a>.
 * Данный класс потокобезопасный.
 * @since 17.11.2018
 * @version 18.11.2018
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 */
public class ExternalDataUpdater {

    public ExternalDataUpdater(boolean isNeedDownload) throws IOException {
        this("cache/excel/", isNeedDownload);
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     */
    public ExternalDataUpdater() throws IOException {
        this("cache/excel/", true);
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     * @param path Путь до каталога, где должны содержаться excel файлы.
     * @throws IOException Возникает тогда, когда при отсутсвии заданной папки
     * возникает неудачная попытка создать данную папку и её родительские каталоги.
     */
    public ExternalDataUpdater(String path) throws IOException {
        this(new File(path).getAbsoluteFile(), true);
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     * @param path Путь до каталога, где должны содержаться excel файлы.
     * @param isNeedDownload Укажите True, если надо скачать базу данных файлов.
     * @throws IOException Возникает тогда, когда при отсутсвии заданной папки
     * возникает неудачная попытка создать данную папку и её родительские каталоги.
     */
    public ExternalDataUpdater(String path, boolean isNeedDownload) throws IOException {
        this(new File(path).getAbsoluteFile(), isNeedDownload);
    }

    public ExternalDataUpdater(File path, boolean isNeedDownload) throws IOException {
        pathToCache = path;
        createCacheDir();
        if(isNeedDownload)
            download(); // first download
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
    private Thread myThread = null;

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
     * Файлы надо закрывать методом {@link Closeable#close()}!
     * Если вам по-середине понадобилось закрыть все файлы, вам всё равно придётся все элементы перебрать и закрыть.
     * @return Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>. Вызвать метод .close обязательно.
     */
    public Iterator<ExcelFileInterface> openTablesFromExternal() {
        return new Iterator<ExcelFileInterface>() {
            Iterator<File> FilesIterator = ((ArrayList<File>) excelFiles.clone()).iterator();
            Iterator<? extends ExcelFileInterface> nextElm = null;

            @Override
            public boolean hasNext() {
                if(nextElm == null || !nextElm.hasNext()) {
                    for (; FilesIterator.hasNext(); ) {
                        File path = FilesIterator.next();
                        try {
                            nextElm = new ArrayList<>(OpenFile.newInstances(path.getPath())).iterator();
                            return true;
                        } catch (IOException | InvalidFormatException e) {
                            System.out.println(e.getLocalizedMessage() + "\nfile: " + path);
                        }
                    }
                    return false;
                }
                return true;
            }

            @Override
            public ExcelFileInterface next() {
                return nextElm.next();
            }
        };
    }

    /**
     * Функция ведёт поиск полного имени преподавателя.
     * @param nameInExcel Краткое имя преподавателя.
     * @return Полное имя и должность преподавателя.
     */
    public String findTeacher(String nameInExcel) {
        // TODO: Необходимо реализовать функционал.
        return nameInExcel;
        //String[] fullName = Teacher.ConvertNameFromStrToArray(nameInExcel);
        //return findTeacher(fullName[0], fullName[1], fullName[2]);
    }

    //private String findTeacher(String surname, String firstName, String partonymic) {
        //Collections.binarySearch(teachers, surname, (left, right) -> )
    //}

    /**
     * Запускает механизм автоматического обновления, скачивания данных
     * из сайта <a href="https://www.mirea.ru/education/">mirea.ru</a>.
     */
    public void run() {
        if(myThread != null) {
            if(!myThread.isInterrupted())
                myThread.interrupt();
        }
        myThread = new Thread(this::threadBody);
        myThread.start();
    }

    public boolean isAlive() {
        return myThread != null && myThread.isAlive();
    }

    public void interrupt() {
        if(!myThread.isInterrupted())
            myThread.interrupt();
        myThread = null;
    }

    private void threadBody() {
        try {
            while (!Thread.currentThread().isInterrupted()) {
                Thread.sleep(1000 * 60 * 60 * 8); // Проверка три раза в день.
                download();
            }
        } catch (InterruptedException e) {
            // finish.
        }
    }

    /**
     * Функция скачивает необходимый контент в папку кэша.
     */
    private synchronized void download() {
        Stream<String> htmlExcels = downloadHTML("https://www.mirea.ru/education/schedule-main/schedule/");

        ArrayList<String> excelUrls = findAllExcelURLs(htmlExcels);
        htmlExcels.close();
        // ---- https -> http
        String elm;
        for(int i = excelUrls.size() - 1; i >= 0; i--) {
            elm = excelUrls.get(i);
            System.out.print(ZonedDateTime.now() + " ExternalDataUpdater.java: " + elm + " ;\t");
            elm = elm.replaceFirst("https:/", "http:/");
            excelUrls.set(i, elm);
        }
        System.out.println();
        // ----
        try {
            excelFiles = downloadFilesToPath(excelUrls, this.pathToCache);

            Stream<String> htmlNames = downloadHTML("https://www.mirea.ru/sveden/employees/");
            teachers = getTeachers(htmlNames);
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(e.getLocalizedMessage());
        }
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
    private ArrayList<File> downloadFilesToPath(Collection<String> excelUrls, File pathToCache) throws IOException {
        int size = excelUrls.size();
        ArrayList<File> excelFilesPaths = new ArrayList<>(size);
        Iterator<String> it = excelUrls.iterator();
        for(int i = 0; i < size; i++) {
            String url = it.next();
            excelFilesPaths.add(new File(pathToCache, LocalDateTime.now().toString().replace(':', '-').replace('.', '_') + "_" + random.nextLong() + "_" + url.substring(url.lastIndexOf("/") + 1)));
            System.out.print(ZonedDateTime.now() + " ExternalDataUpdater.java: " + excelFilesPaths.get(i).toString() + ";\t");
        }
        System.out.println();
        it = excelUrls.iterator();
        for(int i = 0; i < excelFilesPaths.size(); i++) {
            if(!excelFilesPaths.get(i).createNewFile())
                throw new IOException("can't create new excel file. File = " + excelFilesPaths.get(i).toString());
            try {
                downloadFile(new URL(it.next()), excelFilesPaths.get(i));
            }
            catch (IOException e) {
                System.out.println("ExternalDataUpdater.java: I can't save file " + excelFilesPaths.get(i));
                excelFilesPaths.remove(i);
                i--;
                continue;
            }
            if(i % 5 == 0)
                System.out.println((int)((float)i / (float)size * 100f));
        }
        return excelFilesPaths;
    }

    /**
     * Находит все ссылки на файлы excel из потока текста.
     * @param htmlExcels Поток текста html.
     * @return Лист на ссылки excel файлов.
     */
    private ArrayList<String> findAllExcelURLs(Stream<String> htmlExcels) {
        // href ?= ?"http.+?\.[xX][lL][sS][xX]?"
        ArrayList<String> excelsUrls = new ArrayList<>();
        Pattern p = Pattern.compile("href ?= ?\"http.+?\\.[xX][lL][sS][xX]?\"");
        htmlExcels.forEach((str) -> {
            Matcher m = p.matcher(str);
            if(m.find()) {
                String resultFind =  m.group();
                resultFind = resultFind.substring(resultFind.indexOf("http"), resultFind.lastIndexOf("\""));
                excelsUrls.add(resultFind);
            }

        });
        return excelsUrls;
    }

    /**
     * Передаёт поток на скаичвание. Поток надо закрывать!
     * @param s
     * @return
     */
    private Stream<String> downloadHTML(String s) {
        URL url;
        InputStream is = null;
        BufferedReader br;
        String line;

        try {
            url = new URL(s);
            URLConnection connection = url.openConnection();
            String redirect = connection.getHeaderField("Location");
            if (redirect != null){
                url = new URL(redirect);
            }
            is = url.openStream();  // throws an IOException
            br = new BufferedReader(new InputStreamReader(is));
            return br.lines();
        } catch (IOException ioe) {
            ioe.printStackTrace();
        }
        return null;
    }

    private void downloadFile(URL url, File fileToWrite) throws IOException {
        URLConnection connection = url.openConnection();
        String redirect = connection.getHeaderField("Location");
        if (redirect != null){ // https://stackoverflow.com/questions/18431440/301-moved-permanently
            url = new URL(redirect);
        }

        ReadableByteChannel readableByteChannel = Channels.newChannel(url.openStream());

        FileOutputStream fileOutputStream = new FileOutputStream(fileToWrite);
        fileOutputStream.getChannel().transferFrom(readableByteChannel, 0, Long.MAX_VALUE);

        readableByteChannel.close();
        fileOutputStream.close();
    }
}
