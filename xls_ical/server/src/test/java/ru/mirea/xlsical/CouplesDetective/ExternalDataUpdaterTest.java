package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.IOException;
import java.time.*;
import java.time.temporal.ChronoUnit;
import java.util.Iterator;

import static org.junit.Assert.*;

public class ExternalDataUpdaterTest {

    @Test
    public void run() throws IOException, InvalidFormatException {
        ExternalDataUpdater edu = new ExternalDataUpdater();
        assertNotNull(edu.pathToCache);
        assertTrue(edu.pathToCache.canWrite());
        edu.run();
        assertTrue(edu.isAlive());
        try {
            Thread.sleep(100);
        } catch (InterruptedException e) {
            fail("Cancel test.");
        }
        assertTrue(edu.isAlive());
        // in future: assertEquals("Матчин Василий Тимофеевич, старший преподаватель кафедры инструментального и прикладного программного обеспечения.", edu.findTeacher("Матчин В.Т."));
        // 63 файла в бакалавре
        // 22 файла в магистратуре
        // 25 файла в аспирантуре
        // 1 файл в колледже
        // -10 pdf
        // .xls 101 файл.
        Iterator<ExcelFileInterface> files = edu.openTablesFromExternal();
        // Осторожно! Число со временем тестирования может меняться!
        /* <a ref="view-source:https://www.mirea.ru/education/schedule-main/schedule/">
         * сюда</a> и проверитье количество соответствий с ".xls"*/
        //assertEquals(files.size(), 101);
        //for (ExcelFileInterface file : files) {
        //    file.close();
        //}
        assertTrue(files.hasNext());
        assertTrue(files.hasNext());
        edu.interrupt();
        int count = 0;
        while(files.hasNext()) {
            files.next().close();
            if(count % 20 == 0)
                System.out.println(count);
            count++;
        }
        assertTrue(count > 100);
        //assertEquals(101, count);
        try {
            Thread.sleep(20);
        } catch (InterruptedException e) {
            fail("Cancel test.");
        }
        assertFalse(edu.isAlive());
    }
}