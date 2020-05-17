package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveSemester;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import static org.junit.Assert.*;

import java.io.IOException;
import java.time.DayOfWeek;
import java.time.LocalDate;
import java.time.ZoneId;
import java.util.*;
import java.awt.*;
import java.util.List;

public class DetectiveSemesterTest {
    @Test
    public void GetMinutesFromTimeStringTest() throws DetectiveException {
        DetectiveSemester ds = new DetectiveSemester(null, new DetectiveDate());
        assertEquals(728, ds.GetMinutesFromTimeString("12:08"));
        assertEquals(13, ds.GetMinutesFromTimeString("00-13"));
        assertEquals(78, ds.GetMinutesFromTimeString("01-18"));
        assertEquals(860, ds.GetMinutesFromTimeString("14:20"));
        assertEquals(1230, ds.GetMinutesFromTimeString("20-30"));
        assertEquals(0, ds.GetMinutesFromTimeString("00:00"));
    }

    @Test
    public void IsStringNumberTest(){
        assertTrue(DetectiveSemester.IsStringNumber("8"));
        assertFalse(DetectiveSemester.IsStringNumber(""));
        assertTrue(DetectiveSemester.IsStringNumber("85"));
        assertTrue(DetectiveSemester.IsStringNumber("0"));
        assertTrue(DetectiveSemester.IsStringNumber("-3"));
        assertFalse(DetectiveSemester.IsStringNumber("f"));
        assertFalse(DetectiveSemester.IsStringNumber("."));
    }

    @Test
    public void IsEqualsInListTest(){
        String a = "a";
        String b = "b";
        ArrayList <String> list = new ArrayList <String>();
        list.add(a);
        assertTrue(DetectiveSemester.IsEqualsInList(list, a));
        assertFalse(DetectiveSemester.IsEqualsInList(list, b));
    }

    @Test
    public void startAnInvestigationTest() throws IOException, InvalidFormatException {
        Collection<? extends ExcelFileInterface> files = null;

        files = OpenFile.newInstances("tests/IIT-3k-18_19-osen.xlsx");

        assertNotNull(files);
        assertEquals(1, files.size());
        for(ExcelFileInterface file : files)
            file.close();
    }


    @Test
    public void GetCouplesFromDayTest() throws IOException, InvalidFormatException {
        List<Point> list = new ArrayList<>();
        Collection<? extends ExcelFileInterface> files;
        Collection<? extends DetectiveSemester.CoupleInExcel> col;
        int[] times = {540,630,640,730,780,870,880,970,980,1070,1080,1170};

        files = OpenFile.newInstances("tests/test-01.xlsx");
        assertEquals(1, files.size());
        ExcelFileInterface file = files.iterator().next();
        Seeker seeker = new Seeker("ИКБО-04-16", LocalDate.of(2018,9,1), LocalDate.of(2018, 10,1), ZoneId.of("UTC+3"));

        col = new DetectiveSemester(files.iterator().next(), new DetectiveDate()).GetCouplesFromDay(
                6,
                3,
                "ИКБО-04-16",
                DayOfWeek.of(1),
                list,
                times,
                "пр-т Вернадского, 78"
        );

        for(Object couple : col)
            System.out.println(couple.toString());

        file.close();
    }

    @Test
    public void openFirstXls() throws IOException, InvalidFormatException {

        List<? extends ExcelFileInterface> files = null;
        files = OpenFile.newInstances("tests/IIT-3k-18_19-osen.xlsx");
        assertNotNull(files);
        assertEquals(1, files.size());
        ExcelFileInterface file = files.get(0);

        System.out.println(file.getCellData(1, 1));
        System.out.println(file.getCellData(2, 1));
        System.out.println(file.getCellData(1, 2));
        System.out.println(file.getCellData(2, 2));

        file.close();
    }
/*
    @Test
    public void GetTimesTest() throws IOException, InvalidFormatException, DetectiveException {
        Point point = new Point(5,3);
        ArrayList<? extends ExcelFileInterface> files;
        int [] mas = {0,0};

        files = OpenFile.newInstances("tests/IIT-3k-18_19-osen.xlsx");
        assertNotNull(files);
        assertEquals(1, files.size());
        ExcelFileInterface file = files.get(0);

        mas = GetTimes(point, file);

    }*/
}
