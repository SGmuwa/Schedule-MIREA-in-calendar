package ru.mirea.xlsical.CouplesDetective;

import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.interpreter.PercentReady;

import java.io.IOException;
import java.time.ZonedDateTime;
import java.util.LinkedList;

import static org.junit.Assert.assertEquals;

public class CoupleHistorianTest
        extends CoupleHistorian { // Для использования protected


    public CoupleHistorianTest() throws IOException {
        super(new ExternalDataUpdater(false), new DetectiveDate(), false, ZonedDateTime.now(), new PercentReady());
    }

    @Test
    public void testBigCache() throws IOException {
        CoupleHistorian a1 = GlobalTaskExecutor.coupleHistorian;
        CoupleHistorian a2 = GlobalTaskExecutor.coupleHistorian;
        LinkedList<CoupleInCalendar> c11 = a1.getCache();
        long f1 = GlobalTaskExecutor.fileForCacheCoupleHistorian.length();
        a1.saveCache();
        a1.loadCache();
        long f2 = GlobalTaskExecutor.fileForCacheCoupleHistorian.length();
        LinkedList<CoupleInCalendar> c12 = a1.getCache();
        LinkedList<CoupleInCalendar> c21 = a2.getCache();
        a2.saveCache();
        a2.loadCache();
        long f3 = GlobalTaskExecutor.fileForCacheCoupleHistorian.length();
        LinkedList<CoupleInCalendar> c22 = a2.getCache();
        assertEquals(c11, c12);
        assertEquals(c12, c21);
        assertEquals(c21, c22);
        assertEquals(c11, c22);
        assertEquals(f1, f2);
        assertEquals(f1, f3);
    }

}