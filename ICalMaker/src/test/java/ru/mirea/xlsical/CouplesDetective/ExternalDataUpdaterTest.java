/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

package ru.mirea.xlsical.CouplesDetective;

import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.io.IOException;
import java.util.Iterator;

import static org.junit.Assert.*;

public class ExternalDataUpdaterTest {

    @Test
    public void run() throws IOException {
        System.out.println("ExternalDataUpdaterTest.java#run: start");
        ExternalDataUpdater edu = GlobalTaskExecutor.externalDataUpdater;
        System.out.println("ExternalDataUpdaterTest.java#run: finish externalDataUpdater");
        assertNull(edu.pathToCache); // Теперь нет пути кэша в данном режиме тестов.
        //assertTrue(edu.pathToCache.canWrite());
        edu.run();
        // assertTrue(edu.isAlive()); Он теперь ничего не скачивает во время тестов.
        try {
            Thread.sleep(100);
        } catch (InterruptedException e) {
            fail("Cancel test.");
        }
        // assertTrue(edu.isAlive()); Он теперь ничего не скачивает во время тестов.
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
        System.out.println("ExternalDataUpdaterTest.java#run: start open files");
        while(files.hasNext()) {
            ExcelFileInterface efi = files.next();
            System.out.println(efi);
            efi.close();
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