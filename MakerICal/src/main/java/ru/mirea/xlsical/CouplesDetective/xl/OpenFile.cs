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
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using NodaTime;

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
            SpreadsheetDocument document = SpreadsheetDocument.Open(fileName.FullName, false);
            int size = document.WorkbookPart.Workbook.Descendants<Sheet>().Count();
            List<ExcelFileInterface> @out = new List<ExcelFileInterface>(size);
            Dictionary<uint, String> formatMappings = StaticTools.BuildFormatMappingsFromXlsx(document);

            for (int i = 0; i < size; i++)
                @out.Add(new OpenFile(document, i, size, setInt, fileName.Name, formatMappings));
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
            SpreadsheetDocument document = SpreadsheetDocument.Open(stream, false);
            int size = document.WorkbookPart.Workbook.Descendants<Sheet>().Count();
            List<ExcelFileInterface> @out = new List<ExcelFileInterface>(size);
            Dictionary<uint, String> formatMappings = StaticTools.BuildFormatMappingsFromXlsx(document);

            for (int i = 0; i < size; i++)
                @out.Add(new OpenFile(document, i, size, setInt, stream.ToString(), formatMappings));
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

        private Dictionary<uint, String> FormatMappingsFromXlsx;

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

            var formants = FormatMappingsFromXlsx;

            WorkbookStylesPart styles = document.WorkbookPart.WorkbookStylesPart;
            int cellStyleIndex = cell.StyleIndex == null ? 0 : (int)cell.StyleIndex.Value;
            CellFormat cellFormat = (CellFormat)styles.Stylesheet.CellFormats.ChildElements[cellStyleIndex];
            if (formants.ContainsKey(cellFormat.NumberFormatId))
            {
                CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
                ci.NumberFormat.CurrencyDecimalSeparator = ".";
                double durF = double.Parse(cell.InnerText, NumberStyles.Any, ci);
                var foramt = formants[cellFormat.NumberFormatId];
                if (foramt.Contains("mm"))
                {
                    Duration d = Duration.FromDays(durF);
                    return d.ToString(foramt, null);
                }
                else
                    return durF.ToString(foramt);
            }

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
        /// <param name="document">Открытый документ.</param>
        /// <param name="numberSheet">Номер страницы книги Excel.</param>
        /// <param name="needToClose">Количество открытых листов у Excel файла при newInstance.</param>
        /// <param name="closed">Количество закрытых листов Excel у файла.</param>
        /// <param name="fileName">Имя файла.</param>
        /// <param name="formatMappings">Расшифровка кодировок форматов ячеек.</param>
        /// <exception cref="System.IO.IOException">Ошибка доступа к файлу.</exception>
        /// <exception cref="System.InvalidCastException">Ошибка распознования файла.</exception>
        private OpenFile(SpreadsheetDocument document, int numberSheet, int needToClose, SetInt closed, string fileName, Dictionary<uint, String> formatMappings)
        {
            this.document = document;
            this.numberSheet = numberSheet;
            this.needToClose = needToClose;
            this.closed = closed;
            this.fileName = fileName;
            this.FormatMappingsFromXlsx = formatMappings;
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

            public static Dictionary<uint, String> BuildFormatMappingsFromXlsx(SpreadsheetDocument document)
            {
                Dictionary<uint, String> formatMappings = new Dictionary<uint, String>()
                {
                    {1, "0"},
                    {2, "0.00"},
                    {3, "#,##0"},
                    {4, "#,##0.00"},
                    {9, "0%"},
                    {10, "0.00%"},
                    {11, "0.00E+00"},
                    {12, "# ?/?"},
                    {13, "# ??/??"},
                    {14, "d/m/yyyy"},
                    {15, "d-mmm-yy"},
                    {16, "d-mmm"},
                    {17, "mmm-yy"},
                    {18, "h:mm tt"},
                    {19, "h:mm:ss tt"},
                    {20, "H:mm"},
                    {21, "H:mm:ss"},
                    {22, "m/d/yyyy H:mm"},
                    {37, "#,##0 ;(#,##0)"},
                    {38, "#,##0 ;[Red](#,##0)"},
                    {39, "#,##0.00;(#,##0.00)"},
                    {40, "#,##0.00;[Red](#,##0.00)"},
                    {45, "mm:ss"},
                    {46, "[h]:mm:ss"},
                    {47, "mmss.0"},
                    {48, "##0.0E+0"},
                    {49, "@"}
                };
                var stylePart = document.WorkbookPart.WorkbookStylesPart;
                var numFormatsParentNodes = stylePart.Stylesheet.ChildElements.OfType<NumberingFormats>();
                foreach (var numFormatParentNode in numFormatsParentNodes)
                {
                    var formatNodes = numFormatParentNode.ChildElements.OfType<NumberingFormat>();
                    foreach (var formatNode in formatNodes)
                        formatMappings[formatNode.NumberFormatId.Value] = formatNode.FormatCode;
                }
                return formatMappings;
            }
        }
    }
}
