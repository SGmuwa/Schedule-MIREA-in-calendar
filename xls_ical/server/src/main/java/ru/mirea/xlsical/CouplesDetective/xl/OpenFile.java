package ru.mirea.xlsical.CouplesDetective.xl;

import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.xssf.usermodel.XSSFWorkbook;

import java.io.FileInputStream;
import java.io.IOException;


public class OpenFile implements ExcelFileInterface {

    private ExcelFileInterface myExcelBook;


    public OpenFile(String fileName) throws IOException {

        String type = null;

        for (int i = 0; i < fileName.length(); i++){
            if (fileName.charAt(i) == '.'){
                type = fileName.substring(i + 1);
            }
        }

        if (type != null && type.equals("xlsx")){
            myExcelBook = new readFromExcelXLSX(fileName);
        }else {
            myExcelBook = new readFromExcelXLS(fileName);
        }

    }

    @Override
    public String getCellData(int Column, int Row) throws IOException {
        return myExcelBook.getCellData(Column, Row);
    }

    @Override
    public void close() throws IOException {
        myExcelBook.close();
    }
}
