/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)
    George Andreevich Falileev

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
using NodaTime;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using ru.mirea.xlsical.CouplesDetective;
using ru.mirea.xlsical.CouplesDetective.xl;
using ru.mirea.xlsical.interpreter;
using ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

namespace ru.mirea.xlsical.Server
{
    /// <summary>
    /// Класс, который выступает в роле исполнителя обработчика
    /// из excel файлов в ics расписание.
    /// Используйте <see cref="add(PackageToMakerICal)"/> для добавления задания.
    /// Используйте <see cref="take" для получения ответа.
    /// </summary>
    public class TaskExecutor
    {
        private readonly BlockingCollection<PackageToMakerICal> qIn;
        private readonly BlockingCollection<PackageToProviderHTTP> qOut;
        private readonly CoupleHistorian coupleHistorian;

        public TaskExecutor(CoupleHistorian manualHistorian = null)
        {
            this.qIn = new BlockingCollection<PackageToMakerICal>(new ConcurrentQueue<PackageToMakerICal>());
            this.qOut = new BlockingCollection<PackageToProviderHTTP>(new ConcurrentQueue<PackageToProviderHTTP>());
            this.coupleHistorian = manualHistorian == null ? new CoupleHistorian(DateTimeZone.Utc.AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow))) : manualHistorian;
        }

        public TaskExecutor(PercentReady pr)
        : this(new CoupleHistorian(DateTimeZone.Utc.AtStrictly(LocalDateTime.FromDateTime(DateTime.UtcNow)), pr))
        { }

        /// <summary>
        /// Получает готовый элемент из очереди и удаляет его из очереди.
        /// Если выходная очередь пуста, то ждёт появления элемента.
        /// В случае, если ожидание прервать, то сработает исключение.
        /// </summary>
        /// <returns>Пакет от обработчика.</returns>
        public PackageToProviderHTTP take() => qOut.Take();

        /// <summary>
        /// Добавляет элемент в очередь задач.
        /// </summary>
        /// <param name="pack">Пакет с требованиями к решению задачи.</param>
        public void add(PackageToMakerICal pack) => qIn.Add(pack);

        /// <summary>
        /// Запускает выполнение задач до тех пор, пока не вызовется interrupt потока.
        /// По факту - циклический вызов <see cref="step"/>.
        /// </summary>
        public void run()
        {
            while (!Thread.CurrentThread.ThreadState.HasFlag(ThreadState.AbortRequested))
                try
                {
                    step();
                }
                catch (System.Threading.ThreadAbortException)
                {
                    return;
                }
        }

        /// <summary>
        /// Берёт из входной очереди <see cref="add(PackageToMakerICal)"/> входной элемент,
        /// и отправляет его в выходную очередь <see cref="take"/>.
        /// </summary>
        /// <exception cref="OperationCanceledException">Срабатывает исключение в случае прерывания ожидания из входной очереди.</exception>
        public void step() => qOut.Add(monoStep(qIn.Take()));

        /// <summary>
        /// Выполняет обработку пакета без использования очередей.
        /// Не рекомендуется использовать данный метод, так как он
        /// заставляет ждать выполнения работы входной поток.
        /// </summary>
        /// <param name="pkg">Пакет с требованиями к решению задачи.</param>
        /// <returns>Пакет от обработчика.</returns>
        public PackageToProviderHTTP monoStep(PackageToMakerICal pkg)
        {
            if (pkg == null)
            {
                return new PackageToProviderHTTP(null, null, 0, "Ошибка: была предпринята попытка обработать пустой пакет.");
            }
            if (pkg.queryCriteria == null)
            {
                pkg.percentReady.Ready = 1.0f;
                return new PackageToProviderHTTP(pkg.Context, null, 0, "Ошибка: отсутствуют критерии поиска.");
            }
            List<CoupleInCalendar> couples = coupleHistorian.getCouples(pkg.queryCriteria, new PercentReady(pkg.percentReady, 0.6f));
            FileInfo iCalFile = ExportCouplesToICal.start(couples, new PercentReady(pkg.percentReady, 0.4f));
            Console.WriteLine(iCalFile);
            if (iCalFile != null)
                return new PackageToProviderHTTP(
                    pkg.Context,
                    iCalFile,
                    couples.Count,
                    "ok.");
            else
                return new PackageToProviderHTTP(
                    pkg.Context,
                    null,
                    couples.Count,
                    "empty.");
        }

        /// <summary>
        /// Открывает все excel файлы, которые были переданы из массива путей до файлов.
        /// </summary>
        /// <param name="filesStr">Массив строк, который символизируют путь до файлов Excel.</param>
        /// <returns>Массив-список открытых Excel файлов.</returns>
        private static List<ExcelFileInterface> openExcelFiles(string[] filesStr)
        {
            List<ExcelFileInterface> output = new List<ExcelFileInterface>(filesStr.Length);
            for (int index = filesStr.Length - 1; index >= 0; index--)
                output.AddRange(openExcelFiles(filesStr[index]));
            return output;
        }

        /// <summary>
        /// Открывает все Excel файлы, которые были переданы в колекции путей до файлов.
        /// </summary>
        /// <param name="filesStr">Коллекция строк, который символизируют путь до файлов Excel.</param>
        /// <returns>Массив-список открытых Excel файлов.</returns>
        private static List<ExcelFileInterface> openExcelFiles(ICollection<string> filesStr)
        {
            int size = filesStr.Count;
            List<ExcelFileInterface> output = new List<ExcelFileInterface>(size);
            foreach (string str in filesStr)
                output.AddRange(openExcelFiles(str));
            return output;
        }

        /// <summary>
        /// Открывает все Excel файлы, которые были переданы из получателя путей до файлов.
        /// </summary>
        /// <param name="filesStr">Перечеслитель строк, который символизируют путь до файлов Excel.</param>
        /// <returns>Связный-список открытых Excel файлов.</returns>
        private static LinkedList<ExcelFileInterface> openExcelFiles(IEnumerable<string> filesStr)
        {
            LinkedList<ExcelFileInterface> output = new LinkedList<ExcelFileInterface>();
            foreach (String str in filesStr)
                output.AddLastRange(openExcelFiles(str));
            return output;
        }

        /// <summary>
        /// Открывает Excel файл, который был передан по fileStr.
        /// </summary>
        /// <param name="fileStr">Путь до файла Excel.</param>
        /// <returns>Список открытых Excel файлов.</returns>
        private static List<ExcelFileInterface> openExcelFiles(string fileStr)
        {
            FileInfo a = new FileInfo(fileStr);
            try
            {
                return OpenFile.NewInstances(a);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message + e.StackTrace);
                return new List<ExcelFileInterface>();
            }
        }
    }
}
