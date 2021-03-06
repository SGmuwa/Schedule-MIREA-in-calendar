package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

public class DetectiveException extends Exception {

    public final ExcelFileInterface excelFile;

    DetectiveException(String message, ExcelFileInterface file){
        super(message);
        excelFile = file;
    }

    @Override
    public String getLocalizedMessage() {
        return super.getLocalizedMessage() + " file: " + excelFile.toString();
    }
}
