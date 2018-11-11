package ru.mirea.xlsical.CouplesDetective.Detective;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

public class DetectiveException extends Exception {

    public final ExcelFileInterface excelFile;

    DetectiveException(String message, ExcelFileInterface file){
        super(message);
        excelFile = file;
    }
}
