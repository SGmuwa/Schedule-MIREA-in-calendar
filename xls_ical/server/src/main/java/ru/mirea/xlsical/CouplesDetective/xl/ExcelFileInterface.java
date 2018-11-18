package ru.mirea.xlsical.CouplesDetective.xl;

import java.awt.*;
import java.io.Closeable;
import java.io.IOException;

// Интерфейс по работе с Excel файлами. Экземпляр такого интерфейса должен хранит в себе дескриптор файла.
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
     * Узнаёт фоновый цвет ячейки.
     * @param column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Цвет фона ячейки.
     * @throws IOException Потерян доступ к файлу.
     */
    org.apache.poi.ss.usermodel.Color getBackgroundColor(int column, int row) throws IOException;

}
