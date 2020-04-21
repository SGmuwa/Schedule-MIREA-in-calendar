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

using System;
using System.IO;
using System.Collections.Generic;
using ru.mirea.xlsical.interpreter;
using System.Threading;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Класс, который отвечает за синхронизацию с <a href="https://www.mirea.ru/education/">mirea.ru</a>.
    /// Данный класс потокобезопасный.
    /// </summary>
    public class ExternalDataUpdater
    {
        public ExternalDataUpdater(bool isNeedDownload)
        : this("cache/excel/", isNeedDownload)
        {
        }

        /// <summary>
        /// Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
        /// </summary>
        public ExternalDataUpdater()
        : this(new DirectoryInfo("cache/excel/"), true, new PercentReady())
        {

        }

        /// <summary>
        /// Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
        /// </summary>
        /// <param name="pr">Доступ на отправку отчёта о прогрессе загрузки.</param>
        public ExternalDataUpdater(PercentReady pr)
        : this(new DirectoryInfo("cache/excel/"), true, pr)
        {

        }

        /// <summary>
        /// Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
        /// </summary>
        /// <param name="path">Путь до каталога, где должны содержаться excel файлы.</param>
        /// <exception cref="System.IO.IOException">Возникает тогда, когда при отсутствии заданной
        /// папки возникает неудачная попытка создать данную папку и её родительские каталоги.</exception>
        public ExternalDataUpdater(string path)
        {
            if (path != null)
            {
                pathToCache = new DirectoryInfo(path);
                createCacheDir();
                download(new PercentReady());
            }
            else
                pathToCache = null;
        }
        
        /// <summary>
        /// Создаёт новый экземпляр синхронизатора расписания и имён преподавателей.
        /// </summary>
        /// <param name="path">Путь до каталога, где должны содержаться excel файлы.</param>
        /// <param name="isNeedDownload">Укажите True, если надо скачать базу данных файлов.</param>
        /// <exception cref="System.IO.IOException">Возникает тогда, когда при отсутствии заданной
        /// папки возникает неудачная попытка создать данную папку и её родительские каталоги.</exception>
        public ExternalDataUpdater(string path, bool isNeedDownload)
        {
            if (path != null)
            {
                pathToCache = new DirectoryInfo(path);
                createCacheDir();
                if (isNeedDownload)
                    download(new PercentReady());
            }
            else
                pathToCache = null;
        }

        public ExternalDataUpdater(DirectoryInfo path, bool isNeedDownload, PercentReady pr)
        {
            pathToCache = path;
            if (path != null)
            {
                createCacheDir();
                if (isNeedDownload)
                    download(pr); // first download
            }
        }

        private void createCacheDir()
        {
            pathToCache.Create();
            if (pathToCache.Attributes.HasFlag(FileAttributes.Directory)
            && !pathToCache.Attributes.HasFlag(FileAttributes.ReadOnly))
                throw new System.IO.IOException("Need directory and not readonly.");
        }

        public ExternalDataUpdater(List<FileInfo> excelFiles, List<Teacher> teachers)
        {
            pathToCache = null;
            if (excelFiles != null)
                this.excelFiles = excelFiles;
            else
                this.excelFiles = new List<FileInfo>();
            if (teachers != null)
                this.teachers = teachers;
            else
                this.teachers = new List<Teacher>();
        }

        /// <summary>
        /// Путь до временных файлов.
        /// </summary>
        public readonly DirectoryInfo pathToCache;

        /// <summary>
        /// Данный монитор выступает в роли синхронизации потоков.
        /// </summary>
        private Thread myThread = null;

        /// <summary>
        /// Генератор случайных чисел
        /// </summary>
        private static readonly System.Random random = new System.Random();

        /**
         * Расположение доступных файлов Excel
         */
        private List<FileInfo> excelFiles = new List<FileInfo>();

        /// <summary>
        /// Список преподавателей.
        /// </summary>
        private List<Teacher> teachers = new List<Teacher>();

        /// <summary>
        /// Объект, который надо обновлять после получения обновлений.
        /// </summary>
        private UpdateCache needUpdate;

        /// <summary>
        /// Функция отвечает за то, чтобы получить таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
        /// Стоит отметить, что если в кэше есть не устаревшие таблицы, то функция
        /// вернёт таблицы из кэша.
        /// Файлы надо закрывать методом <see cref="EnumeratorExcels.Dispose()">!
        /// Если вам по-середине понадобилось закрыть все файлы, вам всё равно придётся все элементы перебрать и закрыть.
        /// </summary>
        /// <returns>Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>. Вызвать метод .close обязательно.</returns>
        public EnumeratorExcels OpenTablesFromExternal()
        {
            return new EnumeratorExcels(excelFiles);
        }

        /**
         * Функция ведёт поиск полного имени преподавателя.
         * @param nameInExcel Краткое имя преподавателя.
         * @return Полное имя и должность преподавателя.
         */
        public String findTeacher(String nameInExcel)
        {
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
        public void run()
        {
            if (myThread != null)
            {
                if (!myThread.isInterrupted())
                    myThread.interrupt();
            }
            myThread = new Thread(this.threadBody);
            myThread.start();
        }

        public bool isAlive()
        {
            return myThread != null && myThread.isAlive();
        }

        public void interrupt()
        {
            if (!myThread.isInterrupted())
                myThread.interrupt();
            myThread = null;
        }

        private void threadBody()
        {
            if (this.pathToCache != null)
                try
                {
                    while (!Thread.currentThread().isInterrupted())
                    {
                        Thread.sleep(1000 * 60 * 60 * 8); // Проверка три раза в день.
                        download(new PercentReady(new SampleConsoleTransferPercentReady("ExternalDataUpdater.java: download xls: ")));
                    }
                }
                catch (InterruptedException e)
                {
                    // finish.
                }
        }

        private readonly object lock_download = new object();

        /// <summary>
        /// Функция скачивает необходимый контент в папку кэша.
        /// </summary>
        /// <param name="pr">Класс для управления процентом загрузки.</param>
        private void download(PercentReady pr)
        {
            lock (lock_download)
            {
                PercentReady PR_loader = new PercentReady(pr, 0.05f);
                PercentReady PR_downloader = new PercentReady(pr, 0.25f);
                PercentReady PR_install = new PercentReady(pr, 0.70f);
                if (pathToCache == null)
                {
                    Console.WriteLine("Can't update: pathToCache is null.");
                    return;
                }
                PR_loader.setReady(0.1f);
                Stream<String> htmlExcels = downloadHTML("https://www.mirea.ru/education/schedule-main/schedule/");
                PR_loader.setReady(0.2f);
                List<String> excelUrls = findAllExcelURLs(htmlExcels);
                PR_loader.setReady(0.3f);
                htmlExcels.close();
                PR_loader.setReady(0.4f);
                // ---- https -> http
                String elm;
                for (int i = excelUrls.size() - 1; i >= 0; i--)
                {
                    elm = excelUrls.get(i);
                    Console.Write(ZonedDateTime.now() + " ExternalDataUpdater.java: " + elm + " ;\t");
                    elm = elm.replaceFirst("https:/", "http:/");
                    excelUrls.set(i, elm);
                }
                PR_loader.setReady(0.5f);
                Console.WriteLine();
                // ----
                try
                {
                    PR_loader.setReady(0.6f);
                    excelFiles = downloadFilesToPath(excelUrls, this.pathToCache, PR_downloader);
                    PR_loader.setReady(0.7f);

                    Stream<String> htmlNames = downloadHTML("https://www.mirea.ru/sveden/employees/");
                    PR_loader.setReady(0.8f);
                    teachers = getTeachers(htmlNames);
                    PR_loader.setReady(0.9f);
                }
                catch (Exception e)
                {
                    e.printStackTrace();
                    Console.WriteLine(e.getLocalizedMessage());
                }
                if (needUpdate != null)
                {
                    try
                    {
                        needUpdate.update(PR_install);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(ZonedDateTime.now() + " ExternalDataUpdater.java: " + e.getLocalizedMessage());
                    }
                    PR_install.setReady(1f);
                }
                PR_loader.setReady(1f);
            }
        }

        /// <summary>
        /// Получает преподавателей из HTML потока.
        /// </summary>
        /// <param name="htmlNames">HTML код, где таблица преподавателей.</param>
        /// <returns>Список преподавателей.</returns>
        private List<Teacher> getTeachers(Stream<String> htmlNames)
        {
            if (htmlNames == null)
                return null;
            StringBuilder html = new StringBuilder();
            Pattern pFio = Pattern.compile("<td itemprop=\"fio\">");
            Pattern pPost = Pattern.compile("<td itemprop=\"post\">");
            Pattern pEnd = Pattern.compile("</td>");
            bool needEnd = false;
            Iterator<String> iteratorHtmlNames = htmlNames.iterator();
            while (iteratorHtmlNames.hasNext())
            {
                String str = iteratorHtmlNames.next();
                Matcher mFio = pFio.matcher(str);
                Matcher mPost = pPost.matcher(str);
                Matcher mEnd = pEnd.matcher(str);
                bool isAdd = false; // Необходимо, чтобы дважды не добавить одну и ту же строку.
                if (mFio.find() || mPost.find() || needEnd)
                {
                    html.append(' ');
                    html.append(str);
                    isAdd = true;
                    needEnd = true;
                }
                if (mEnd.find())
                {
                    if (!isAdd) html.append(str);
                    needEnd = false;
                }
            }
            Matcher mFio = pFio.matcher(html);
            List<String> namesOfTeachers = new List<>();
            while (mFio.find())
            {
                int indexStart = mFio.end() + 1;
                int indexFinish = html.indexOf("</td", indexStart) - 1;
                namesOfTeachers.add(html.substring(indexStart, indexFinish));
            }
            Matcher mPost = pPost.matcher(html);
            List<String> posts = new List<>();
            while (mPost.find())
            {
                int indexStart = mPost.end() + 1;
                int indexFinish = html.indexOf("</td", indexStart) - 1;
                posts.add(html.substring(indexStart, indexFinish));
            }
            if (namesOfTeachers.size() != posts.size())
                throw new Exception("size of Teachers " + namesOfTeachers.size() + " not equals size Posts " + posts.size() + " .");

            List<Teacher> teachers = new List<>(posts.size());
            Iterator<String> iPost = posts.iterator();
            Iterator<String> iName = namesOfTeachers.iterator();
            while (iPost.hasNext() && iName.hasNext())
            {
                teachers.add(new Teacher(iName.next(), iPost.next()));
            }
            return teachers;
        }

        /// <summary>
        /// Загружает все файлы по URL и помещает в директорию кэша.
        /// </summary>
        /// <param name="excelUrls">Коллекция ссылок на скачивание.</param>
        /// <param name="pathToCache">Путь до местоположения временных файлов, место загрузки.</param>
        /// <param name="pr">Доступ к управлению процентом готовности.</param>
        /// <returns>Список скаченных файлов.</returns>
        private List<FileInfo> downloadFilesToPath(Collection<String> excelUrls, DirectoryInfo pathToCache, PercentReady pr = new PercentReady())
        {
            PercentReady PR_writerUrls = new PercentReady(pr, 0.2f);
            PercentReady PR_downloader = new PercentReady(pr, 0.8f);


            int size = excelUrls.size();
            List<File> excelFilesPaths = new List<>(size);
            Iterator<String> it = excelUrls.iterator();
            for (int i = 0; i < size; i++)
            {
                String url = it.next();
                excelFilesPaths.add(new File(pathToCache, LocalDateTime.now().toString().replace(':', '-').replace('.', '_') + "_" + random.nextLong() + "_" + url.substring(url.lastIndexOf("/") + 1)));
                Console.Write(ZonedDateTime.now() + " ExternalDataUpdater.java: " + excelFilesPaths.get(i).toString() + ";\t");
                PR_writerUrls.setReady(i / (float)size);
            }
            Console.WriteLine();
            it = excelUrls.iterator();
            for (int i = 0; i < excelFilesPaths.size(); i++)
            {
                if (!excelFilesPaths.get(i).createNewFile())
                    throw new IOException("can't create new excel file. File = " + excelFilesPaths.get(i).toString());
                try
                {
                    downloadFile(new URL(it.next()), excelFilesPaths.get(i));
                }
                catch (IOException e)
                {
                    Console.WriteLine("ExternalDataUpdater.java: I can't save file " + excelFilesPaths.get(i));
                    excelFilesPaths.remove(i);
                    i--;
                }
                PR_downloader.Ready = i / ((float)size);
            }
            PR_downloader.Ready = 1f;
            PR_writerUrls.Ready = 1f;
            return excelFilesPaths;
        }

        /**
         * Находит все ссылки на файлы excel из потока текста.
         * @param htmlExcels Поток текста html.
         * @return Лист на ссылки excel файлов.
         */
        private List<String> findAllExcelURLs(Stream<String> htmlExcels)
        {
            // href ?= ?"http.+?\.[xX][lL][sS][xX]?"
            List<String> excelsUrls = new List<>();
            Pattern p = Pattern.compile("href ?= ?\"http.+?\\.[xX][lL][sS][xX]?\"");
            htmlExcels.forEach((str)=> {
                Matcher m = p.matcher(str);
                if (m.find())
                {
                    String resultFind = m.group();
                    resultFind = resultFind.substring(resultFind.indexOf("http"), resultFind.lastIndexOf("\""));
                    excelsUrls.add(resultFind);
                }

            });
            return excelsUrls;
        }

        /// <summary>
        /// Передаёт поток на скачивание. Поток надо закрывать!
        /// </summary>
        /// <param name="s">URL ссылка, с которой происходит скачивание.</param>
        /// <returns>Текстовый поток от URL GET запроса.</returns>
        private Stream<String> downloadHTML(String s)
        {
            URL url;
            InputStream @is = null;
            BufferedReader br;
            String line;

            try
            {
                url = new URL(s);
                URLConnection connection = url.openConnection();
                String redirect = connection.getHeaderField("Location");
                if (redirect != null)
                {
                    url = new URL(redirect);
                }
            @is = url.openStream();  // throws an IOException
                br = new BufferedReader(new InputStreamReader(@is));
                return br.lines();
            }
            catch (IOException ioe)
            {
                ioe.printStackTrace();
            }
            return null;
        }

        private void downloadFile(URL url, File fileToWrite)
        {
            URLConnection connection = url.openConnection();
            String redirect = connection.getHeaderField("Location");
            if (redirect != null)
            { // https://stackoverflow.com/questions/18431440/301-moved-permanently
                url = new URL(redirect);
            }

            ReadableByteChannel readableByteChannel = Channels.newChannel(url.openStream());

            FileOutputStream fileOutputStream = new FileOutputStream(fileToWrite);
            fileOutputStream.getChannel().transferFrom(readableByteChannel, 0, Long.MAX_VALUE);

            readableByteChannel.close();
            fileOutputStream.close();
        }

        public void setNeedUpdate(UpdateCache needUpdate)
        {
            this.needUpdate = needUpdate;
        }
    }
}