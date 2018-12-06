package ru.mirea.xlsical.CouplesDetective.xl;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.apache.poi.openxml4j.util.ZipSecureFile;
import org.apache.poi.ss.usermodel.*;

import java.io.File;
import java.io.IOException;
import java.util.ArrayList;

/**
 * Класс реализует интерфейс {@link ExcelFileInterface}.
 * Он который является переходным между
 * {@link ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective Detective}
 * и {@link Workbook}.
 * @since 13.05.2018
 * @version 18.11.2018
 * @author <a href="https://github.com/gosharas/">gosharas</a>, <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 * @see OpenFile#newInstances(String)
 */
public class OpenFile implements ExcelFileInterface {

    static {
        ZipSecureFile.setMinInflateRatio(0.008);
    }

    @Override
    public String toString() {
        return "OpenFile{" +
                "wb=" + wb +
                ", numberSheet=" + numberSheet +
                '}';
    }

    private int needToClose;

    private SetInt closed;

    /**
     * Открывает Excel файл вместе со всеми его листами.
     * @param fileName Путь до файла, который необходимо открыть.
     * @return Возвращает список открытых листов.
     * @throws IOException Ошибка доступа к файлу.
     * @throws InvalidFormatException Ошибка распознования .xls или .xlsx файла.
     */
    public static ArrayList<OpenFile> newInstances(String fileName) throws IOException, InvalidFormatException {
        SetInt setInt = new SetInt();
        OpenFile first = new OpenFile(fileName, 0);
        int size = first.wb.getNumberOfSheets();
        ArrayList<OpenFile> out = new ArrayList<>(size);
        out.add(first);
        first.needToClose = size;
        first.closed = setInt;

        for(int i = 1; i < size; i++) {
            out.add(new OpenFile(first.wb, i, size, setInt));
        }
        return out;
    }

    /**
     * Получение данных в текстовом виде из указанной ячейки Excel файла.
     * @param column Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row Порядковый номер строки. Отсчёт начинается с 1.
     * @return Текстовые данные в ячейке. Не NULL.
     * @throws IOException Потерян доступ к файлу.
     */
    @Override
    public String getCellData(int column, int row) throws IOException {
        Cell cell = getCell(column, row);
        if (cell == null)
            return "";
        switch (cell.getCellType()) {
            case STRING:
                return cell.getStringCellValue();
            case NUMERIC:
                return Long.toString((long) cell.getNumericCellValue());
            default:
                return "";
        }
    }

    /**
     * Узнаёт фоновый цвет двух ячеек и отвечает на вопрос, одинаковый ли у них фоновый цвет.
     * @param column1 Первая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row1 Первая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.
     * @param column2 Вторая сравниваемая ячейка. Порядковый номер столбца. Отсчёт начинается с 1.
     * @param row2 Вторая сравниваемая ячейка. Порядковый номер строки. Отсчёт начинается с 1.
     * @return {@code True}, если цвета совпадают. Иначе - {@code false}.
     * @throws IOException Потерян доступ к файлу.
     */
    @Override
    public boolean isBackgroundColorsEquals(int column1, int row1, int column2, int row2) throws IOException {
        Cell cellA = getCell(column1, row1);
        Cell cellB = getCell(column2, row2);
        if (cellA == null || cellB == null)
            return false;
        // need test!
        return cellA.getCellStyle().getFillBackgroundColorColor().equals(cellB.getCellStyle().getFillBackgroundColorColor());
        //return cellA.getCellStyle().getFillBackgroundColor() == cellB.getCellStyle().getFillBackgroundColor(); // XSSF only work.
    }

    private boolean isOpen = true;

    /**
     * Закрывает Excel файл.
     * @throws IOException Ошибка при закрытии файла.
     */
    @Override
    public synchronized void close() throws IOException {
        if(isOpen && closed.get() < needToClose) {
            isOpen = false;
            closed.add();
            if(closed.get() == needToClose) {
                wb.close();
            }
        }
    }

    private final Workbook wb;
    private final int numberSheet;

    /**
     * Создаёт экземпляр открытия файла.
     * Для открытия всех листов Excel файла используйте {@link #newInstances(String)}.
     * @param fileName Имя файла, который необходимо открыть.
     * @param numberSheet Номер страницы книги Excel.
     * @throws IOException Ошибка доступа к файлу.
     * @throws InvalidFormatException Ошибка распознования файла.
     * @see #newInstances(String)
     */
    private OpenFile(String fileName, int numberSheet) throws IOException, InvalidFormatException {
        wb = WorkbookFactory.create(new File(fileName));
        this.numberSheet = numberSheet;
    }

    /**
     * Создаёт экземпляр открытия файла.
     * Для открытия всех листов Excel файла используйте {@link #newInstances(String)}.
     * @param workbook Открытая книга.
     * @param numberSheet Номер страницы книги Excel.
     * @throws IOException Ошибка доступа к файлу.
     * @throws InvalidFormatException Ошибка распознования файла.
     * @see #newInstances(String)
     */
    private OpenFile(Workbook workbook, int numberSheet, int needToClose, SetInt closed) throws IOException, InvalidFormatException {
        this.needToClose = needToClose;
        this.closed = closed;
        this.wb = workbook;
        this.numberSheet = numberSheet;
    }

    /**
     * Получение ячейки по номеру колонки и строки.
     * @param column Порядковый номер колонки.
     * @param row Порядковый номер строки.
     * @return Ячейка по данному адресу.
     */
    private Cell getCell(int column, int row) {
        if (column < 1 || row < 1)
            throw new IllegalArgumentException("column and row must be more 0.");
        Sheet myExcelSheet = wb.getSheetAt(numberSheet);
        org.apache.poi.ss.usermodel.Row rowModel = myExcelSheet.getRow(row - 1);
        if (rowModel == null)
            return null;
        Cell cell = rowModel.getCell(column - 1);
        if (cell == null)
            return null;
        return cell;
    }
}


class SetInt {
    SetInt(int value) {
        this.value = value;
    }

    SetInt() {
        this.value = 0;
    }

    private int value;

    public int get() {
        return value;
    }

    public void set(int value) {
        this.value = value;
    }

    public void add(int value) {
        this.value += value;
    }

    public void add() {
        this.value++;
    }
}