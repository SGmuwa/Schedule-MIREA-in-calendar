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
package ru.mirea.xlsical.CouplesDetective.xl;

import java.io.Closeable;
import java.io.IOException;

/**
 * Интерфейс по работе с Excel файлами. Экземпляр такого интерфейса должен хранит в себе дескриптор файла.
 */
public interface ExcelFileInterface extends Closeable {

    /**
     * Получение текстовых данных из файла.
     * @param column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Текстовые данные в ячейке. Не NULL.
     * @throws IOException Потерян доступ к файлу.
     */
    String getCellData(int column, int row) throws IOException;

    /**
     * Узнаёт фоновый цвет двух ячеек и отвечает на вопрос, одинаковый ли у них фоновый цвет.
     * @param column1 Первая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row1 Первая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.
     * @param column2 Вторая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row2 Вторая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.
     * @return {@code True}, если цвета совпадают. Иначе - {@code false}.
     * @throws IOException Потерян доступ к файлу.
     */
    boolean isBackgroundColorsEquals(int column1, int row1, int column2, int row2) throws IOException;

}
