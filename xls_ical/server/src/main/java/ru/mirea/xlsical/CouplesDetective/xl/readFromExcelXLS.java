package ru.mirea.xlsical.CouplesDetective.xl;

import org.apache.poi.hssf.usermodel.HSSFCell;
import org.apache.poi.hssf.usermodel.HSSFRow;
import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.poifs.filesystem.POIFSFileSystem;

import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;

class readFromExcelXLS implements ExcelFileInterface {
    private HSSFWorkbook myExcelBook;
    private InputStream fs;


    readFromExcelXLS(String inputFile) throws IOException {
        fs = new FileInputStream(inputFile);
        this.myExcelBook = new HSSFWorkbook(fs);
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
        HSSFSheet myExcelSheet = myExcelBook.getSheet("1");
        HSSFRow row = myExcelSheet.getRow(Row - 1);
        if (Column >= 0 && Row >= 0 && row != null)
            if (row.getCell(Column - 1).getCellType() == HSSFCell.CELL_TYPE_STRING)
                name = row.getCell(Column - 1).getStringCellValue();
            else if(row.getCell(Column - 1).getCellType() == HSSFCell.CELL_TYPE_NUMERIC)
                name = "" + (long)row.getCell(Column - 1).getNumericCellValue();

        return name;
    }

    /**
     * ЧОТО-ДЕЛОЕТ
     * @throws IOException
     */
    @Override
    public void close() throws IOException {
        myExcelBook.close();
        fs.close();
    }
}
