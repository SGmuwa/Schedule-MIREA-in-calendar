package ru.mirea.xlsical.CouplesDetective.xl;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.*;

import java.awt.Color;
import java.io.File;
import java.io.IOException;
import java.util.ArrayList;


public class OpenFile implements ExcelFileInterface {

    private Workbook wb;
    private int numberSheet;

    /**
     * Создаёт экземпляр открытия файла.
     * Для открытия всех листов Excel файла используйте {@link #newInstances(String)}.
     * @param fileName Имя файла, который необходимо открыть.
     * @throws IOException Ошибка доступа к файлу.
     * @throws InvalidFormatException Ошибка распознования файла.
     * @see #newInstances(String)
     */
    private OpenFile(String fileName) throws IOException, InvalidFormatException {
        wb = WorkbookFactory.create(new File(fileName));
    }

    /**
     * Открывает Excel файл вместе со всеми его листами.
     * @param fileName Путь до файла, который необходимо открыть.
     * @return Возвращает список открытых листов.
     * @throws IOException Ошибка доступа к файлу.
     * @throws InvalidFormatException Ошибка распознования .xls или .xlsx файла.
     */
    public static ArrayList<OpenFile> newInstances(String fileName) throws IOException, InvalidFormatException {
        OpenFile first = new OpenFile(fileName);
        int size = first.wb.getNumberOfSheets();
        ArrayList<OpenFile> out = new ArrayList<>(size + 1);
        out.add(first);
        for(int i = 1; i < size; i++) {
            out.add(new OpenFile(fileName));
            out.get(i).numberSheet = i;
        }
        return out;
    }

    /**
     * Получение данных в текстовом виде из указанной ячейки Excel файла.
     * @param column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Текстовые данные в ячейке. Не NULL.
     */
    @Override
    public String getCellData(int column, int row) {
        if (column < 1 || row < 1)
            throw new IllegalArgumentException("column and row must be more 0.");
        Sheet myExcelSheet = wb.getSheetAt(numberSheet);
        org.apache.poi.ss.usermodel.Row rowModel = myExcelSheet.getRow(row - 1);
        if (rowModel == null)
            return "";
        Cell columnModel = rowModel.getCell(column - 1);
        if (columnModel == null)
            return "";
        switch (columnModel.getCellType()) {
            case Cell.CELL_TYPE_STRING:
                return rowModel.getCell(column - 1).getStringCellValue();
            case Cell.CELL_TYPE_NUMERIC:
                return Long.toString((long) rowModel.getCell(column - 1).getNumericCellValue());
            default:
                return "";
        }
    }

    /**
     * Узнаёт фоновый цвет ячейки.
     *
     * @param column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row    Порядковый номер строки. Отсчёт начинается с 1.
     * @return Цвет фона ячейки.
     * @throws IOException Потерян доступ к файлу.
     */
    @Override
    public org.apache.poi.ss.usermodel.Color getBackgroundColor(int column, int row) throws IOException {
        if (column < 1 || row < 1)
            throw new IllegalArgumentException("column and row must be more 0.");
        Sheet myExcelSheet = wb.getSheetAt(numberSheet);
        org.apache.poi.ss.usermodel.Row rowModel = myExcelSheet.getRow(row - 1);
        if (rowModel == null)
            return null;
        Cell columnModel = rowModel.getCell(column - 1);
        if (columnModel == null)
            return null;
        return columnModel.getCellStyle().getFillBackgroundColorColor();
    }

    /**
     * Закрывает Excel файл.
     * @throws IOException Ошибка при закрытии файла.
     */
    @Override
    public void close() throws IOException {
        wb.close();
    }
}
