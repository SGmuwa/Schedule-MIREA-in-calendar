package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.Closeable;
import java.io.IOException;
import java.time.*;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.Iterator;

import static org.junit.Assert.*;

public class ExternalDataUpdaterTest {

    /**
     * Функция разработана для математического вычисления,
     * как надо реализовать расстановку начала и конца семестра.
     */
    @Test
    public void calculateDays() {

        assertEquals(2,
                getCounts6Days(ZonedDateTime.of(
                        LocalDate.of(2000, 1, 1),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                        ),
                        ZonedDateTime.of(
                                LocalDate.of(2000, 1, 2),
                                LocalTime.of(0, 0, 0),
                                ZoneId.systemDefault()
                        )
                )
        );

        assertEquals(6,
                getCounts6Days(ZonedDateTime.of(
                        LocalDate.of(2000, 1, 1),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                        ),
                        ZonedDateTime.of(
                                LocalDate.of(2000, 1, 7),
                                LocalTime.of(0, 0, 0),
                                ZoneId.systemDefault()
                        )
                )
        );

        System.out.println(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2015, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2015, 2, 9),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )));

        System.out.println(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2017, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2017, 2, 6),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )));
        System.out.println(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2018, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2018, 2, 9),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )));
        System.out.println(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2019, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2019, 2, 11),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )));
    }

    private int getCounts6Days(ZonedDateTime current, ZonedDateTime target) {
        int out = 1;
        if(current.compareTo(target) >= 0)
            throw new IllegalArgumentException();
        while(current.compareTo(target) < 0) {
            if(current.getDayOfWeek() == DayOfWeek.SUNDAY)
                current = current.plus(2, ChronoUnit.DAYS);
            else
                current = current.plus(1, ChronoUnit.DAYS);
            out++;
        }
        return out;
    }

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
        edu.interrupt();
        try {
            Thread.sleep(20);
        } catch (InterruptedException e) {
            fail("Cancel test.");
        }
        assertFalse(edu.isAlive());
    }
}