package ru.mirea.xlsical.CouplesDetective.xl;

import org.apache.poi.xssf.usermodel.XSSFSheet;
import org.apache.poi.xssf.usermodel.XSSFRow;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;
import org.apache.poi.xssf.usermodel.XSSFCell;


import java.io.IOException;

public class readFromExcelXLSX implements ExcelFileInterface {

    private XSSFWorkbook myExcelBook;

    readFromExcelXLSX(String inputFile) throws IOException {
        this.myExcelBook = new XSSFWorkbook(inputFile);
    }

    /**
     * Функция возвращает значение ячейки.
     * @param Column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param Row Порядковый номер строки. Отсчёт начинается с 1.
     * @return значение ячейки типа String.
     */

    @Override
    public String getCellData(int Column, int Row) {
        String name = "";
        if(Column < 0 || Row < 0)
            return name;
        XSSFSheet myExcelSheet = myExcelBook.getSheetAt(0);
        XSSFRow row = myExcelSheet.getRow(Row - 1);
        if(row == null)
            return name;
        XSSFCell cell = row.getCell(Column - 1);
        if (cell != null)
            if (cell.getCellType() == XSSFCell.CELL_TYPE_STRING)
                name = row.getCell(Column - 1).getStringCellValue();
            else if (cell.getCellType() == XSSFCell.CELL_TYPE_NUMERIC)
                name = "" + (long) row.getCell(Column - 1).getNumericCellValue();

        return name;
    }

    /**
     * ЧОТО-ДЕЛОЕТ
     * @throws IOException
     */
    @Override
    public void close() throws IOException {
        myExcelBook.close();
    }
}
