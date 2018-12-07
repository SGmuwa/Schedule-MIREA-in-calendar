import org.apache.poi.hssf.usermodel.HSSFSheet;
import org.apache.poi.hssf.usermodel.HSSFWorkbook;
import org.apache.poi.ss.usermodel.Row;
import org.apache.poi.ss.usermodel.Sheet;
import org.apache.poi.ss.usermodel.Workbook;
import org.apache.poi.ss.usermodel.WorkbookFactory;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;

import java.io.File;
import java.io.FileInputStream;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.ArrayList;

import static org.junit.Assert.*;

public class OpenFileTest {

    @Test
    public void testOpenBadFile() throws Exception {
        File test = new File("tests/badExcel.xlsx");
        String message = "";
        for(int i = 0; i < 1000; i++) {
            assertTrue(test.exists());
            ArrayList<? extends ExcelFileInterface> files = null;
            try {
                files = OpenFile.newInstances(test.getAbsolutePath());
                fail();
            } catch (Exception e) {
                message = e.getLocalizedMessage();
                // good
            }
            assertNull(files);
        }
        System.out.println(message);
    }

    @Test
    public void testOpenNormalFile() throws Exception {
        File file = new File("tests/IIT-3k-18_19-osen.xlsx");
        assertTrue(file.exists());
        for(int i = 0; i < 200; i++) {
            System.out.println(i);
            ArrayList<? extends ExcelFileInterface> files =
                    OpenFile.newInstances(file.getAbsolutePath());
            for (ExcelFileInterface aFile : files) {
                aFile.close();
            }
        }
    }

    @Test
    public void testHeapSpace() throws Exception {
        File[] heap = {new File("tests/heap1.xlsx"), new File("tests/heap2.xlsx")};
        String message = "";
        for(int i = 0; i < 10; i++) {
            System.out.println(i);
            for (File aHeap : heap) {
                assertTrue(aHeap.exists());
                ArrayList<? extends ExcelFileInterface> files =
                        OpenFile.newInstances(aHeap.getAbsolutePath());
                for (ExcelFileInterface file : files) {
                    file.close();
                }
            }
        }
        System.out.println(message);
    }

    @Test
    public void testOpenBadAndGoodFile() throws Exception {

    }

    @Test
    public void testOpenXLS() throws Exception {
        Workbook workbook = new HSSFWorkbook();
        Sheet sheet = workbook.createSheet("1");

        // создаем подписи к столбцам (это будет первая строчка в листе Excel файла)
        Row row = sheet.createRow(0);
        row.createCell(0).setCellValue("АА");
        row.createCell(1).setCellValue("БА");

        // создаем подписи к столбцам (это будет вторая строчка в листе Excel файла)
        Row row1 = sheet.createRow(1);
        row1.createCell(0).setCellValue("АБ");
        row1.createCell(1).setCellValue("Груша");

        File testFile = new File("delete1.xls");
        if(testFile.exists())
            assertTrue(testFile.delete());

        try (FileOutputStream out = new FileOutputStream(testFile)) {
            workbook.write(out);
        }

        assertTrue(testFile.exists());
        assertTrue(testFile.delete());

        try (FileOutputStream out = new FileOutputStream(testFile)) {
            workbook.write(out);
        }

        assertTrue(testFile.exists());

        Workbook wb = WorkbookFactory.create(new FileInputStream(testFile));
        wb.close();

        assertTrue(testFile.exists());
        assertTrue(testFile.delete());

        try (FileOutputStream out = new FileOutputStream(testFile)) {
            workbook.write(out);
        }

        assertTrue(testFile.exists());

        OpenFile.newInstances(testFile.getPath()).get(0).close();

        assertTrue(testFile.exists());
        assertTrue(testFile.delete());

        try (FileOutputStream out = new FileOutputStream(testFile)) {
            workbook.write(out);
        }

        assertTrue(testFile.exists());

        for(int i = 0; i < 2; i++) {
            ExcelFileInterface openFile = OpenFile.newInstances(testFile.getPath()).get(0);
            assertEquals("Error 1:1(AA)", "АА", openFile.getCellData(1, 1));
            assertEquals("Error 1:2(AБ)", "АБ", openFile.getCellData(1, 2));
            assertEquals("Error 2:1(БА)", "БА", openFile.getCellData(2, 1));
            assertEquals("Error 2:2(Груша)", "Груша", openFile.getCellData(2, 2));
            try {
                assertEquals("Error -1:-1(null)", "", openFile.getCellData(-1, -1));
                fail();
            } catch (Exception e) {
                // good.
            }
            assertEquals("Error 999:999( )", "", openFile.getCellData(999, 999));
            openFile.close();
        }
        assertTrue("Ошибка при последнем удалении файла", testFile.delete());
    }
}
