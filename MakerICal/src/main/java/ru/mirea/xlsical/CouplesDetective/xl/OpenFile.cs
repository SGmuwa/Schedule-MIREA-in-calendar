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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace ru.mirea.xlsical.CouplesDetective.xl
{
    /// <summary>
    /// Класс реализует интерфейс <see cref="ExcelFileInterface"/>.
    /// Используйте <see cref="OpenFile.NewInstances(string)"/>
    /// Он который является переходным между
    /// <see cref="ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective"/>
    /// и <see cref="SpreadsheetDocument"/>.
    /// </summary>
    public class OpenFile : ExcelFileInterface
    {
        private readonly string fileName;

        private readonly int numberSheet;

        private (Task task, CancellationTokenSource token)? cacheTask = null;

        private readonly ConcurrentDictionary<(int column, int row), (string text, PatternFill color)> cellCache = new ConcurrentDictionary<(int column, int row), (string text, PatternFill color)>();

#warning Должен возвращаться лист, поддерживающий using.
        /// <summary>
        /// Открывает Excel файл вместе со всеми его листами.
        /// </summary>
        /// <param name="fileName">Путь до файла, который необходимо открыть.</param>
        /// <returns>Возвращает список открытых листов.</returns>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования .xls или .xlsx файла.</exception>
        public static List<ExcelFileInterface> NewInstances(FileInfo fileName)
        {
            if (fileName == null)
                throw new System.ArgumentNullException(nameof(fileName));
            if (!fileName.Exists)
                throw new System.IO.FileNotFoundException(fileName.FullName);
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(fileName.FullName, false))
                return NewInstances(document, fileName.FullName);
        }

        /// <summary>
        /// Открывает Excel файл вместе со всеми его листами.
        /// </summary>
        /// <param name="stream">Поток файла, в котором содержится Excel файл.</param>
        /// <returns>Возвращает список открытых листов.</returns>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования .xls или .xlsx файла.</exception>
        public static List<ExcelFileInterface> NewInstances(Stream stream)
        {
            if (stream == null)
                throw new System.ArgumentNullException(nameof(stream));
            using (SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false))
                return NewInstances(document, stream.ToString());
        }

        public static List<ExcelFileInterface> NewInstances(SpreadsheetDocument document, string documentName)
        {
            if (document == null)
                throw new System.ArgumentNullException(nameof(document));
            int size = document.WorkbookPart.Workbook.Descendants<Sheet>().Count();
            List<ExcelFileInterface> output = new List<ExcelFileInterface>(size);
            for (int i = 0; i < size; i++)
            {
                OpenFile toAdd = new OpenFile(i, documentName);
                toAdd.CreateCache(document).task.Wait();
                output.Add(toAdd);
            }
            return output;
        }

        public static List<ExcelFileInterface> NewInstancesParallel(FileInfo fileName, Action callBack = null)
        {
            if (fileName == null)
                throw new System.ArgumentNullException(nameof(fileName));
            if (!fileName.Exists)
                throw new System.IO.FileNotFoundException(fileName.FullName);
            SpreadsheetDocument document = SpreadsheetDocument.Open(fileName.FullName, false);
            return NewInstancesParallel(document, fileName.FullName, () => { document.Dispose(); callBack?.Invoke(); });
        }

        public static List<ExcelFileInterface> NewInstancesParallel(Stream stream, Action callBack = null)
        {
            if (stream == null)
                throw new System.ArgumentNullException(nameof(stream));
            SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false);
            var output = NewInstancesParallel(document, stream.ToString(), () => { document.Dispose(); stream.Dispose(); callBack?.Invoke(); });
            return output;
        }

        public static List<ExcelFileInterface> NewInstancesParallel(SpreadsheetDocument document, string documentName, Action callBack = null)
        {
            if (document == null)
                throw new System.ArgumentNullException(nameof(document));
            int size = document.WorkbookPart.Workbook.Descendants<Sheet>().Count();
            List<ExcelFileInterface> output = new List<ExcelFileInterface>(size);
            for (int i = 0; i < size; i++)
            {
                OpenFile toAdd = new OpenFile(i, documentName);
                toAdd.cacheTask = toAdd.CreateCache(document);
                output.Add(toAdd);
            }
            Task.Run(() =>
            {
                try
                {
                    Parallel.ForEach(output.Cast<OpenFile>(), (excel) =>
                    {
                        excel.cacheTask.Value.task.Wait();
                    });
                }
                finally
                {
                    callBack?.Invoke();
                }
            });
            return output;
        }

        /// <summary>
        /// Открывает Excel файл вместе со всеми его листами.
        /// </summary>
        /// <param name="fileName">Путь до файла, который необходимо открыть.</param>
        /// <returns>Возвращает список открытых листов.</returns>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования .xls или .xlsx файла.</exception>
        /// <seealso cref="NewInstances(FileInfo)"/>
        public static List<ExcelFileInterface> NewInstances(string fileName)
            => NewInstances(new FileInfo(fileName ?? throw new System.ArgumentNullException(nameof(fileName))));

        public static List<ExcelFileInterface> NewInstancesParallel(string fileName, Action callBack = null)
            => NewInstancesParallel(new FileInfo(fileName ?? throw new System.ArgumentNullException(nameof(fileName))), callBack);

        /// <summary>
        /// Получение данных в текстовом виде из указанной ячейки Excel файла.
        /// </summary>
        /// <param name="column">Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row">Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <returns>Текстовые данные в ячейке. Не NULL.</returns>
        /// <exception cref="System.IO.IOException">Потерян доступ к файлу.</exception>
        public string GetCellData(int column, int row)
            => getCell(column, row).text;

        /// <summary>
        /// Узнаёт фоновый цвет двух ячеек и отвечает на вопрос, одинаковый ли у них фоновый цвет.
        /// </summary>
        /// <param name="column1">Первая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row1">Первая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <param name="column2">Вторая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row2">Вторая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <returns><c>true</c>, если цвета совпадают. Иначе — <c>false</c>.</returns>
        public bool IsBackgroundColorsEquals(int column1, int row1, int column2, int row2)
        {
            PatternFill cellA = getCell(column1, row1).color;
            PatternFill cellB = getCell(column2, row2).color;
            if (cellA == null || cellB == null)
                return cellA == null && cellB == null;
            return cellA.Equals(cellB);
        }

        /// <summary>
        /// Закрывает Excel файл.
        /// </summary>
        /// <exception cref="System.IO.IOException">Ошибка при закрытии файла.</exception>
        public void Dispose()
        {
            cacheTask?.token.Cancel();
            cacheTask?.token.Dispose();
        }

        /// <summary>
        /// Создаёт экземпляр открытия файла.
        /// Для открытия всех листов Excel файла используйте <see cref="NewInstances(string)"/>.
        /// </summary>
        /// <param name="document">Открытый документ.</param>
        /// <param name="numberSheet">Номер страницы книги Excel.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования файла.</exception>
        private OpenFile(int numberSheet, string fileName)
        {
            this.numberSheet = numberSheet;
            this.fileName = fileName;
        }

        private (Task task, CancellationTokenSource token) CreateCache(SpreadsheetDocument document)
        {
            CancellationTokenSource tokenSource = new CancellationTokenSource();
            return (Task.Run(() =>
            {
                WorkbookPart wbPart = document.WorkbookPart;
                Sheet myExcelSheet = wbPart.Workbook.Descendants<Sheet>().ElementAt(numberSheet);
                WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(myExcelSheet.Id));
                foreach (Cell cell in wsPart.Worksheet.Descendants<Cell>())
                {
                    if(tokenSource.Token.IsCancellationRequested)
                        return;
                    cellCache[StaticTools.AddressToCoordinate(cell.CellReference)] = (StaticTools.CellToString(document, cell), StaticTools.GetCellPatternFill(document, cell));
                }
            }), tokenSource);
        }

        private static readonly (string text, PatternFill color) emptyCellData = ("", new PatternFill());

        /// <summary>
        /// Получение ячейки по номеру колонки и строки.
        /// </summary>
        /// <param name="column">Порядковый номер колонки.</param>
        /// <param name="row">Порядковый номер строки.</param>
        /// <returns>Информация о ячейке по данному адресу.</returns>
        private (string text, PatternFill color) getCell(int column, int row)
        {
            if (column < 1 || row < 1)
                throw new System.ArgumentException("column and row must be more 0.");
            bool isCacheRunning = cacheTask.HasValue && cacheTask.Value.task.Status == TaskStatus.Running;
            if (cellCache.TryGetValue((column, row), out var output))
                return output;
            if (isCacheRunning)
            {
                cacheTask.Value.task.Wait();
                return cellCache.GetValueOrDefault((column, row), emptyCellData);
            }
            return emptyCellData;
        }

        public override string ToString()
        => $"{nameof(OpenFile)} {{" +
            $", {nameof(fileName)} = '{fileName}'" +
            $", count of cache {nameof(cellCache)} = {cellCache.Count}" +
            $", {nameof(numberSheet)} = {numberSheet}" +
            $" }}";

        public static class StaticTools
        {
            public static (int column, int row) AddressToCoordinate(string address)
            {
                const string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                address = address.ToUpper();
                int startRow = -1;
                while (address[++startRow] > '9') ;
                (int column, int row) output = (0, int.Parse(address.Remove(0, startRow)));
                for (int i = 0; i < startRow; i++)
                    output.column = output.column * abc.Length + address[i] - 'A' + 1;
                return output;
            }

            public static string CoordinateToAddress(int column, int row)
            {
                const string abc = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                int two = column / abc.Length - 1;
                if (two > abc.Length)
                    throw new NotSupportedException("column size can't be more than ZZ (702)");
                if (two == -1)
                    return $"{abc[column]}{row}";
                else
                    return $"{abc[two]}{abc[column % abc.Length]}{row}";
            }

            public static PatternFill GetCellPatternFill(SpreadsheetDocument document, Cell theCell)
            {
                WorkbookStylesPart styles = document.WorkbookPart.WorkbookStylesPart;
                int cellStyleIndex = theCell.StyleIndex == null ? 0 : (int)theCell.StyleIndex.Value;
                CellFormat cellFormat = (CellFormat)styles.Stylesheet.CellFormats.ChildElements[cellStyleIndex];
                Fill fill = (Fill)styles.Stylesheet.Fills.ChildElements[(int)cellFormat.FillId.Value];
                return fill.PatternFill;
            }

            public static string CellToString(SpreadsheetDocument document, Cell cell)
            {
                if (cell == null)
                    return "";
                string output;

                if (cell.DataType != null)
                    switch (cell.DataType.Value)
                    {
                        case CellValues.SharedString:
                            var stringTable =
                                document.WorkbookPart.GetPartsOfType<SharedStringTablePart>()
                                    .FirstOrDefault();

                            if (stringTable != null)
                                output =
                                    stringTable.SharedStringTable
                                    .ElementAt(int.Parse(cell.InnerText)).InnerText;
                            else
                                return cell.InnerText;
                            break;
                        case CellValues.Boolean:
                            output = cell.InnerText == "0" ? "ЛОЖЬ" : "ИСТИНА";
                            break;
                        case CellValues.Number:
                            return cell.CellValue.InnerText;
                        case CellValues.String:
                            return cell.CellValue.InnerText;
                        default:
                            return cell.InnerText;
                    }
                else
                    output = cell.InnerText;
                return output;
            }
        }
    }
}
