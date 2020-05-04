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

namespace ru.mirea.xlsical.CouplesDetective.xl
{
    /// <summary>
    /// Интерфейс по работе с Excel файлами. Экземпляр такого интерфейса должен хранит в себе дескриптор файла.
    /// </summary>
    public interface ExcelFileInterface : IDisposable
    {
        /// <summary>
        /// Получение текстовых данных из файла.
        /// </summary>
        /// <param name="column">Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row">Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <returns>Текстовые данные в ячейке. Не NULL.</returns>
        /// <exception cref="System.IO.IOException">Потерян доступ к файлу.</exception>
        string GetCellData(int column, int row);

        /// <summary>
        /// Узнаёт фоновый цвет двух ячеек и отвечает на вопрос, одинаковый ли у них фоновый цвет.
        /// </summary>
        /// <param name="column1">Первая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row1">Первая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <param name="column2">Вторая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.</param>
        /// <param name="row2">Вторая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.</param>
        /// <returns><code>True</code>, если цвета совпадают. Иначе — <code>False</code>.</returns>
        /// <exception cref="System.IO.IOException">Потерян доступ к файлу.</exception>
        bool IsBackgroundColorsEquals(int column1, int row1, int column2, int row2);
    }
}
