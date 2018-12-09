package ru.mirea.xlsical.CouplesDetective;

import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.interpreter.PercentReady;

import java.io.File;
import java.io.IOException;
import java.time.LocalDate;
import java.time.LocalTime;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.util.ArrayList;
import java.util.LinkedList;

import static org.junit.Assert.*;

public class CoupleHistorianTest extends CoupleHistorian {

    private final File pathToExcelFile = new File("tests/IIT-3k-18_19-osen.xlsx");
    private final File pathToDBFile = new File("tests/coupleHistorianTest.dat");
    private final File pathToBigDBFile = new File("tests/coupleHistorianTest_big.dat");
    private static final PercentReady ready2_4 = new PercentReady(GlobalPercentReady.percentReady, 1f/4f);


    public CoupleHistorianTest() throws IOException {
        super(new ExternalDataUpdater(false), new DetectiveDate(), false, ZonedDateTime.now(), new PercentReady(ready2_4, 1f/3f));
    }

    private CoupleHistorian newInstance(ZonedDateTime now, PercentReady pr) throws IOException {
        ArrayList<File> excels = new ArrayList<>();
        excels.add(pathToExcelFile);
        return new CoupleHistorian(
                new ExternalDataUpdater(excels, new ArrayList<>()),
                new DetectiveDate(),
                true,
                now,
                pr,
                pathToDBFile);
    }

    @Test
    public void testCache() throws IOException {
        PercentReady pr = new PercentReady(ready2_4, 1f/3f);
        ZonedDateTime TimeForInstance = ZonedDateTime.of(
                LocalDate.of(2018, 9, 1),
                LocalTime.MIN,
                ZoneId.of("Europe/Minsk")
        );
        ZonedDateTime TimeForTest = ZonedDateTime.of(
                LocalDate.of(2018, 11, 1),
                LocalTime.MIN,
                ZoneId.of("Europe/Minsk")
        );

        CoupleHistorian a1 = newInstance(TimeForInstance, new PercentReady(pr, 0.5f));
        CoupleHistorian a2 = newInstance(TimeForTest, new PercentReady(pr, 0.5f));
        assertArrayEquals(a1.getCache().toArray(new CoupleInCalendar[0]), a2.getCache().toArray(new CoupleInCalendar[0]));
    }

    private CoupleHistorian newBigInstance(ZonedDateTime now, PercentReady pr) throws IOException {
        return new CoupleHistorian(pr, pathToBigDBFile);
    }

    @Test
    public void testBigCache() throws IOException {
        PercentReady pr = new PercentReady(ready2_4, 1f/3f);
        PercentReady prEquals = new PercentReady(pr, 0.2f);
        ZonedDateTime TimeForInstance = ZonedDateTime.of(
                LocalDate.of(2018, 9, 1),
                LocalTime.MIN,
                ZoneId.of("Europe/Minsk")
        );
        ZonedDateTime TimeForTest = ZonedDateTime.of(
                LocalDate.of(2018, 11, 1),
                LocalTime.MIN,
                ZoneId.of("Europe/Minsk")
        );

        CoupleHistorian a1 = newBigInstance(TimeForInstance, new PercentReady(pr, 0.4f));
        CoupleHistorian a2 = newBigInstance(TimeForTest, new PercentReady(pr, 0.4f));
        LinkedList<CoupleInCalendar> c11 = a1.getCache();
        a1.saveCache();
        a1.loadCache();
        LinkedList<CoupleInCalendar> c12 = a1.getCache();
        LinkedList<CoupleInCalendar> c21 = a2.getCache();
        a2.saveCache();
        a2.loadCache();
        LinkedList<CoupleInCalendar> c22 = a2.getCache();
        assertEquals(c11, c12);
        prEquals.setReady(0.25f);
        assertEquals(c12, c21);
        prEquals.setReady(0.5f);
        assertEquals(c21, c22);
        prEquals.setReady(0.75f);
        assertEquals(c11, c22);
        prEquals.setReady(1f);
    }

}