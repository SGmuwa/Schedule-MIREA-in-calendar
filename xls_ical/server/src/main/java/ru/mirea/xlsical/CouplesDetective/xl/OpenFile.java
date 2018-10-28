package ru.mirea.xlsical.CouplesDetective.xl;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.ss.usermodel.*;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;


public class OpenFile implements ExcelFileInterface {

    private Workbook wb;
    private int numberSheet;

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
    public static ArrayList<OpenFile> newInstance(String fileName) throws IOException, InvalidFormatException {
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
     * @param Column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param Row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Текстовые данные в ячейке. Не NULL.
     */
    @Override
    public String getCellData(int Column, int Row) {
        String name = "";
        if(Column < 0 || Row < 0)
            return name;
        Sheet myExcelSheet = wb.getSheetAt(numberSheet);
        org.apache.poi.ss.usermodel.Row row = myExcelSheet.getRow(Row - 1);
        if(row == null)
            return name;
        Cell cell = row.getCell(Column - 1);
        if (cell != null)
            if (cell.getCellType() == Cell.CELL_TYPE_STRING)
                name = row.getCell(Column - 1).getStringCellValue();
            else if (cell.getCellType() == Cell.CELL_TYPE_NUMERIC)
                name = "" + (long) row.getCell(Column - 1).getNumericCellValue();

        return name;
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
