package ru.mirea.xlsical.CouplesDetective.xl;

import java.io.Closeable;
import java.io.IOException;

// Интерфейс по работе с Excel файлами. Экземпляр такого интерфейса должен хранит в себе дескриптор файла.
public interface ExcelFileInterface extends Closeable {

    /**
     * Получение текстовых данных из файла.
     * @param Column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param Row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Текстовые данные в ячейке. Не NULL.
     */
    String getCellData(int Column, int Row) throws IOException;

}
