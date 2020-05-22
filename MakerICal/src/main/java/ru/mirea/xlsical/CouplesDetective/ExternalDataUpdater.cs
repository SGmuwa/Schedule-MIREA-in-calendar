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
using System.Text;
using System.Text.RegularExpressions;
using System.Net;
using ru.mirea.xlsical.CouplesDetective.xl;
using System.Collections;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Класс, который отвечает за синхронизацию с <a href="https://www.mirea.ru/education/">mirea.ru</a>.
    /// Данный класс потокобезопасный.
    /// </summary>
    public class ExternalDataUpdater : IEnumerable<ExcelFileInterface>
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
            if (!pathToCache.Attributes.HasFlag(FileAttributes.Directory)
            || pathToCache.Attributes.HasFlag(FileAttributes.ReadOnly))
                throw new System.IO.IOException(
                    $"Need directory ({(pathToCache.Attributes.HasFlag(FileAttributes.Directory) ? "ok" : "fail")}) "
                    + $"and not readonly ({(!pathToCache.Attributes.HasFlag(FileAttributes.ReadOnly) ? "ok" : "fail")}): "
                    + $"{pathToCache.FullName}, {pathToCache.Attributes}");
        }

        public ExternalDataUpdater(List<FileInfo> excelFiles, List<Teacher> teachers)
        {
            pathToCache = null;
            if (excelFiles != null)
                this.excelFiles = excelFiles;
            if (teachers != null)
                this.teachers = teachers;
        }

        public ExternalDataUpdater(IEnumerable<Stream> excelStreams, List<Teacher> teachers)
        {
            pathToCache = null;
            if (excelFiles != null)
                this.excelStreams = excelStreams;
            if (teachers != null)
                this.teachers = teachers;
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

        /// <summary>
        /// Расположение доступных файлов Excel
        /// </summary>
        private List<FileInfo> excelFiles = new List<FileInfo>();

        /// <summary>
        /// Работающие потоки чтения Excel файлов.
        /// </summary>
        private IEnumerable<Stream> excelStreams = null;

        /// <summary>
        /// Список преподавателей.
        /// </summary>
        private List<Teacher> teachers = new List<Teacher>();

        /// <summary>
        /// Объект, который надо обновлять после получения обновлений.
        /// </summary>
        private event UpdateCache needUpdate;

        /// <summary>
        /// Функция отвечает за то, чтобы получить таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>.
        /// Стоит отметить, что если в кэше есть не устаревшие таблицы, то функция
        /// вернёт таблицы из кэша.
        /// Если вам по-середине понадобилось закрыть все файлы, вам всё равно придётся все элементы перебрать и закрыть.
        /// </summary>
        /// <returns>Возвращает все таблицы из сайта <a href="https://www.mirea.ru/education/schedule-main/schedule/">mirea.ru</a>. Вызвать метод .close обязательно.</returns>
        public IEnumerator<ExcelFileInterface> GetEnumerator()
        {
            if(excelFiles?.Count != 0)
                return new EnumeratorExcelsFileInfo(excelFiles);
            if(excelStreams != null)
                return new EnumeratorExcelsStream(excelStreams);
            else
                return System.Linq.Enumerable.Empty<ExcelFileInterface>().GetEnumerator();
        }

        /// <summary>
        /// Функция ведёт поиск полного имени преподавателя.
        /// </summary>
        /// <param name="nameInExcel">Краткое имя преподавателя.</param>
        /// <returns>Полное имя и должность преподавателя.</returns>
        public string findTeacher(string nameInExcel)
        {
#warning Необходимо реализовать функционал.
            return nameInExcel;
            //String[] fullName = Teacher.ConvertNameFromStrToArray(nameInExcel);
            //return findTeacher(fullName[0], fullName[1], fullName[2]);
        }

        //private String findTeacher(String surname, String firstName, String partonymic) {
        //Collections.binarySearch(teachers, surname, (left, right) -> )
        //}

        /// <summary>
        /// Запускает механизм автоматического обновления, скачивания данных
        /// из сайта <a href="https://www.mirea.ru/education/">mirea.ru</a>.
        /// Если уже запущен, то будет вызван <see cref="Stop"/>, а затем
        /// запустится механизм.
        /// </summary>
        public void Run()
        {
            Stop();
            myThread = new Thread(this.threadBody);
            myThread.Start();
        }

        /// <summary>
        /// Останавливает механизм автоматического обновления.
        /// Если механизм уже остановлен, то метод ничего не делает.
        /// </summary>
        public void Stop()
        {
            if (myThread != null)
            {
                myThread.Interrupt();
                myThread = null;
            }
        }

        private void threadBody()
        {
            if (this.pathToCache != null)
                try
                {
                    Thread.Sleep(1000 * 60 * 60 * 8); // Проверка три раза в день.
                    download(new PercentReady(subscribers: new SampleConsoleTransferPercentReady("ExternalDataUpdater.java: download xls: ").TransferValue));
                }
                catch (ThreadInterruptedException)
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
                PR_loader.Ready = 0.1f;
                StreamReader htmlExcels = downloadHTML("https://www.mirea.ru/schedule/");
                PR_loader.Ready = 0.2f;
                List<string> excelUrls = findAllExcelURLs(htmlExcels);
                PR_loader.Ready = 0.3f;
                htmlExcels.Dispose();
                PR_loader.Ready = 0.4f;
                // ---- https -> http
                string elm;
                for (int i = 0; i < excelUrls.Count; i++)
                {
                    elm = excelUrls[i];
                    Console.Write(DateTime.UtcNow + " ExternalDataUpdater.java: " + elm + " ;\t");
                    elm = elm.Replace("https:/", "http:/");
                    excelUrls[i] = elm;
                }
                PR_loader.Ready = 0.5f;
                Console.WriteLine();
                // ----
                try
                {
                    PR_loader.Ready = 0.6f;
                    excelFiles = downloadFilesToPath(excelUrls, this.pathToCache, PR_downloader);
                    PR_loader.Ready = 0.7f;

                    StreamReader htmlNames = downloadHTML("https://www.mirea.ru/sveden/employees/");
                    PR_loader.Ready = 0.8f;
                    teachers = getTeachers(htmlNames);
                    PR_loader.Ready = 0.9f;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + " " + e.StackTrace);
                }
                if (needUpdate != null)
                {
                    try
                    {
                        needUpdate?.Invoke(PR_install);
                    }
                    catch (IOException e)
                    {
                        Console.WriteLine(DateTime.UtcNow + " ExternalDataUpdater.java: " + e.Message);
                    }
                    PR_install.Ready = 1f;
                }
                PR_loader.Ready = 1f;
            }
        }

        private static readonly Regex pFio = new Regex("<td itemprop=\"fio\">");
        private static readonly Regex pPost = new Regex("<td itemprop=\"post\">");
        private static readonly Regex pEnd = new Regex("</td>");

        /// <summary>
        /// Получает преподавателей из HTML потока.
        /// </summary>
        /// <param name="htmlNames">HTML код, где таблица преподавателей.</param>
        /// <returns>Список преподавателей.</returns>
        private List<Teacher> getTeachers(StreamReader htmlNames)
        {
            if (htmlNames == null)
                return null;
            StringBuilder htmlBuild = new StringBuilder();
            bool needEnd = false;
            Match mFio;
            Match mPost;
            for (string str = htmlNames.ReadLine(); str != null; str = htmlNames.ReadLine())
            {
                mFio = pFio.Match(str);
                mPost = pPost.Match(str);
                Match mEnd = pEnd.Match(str);
                bool isAdd = false; // Необходимо, чтобы дважды не добавить одну и ту же строку.
                if (mFio.Success || mPost.Success || needEnd)
                {
                    htmlBuild.Append(' ');
                    htmlBuild.Append(str);
                    isAdd = true;
                    needEnd = true;
                }
                if (mEnd.Success)
                {
                    if (!isAdd) htmlBuild.Append(str);
                    needEnd = false;
                }
            }
            string html = htmlBuild.ToString();
            mFio = pFio.Match(html);
            List<string> namesOfTeachers = new List<string>();
            while (mFio.Success)
            {
                int indexStart = mFio.Index + mFio.Length + 1;
                int indexFinish = html.IndexOf("</td", indexStart) - 1;
                namesOfTeachers.Add(html.Substring(indexStart, indexFinish - indexStart));
            }
            mPost = pPost.Match(html);
            List<string> posts = new List<string>();
            while (mPost.Success)
            {
                int indexStart = mPost.Index + mPost.Length + 1;
                int indexFinish = html.IndexOf("</td", indexStart) - 1;
                posts.Add(html.Substring(indexStart, indexFinish));
            }
            if (namesOfTeachers.Count != posts.Count)
                throw new Exception("size of Teachers " + namesOfTeachers.Count + " not equals size Posts " + posts.Count + " .");

            List<Teacher> teachers = new List<Teacher>(posts.Count);
            IEnumerator<string> iPost = posts.GetEnumerator();
            IEnumerator<string> iName = namesOfTeachers.GetEnumerator();
            while (iPost.MoveNext() && iName.MoveNext())
            {
                teachers.Add(new Teacher(iName.Current, iPost.Current));
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
        private List<FileInfo> downloadFilesToPath(ICollection<string> excelUrls, DirectoryInfo pathToCache, PercentReady pr = null)
        {
            if (pr == null)
                pr = new PercentReady();
            PercentReady PR_writerUrls = new PercentReady(pr, 0.2f);
            PercentReady PR_downloader = new PercentReady(pr, 0.8f);


            int size = excelUrls.Count;
            List<FileInfo> excelFilesPaths = new List<FileInfo>(size);
            IEnumerator<string> it = excelUrls.GetEnumerator();
            for (int i = 0; i < size; i++)
            {
                String url = it.Current;
                excelFilesPaths.Add(new FileInfo(Path.Combine(pathToCache.FullName, DateTime.UtcNow.ToString().Replace(':', '-').Replace('.', '_') + "_" + random.NextLong() + "_" + url.Substring(url.LastIndexOf("/") + 1))));
                Console.Write(DateTime.UtcNow + " ExternalDataUpdater.java: " + excelFilesPaths[i].ToString() + ";\t");
                PR_writerUrls.Ready = i / (float)size;
            }
            Console.WriteLine();
            it = excelUrls.GetEnumerator();
            for (int i = 0; i < excelFilesPaths.Count && it.MoveNext(); i++)
            {
                try
                {
                    downloadFile(new Uri(it.Current), excelFilesPaths[i].Create());
                }
                catch (IOException e)
                {
                    Console.WriteLine("ExternalDataUpdater.java: I can't save file " + excelFilesPaths[i] + e.Message);
                    excelFilesPaths.RemoveAt(i);
                    i--;
                }
                PR_downloader.Ready = i / ((float)size);
            }
            PR_downloader.Ready = 1f;
            PR_writerUrls.Ready = 1f;
            return excelFilesPaths;
        }

        private static readonly Regex httpXlsRegex = new Regex(@"href ?= ?""http.+?\.[xX][lL][sS][xX]?""");

        /// <summary>
        /// Находит все ссылки на файлы excel из потока текста.
        /// </summary>
        /// <param name="htmlExcels">Поток текста html.</param>
        /// <returns>Лист на ссылки excel файлов.</returns>
        private static List<string> findAllExcelURLs(StreamReader htmlExcels)
        {
            List<string> excelsUrls = new List<string>();
            string str;
            while ((str = htmlExcels.ReadLine()) != null)
            {
                MatchCollection matches = httpXlsRegex.Matches(str);
                foreach (Match m in matches)
                {
                    int start = m.Value.IndexOf("http");
                    int length = m.Value.LastIndexOf('"') - start;
                    excelsUrls.Add(m.Value.Substring(start, length));
                }
            }
            return excelsUrls;
        }

        /// <summary>
        /// Передаёт поток на скачивание. Поток надо закрывать!
        /// </summary>
        /// <param name="s">URL ссылка, с которой происходит скачивание.</param>
        /// <returns>Текстовый поток от URL GET запроса.</returns>
        private StreamReader downloadHTML(string s)
        {
            WebClient client = new WebClient();
            StreamWithEvent stream = new StreamWithEvent(client.OpenRead(s));
            stream.Disposed += (c, _) => ((WebClient)c).Dispose();
            return new StreamReader(stream);
        }

        private void downloadFile(Uri url, FileStream fileToWrite)
        {
            using WebClient client = new WebClient();
            using Stream stream = client.OpenRead(url);
            stream.CopyTo(fileToWrite);
            fileToWrite.Dispose();
        }

        public void setNeedUpdate(UpdateCache needUpdate)
        {
            this.needUpdate = needUpdate;
        }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
