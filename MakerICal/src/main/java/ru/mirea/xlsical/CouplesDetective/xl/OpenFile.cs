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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
        private int needToClose;

        private SetInt closed;

        private readonly string fileName;

        private readonly SpreadsheetDocument document;

        private readonly int numberSheet;

#warning Кэш можно улучшить, если сохранять только цвет и текст.
        private readonly Dictionary<(int column, int row), Cell> cellCache = new Dictionary<(int column, int row), Cell>();

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
            SetInt setInt = new SetInt();
            OpenFile first = new OpenFile(fileName, 0);
            int size = first.document.WorkbookPart.Workbook.Descendants<Sheet>().Count();
            List<ExcelFileInterface> @out = new List<ExcelFileInterface>(size);
            @out.Add(first);
            first.needToClose = size;
            first.closed = setInt;

            for (int i = 1; i < size; i++)
                @out.Add(new OpenFile(first.document, i, size, setInt, fileName.Name));
            return @out;
        }

#warning Должен возвращаться лист, поддерживающий using.
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
            SetInt setInt = new SetInt();
            OpenFile first = new OpenFile(stream, 0);
            int size = first.document.WorkbookPart.Workbook.Descendants<Sheet>().Count();
            List<ExcelFileInterface> @out = new List<ExcelFileInterface>(size);
            @out.Add(first);
            first.needToClose = size;
            first.closed = setInt;

            for (int i = 1; i < size; i++)
                @out.Add(new OpenFile(first.document, i, size, setInt, stream.ToString()));
            return @out;
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

        /// <summary>
        /// Получение данных в текстовом виде из указанной ячейки Excel файла.
        /// </summary>
        /// <param name="column">Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row">Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <returns>Текстовые данные в ячейке. Не NULL.</returns>
        /// <exception cref="System.IO.IOException">Потерян доступ к файлу.</exception>
        public string GetCellData(int column, int row)
        {
            Cell cell = getCell(column, row);
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
            Cell cellA = getCell(column1, row1);
            Cell cellB = getCell(column2, row2);
            if (cellA == null || cellB == null)
                return cellA == null && cellB == null;
            return GetCellPatternFill(cellA).Equals(GetCellPatternFill(cellB));
        }

        private PatternFill GetCellPatternFill(Cell theCell)
        {
            WorkbookStylesPart styles = document.WorkbookPart.WorkbookStylesPart;
            int cellStyleIndex = theCell.StyleIndex == null ? 0 : (int)theCell.StyleIndex.Value;
            CellFormat cellFormat = (CellFormat)styles.Stylesheet.CellFormats.ChildElements[cellStyleIndex];
            Fill fill = (Fill)styles.Stylesheet.Fills.ChildElements[(int)cellFormat.FillId.Value];
            return fill.PatternFill;
        }

        private bool isOpen = true;

        private readonly object sync_close = new object();

        /// <summary>
        /// Закрывает Excel файл.
        /// </summary>
        /// <exception cref="System.IO.IOException">Ошибка при закрытии файла.</exception>
        public void Dispose()
        {
            lock (sync_close)
                if (isOpen && closed.Value < needToClose)
                {
                    isOpen = false;
                    closed.Add();
                    if (closed.Value == needToClose)
                        document.Dispose();
                }
        }

        /// <summary>
        /// Создаёт экземпляр открытия файла.
        /// Для открытия всех листов Excel файла используйте <see cref="NewInstances(string)"/>.
        /// </summary>
        /// <param name="fileName">Имя файла, который необходимо открыть.</param>
        /// <param name="numberSheet">Номер страницы книги Excel.</param>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования файла.</exception>
        private OpenFile(FileInfo fileName, int numberSheet)
        {
            this.document = SpreadsheetDocument.Open(fileName.FullName, false);
            this.numberSheet = numberSheet;
            this.fileName = fileName.Name;
            CreateCache();
        }



        /// <summary>
        /// Создаёт экземпляр открытия файла.
        /// Для открытия всех листов Excel файла используйте <see cref="NewInstances(string)"/>.
        /// </summary>
        /// <param name="stream">Поток, который необходимо открыть.</param>
        /// <param name="numberSheet">Номер страницы книги Excel.</param>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования файла.</exception>
        private OpenFile(Stream stream, int numberSheet)
        {
            this.document = SpreadsheetDocument.Open(stream, false);
            this.numberSheet = numberSheet;
            this.fileName = stream.ToString();
            CreateCache();
        }



        /// <summary>
        /// Создаёт экземпляр открытия файла.
        /// Для открытия всех листов Excel файла используйте <see cref="NewInstances(string)"/>.
        /// </summary>
        /// <param name="document">Открытый документ.</param>
        /// <param name="numberSheet">Номер страницы книги Excel.</param>
        /// <param name="needToClose">Количество открытых листов у Excel файла при newInstance.</param>
        /// <param name="closed">Количество закрытых листов Excel у файла.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования файла.</exception>
        private OpenFile(SpreadsheetDocument document, int numberSheet, int needToClose, SetInt closed, string fileName)
        {
            this.document = document;
            this.numberSheet = numberSheet;
            this.needToClose = needToClose;
            this.closed = closed;
            this.fileName = fileName;
            CreateCache();
        }

        private void CreateCache()
        {
            WorkbookPart wbPart = document.WorkbookPart;
            Sheet myExcelSheet = wbPart.Workbook.Descendants<Sheet>().ElementAt(numberSheet);
            WorksheetPart wsPart = (WorksheetPart)(wbPart.GetPartById(myExcelSheet.Id));
            foreach (Cell cell in wsPart.Worksheet.Descendants<Cell>())
                cellCache[StaticTools.AddressToCoordinate(cell.CellReference)] = cell;
        }

        /// <summary>
        /// Получение ячейки по номеру колонки и строки.
        /// </summary>
        /// <param name="column">Порядковый номер колонки.</param>
        /// <param name="row">Порядковый номер строки.</param>
        /// <returns>Ячейка по данному адресу.</returns>
        private Cell getCell(int column, int row)
        {
            if (column < 1 || row < 1)
                throw new System.ArgumentException("column and row must be more 0.");
            return cellCache.GetValueOrDefault((column, row));
        }

        public override string ToString()
        => $"{nameof(OpenFile)} {{" +
            $" {nameof(needToClose)} = {needToClose}" +
            $", {nameof(closed)} = {closed}" +
            $", {nameof(fileName)} = '{fileName}'" +
            $", {nameof(isOpen)} = {isOpen}" +
            $", {nameof(document)} = {document}" +
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
        }
    }
}
