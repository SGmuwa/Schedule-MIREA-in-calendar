/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.SampleConsoleTransferPercentReady;

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
        this(new File("cache/excel/"), true, new PercentReady());
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     */
    public ExternalDataUpdater(PercentReady pr) throws IOException {
        this(new File("cache/excel/"), true, pr);
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     * @param path Путь до каталога, где должны содержаться excel файлы.
     * @throws IOException Возникает тогда, когда при отсутсвии заданной папки
     * возникает неудачная попытка создать данную папку и её родительские каталоги.
     */
    public ExternalDataUpdater(String path) throws IOException {
        if(path != null) {
            pathToCache = new File(path);
            createCacheDir();
            download(new PercentReady());
        }
        else {
            pathToCache = null;
        }
    }

    /**
     * Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
     * @param path Путь до каталога, где должны содержаться excel файлы.
     * @param isNeedDownload Укажите True, если надо скачать базу данных файлов.
     * @throws IOException Возникает тогда, когда при отсутсвии заданной папки
     * возникает неудачная попытка создать данную папку и её родительские каталоги.
     */
    public ExternalDataUpdater(String path, boolean isNeedDownload) throws IOException {
        if(path != null) {
            pathToCache = new File(path);
            createCacheDir();
            if(isNeedDownload)
                download(new PercentReady());
        }
        else {
            pathToCache = null;
        }
    }

    public ExternalDataUpdater(File path, boolean isNeedDownload, PercentReady pr) throws IOException {
        pathToCache = path;
        if(path != null) {
            createCacheDir();
            if (isNeedDownload)
                download(pr); // first download
        }
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

    public ExternalDataUpdater(ArrayList<File> excelFiles, ArrayList<Teacher> teachers) {
        pathToCache = null;
        if(excelFiles != null)
            this.excelFiles = excelFiles;
        else
            this.excelFiles = new ArrayList<>();
        if(teachers != null)
            this.teachers = teachers;
        else
            this.teachers = new ArrayList<>();
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
    private ArrayList<File> excelFiles = new ArrayList<>();

    /**
     * Список преподавателей.
     */
    private ArrayList<Teacher> teachers = new ArrayList<>();

    /**
     * Объект, который надо обновлять после получения обновлений.
     */
    private ICacheUpdater needUpdate;

    /**
     * Функция отвечает за то, чтобы получить таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
     * Стоит отметить, что если в кэше есть не устаревшие таблицы, то функция
     * вернёт таблицы из кэша.
     * Файлы надо закрывать методом {@link Closeable#close()}!
     * Если вам по-середине понадобилось закрыть все файлы, вам всё равно придётся все элементы перебрать и закрыть.
     * @return Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>. Вызвать метод .close обязательно.
     */
    public IteratorExcels openTablesFromExternal() {
        return new IteratorExcels(excelFiles);
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
        if (this.pathToCache != null)
            try {
                while (!Thread.currentThread().isInterrupted()) {
                    Thread.sleep(1000 * 60 * 60 * 8); // Проверка три раза в день.
                    download(new PercentReady(new SampleConsoleTransferPercentReady("ExternalDataUpdater.java: download xls: ")));
                }
            } catch (InterruptedException e) {
                // finish.
            }
    }

    /**
     * Функция скачивает необходимый контент в папку кэша.
     */
    private synchronized void download(PercentReady pr) {
        PercentReady PR_loader = new PercentReady(pr, 0.05f);
        PercentReady PR_downloader = new PercentReady(pr, 0.25f);
        PercentReady PR_install = new PercentReady(pr, 0.70f);
        if(pathToCache == null) {
            System.out.println("Can't update: pathToCache is null.");
            return;
        }
        PR_loader.setReady(0.1f);
        Stream<String> htmlExcels = downloadHTML("https://www.mirea.ru/education/schedule-main/schedule/");
        PR_loader.setReady(0.2f);
        ArrayList<String> excelUrls = findAllExcelURLs(htmlExcels);
        PR_loader.setReady(0.3f);
        htmlExcels.close();
        PR_loader.setReady(0.4f);
        // ---- https -> http
        String elm;
        for(int i = excelUrls.size() - 1; i >= 0; i--) {
            elm = excelUrls.get(i);
            System.out.print(ZonedDateTime.now() + " ExternalDataUpdater.java: " + elm + " ;\t");
            elm = elm.replaceFirst("https:/", "http:/");
            excelUrls.set(i, elm);
        }
        PR_loader.setReady(0.5f);
        System.out.println();
        // ----
        try {
            PR_loader.setReady(0.6f);
            excelFiles = downloadFilesToPath(excelUrls, this.pathToCache, PR_downloader);
            PR_loader.setReady(0.7f);

            Stream<String> htmlNames = downloadHTML("https://www.mirea.ru/sveden/employees/");
            PR_loader.setReady(0.8f);
            teachers = getTeachers(htmlNames);
            PR_loader.setReady(0.9f);
        } catch (Exception e) {
            e.printStackTrace();
            System.out.println(e.getLocalizedMessage());
        }
        if(needUpdate != null) {
            try {
                needUpdate.update(PR_install);
            } catch (IOException e) {
                System.out.println(ZonedDateTime.now() + " ExternalDataUpdater.java: " + e.getLocalizedMessage());
            }
            PR_install.setReady(1f);
        }
        PR_loader.setReady(1f);
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
    private ArrayList<File> downloadFilesToPath(Collection<String> excelUrls, File pathToCache, PercentReady pr) throws IOException {
        PercentReady PR_writerUrls = new PercentReady(pr, 0.2f);
        PercentReady PR_downloader = new PercentReady(pr, 0.8f);


        int size = excelUrls.size();
        ArrayList<File> excelFilesPaths = new ArrayList<>(size);
        Iterator<String> it = excelUrls.iterator();
        for(int i = 0; i < size; i++) {
            String url = it.next();
            excelFilesPaths.add(new File(pathToCache, LocalDateTime.now().toString().replace(':', '-').replace('.', '_') + "_" + random.nextLong() + "_" + url.substring(url.lastIndexOf("/") + 1)));
            System.out.print(ZonedDateTime.now() + " ExternalDataUpdater.java: " + excelFilesPaths.get(i).toString() + ";\t");
            PR_writerUrls.setReady(i/(float)size);
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
            }
            PR_downloader.setReady(i/((float)size));
        }
        PR_downloader.setReady(1f);
        PR_writerUrls.setReady(1f);
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

    public void setNeedUpdate(ICacheUpdater needUpdate) {
        this.needUpdate = needUpdate;
    }
}

class IteratorExcels implements Iterator<ExcelFileInterface> {

    public IteratorExcels(ArrayList<File> excelFiles) {
        ArrayList<File> clone = ((ArrayList<File>) excelFiles.clone());
        size = clone.size();
        FilesIterator = clone.iterator();
    }

    public final int size;
    private final Iterator<File> FilesIterator;
    private Iterator<? extends ExcelFileInterface> nextElm = null;

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
}