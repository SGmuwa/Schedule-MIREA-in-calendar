package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

public class ViewerExcelCouplesException extends Exception {

    public final ExcelFileInterface excelFile;

    ViewerExcelCouplesException(String message, ExcelFileInterface file){
        super(message);
        excelFile = file;
    }
}
