import ru.mirea.xlsical.CouplesDetective.Couple;
import ru.mirea.xlsical.CouplesDetective.Detective;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.DetectiveException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import static org.junit.Assert.*;
import static ru.mirea.xlsical.CouplesDetective.Detective.GetTimes;

import java.io.IOException;
import java.time.DayOfWeek;
import java.time.LocalDate;
import java.time.ZoneId;
import java.util.*;
import java.awt.*;
import java.util.List;

public class DetectiveTest {
    @Test
    public void GetMinutesFromTimeStringTest(){
        assertEquals(728, Detective.GetMinutesFromTimeString("12:08"));
        assertEquals(13, Detective.GetMinutesFromTimeString("00-13"));
        assertEquals(78, Detective.GetMinutesFromTimeString("01-18"));
        assertEquals(860, Detective.GetMinutesFromTimeString("14:20"));
        assertEquals(1230, Detective.GetMinutesFromTimeString("20-30"));
        assertEquals(0, Detective.GetMinutesFromTimeString("00:00"));
    }

    @Test
    public void IsStringNumberTest(){
        assertTrue(Detective.IsStringNumber("8"));
        assertTrue(Detective.IsStringNumber(""));
        assertTrue(Detective.IsStringNumber("85"));
        assertTrue(Detective.IsStringNumber("0"));
        assertTrue(Detective.IsStringNumber("-3"));
        assertFalse(Detective.IsStringNumber("f"));
        assertFalse(Detective.IsStringNumber("."));
    }

    @Test
    public void IsEqualsInListTest(){
        String a = "a";
        String b = "b";
        ArrayList <String> list = new ArrayList <String>();
        list.add(a);
        assertEquals(true, Detective.IsEqualsInList(list, a));;
        assertEquals(false, Detective.IsEqualsInList(list, b));;
    }

    @Test
    public void startAnInvestigationTest(){
        ExcelFileInterface file = null;
        try {
            file = new OpenFile("tests/IIT-3k-18_19-osen.xlsx");
        } catch (IOException e)
        {
            System.out.println(e.getLocalizedMessage());
        }
        assertNotNull(file);
        try {
            file.close();
        } catch (IOException e) {
            e.printStackTrace();
        }
    }


    @Test
    public void GetCouplesFromDayTest(){
        List<Point> list = new ArrayList<>();
        ExcelFileInterface file = null;
        Collection<? extends Couple> col;
        int[] times = {540,630,640,730,780,870,880,970,980,1070,1080,1170};

        try{
            file = new OpenFile("tests/test-01.xlsx");
        }catch(IOException e){
                System.out.println(e.getLocalizedMessage());
        }

        Seeker seeker = new Seeker("ИКБО-04-16", SeekerType.StudyGroup, LocalDate.of(2018,9,1), LocalDate.of(2018, 10,1), ZoneId.of("UTC+3"), "пр-т Вернадского, 78", 1);
        try {
            col = Detective.GetCouplesFromDay(6,3, "ИКБО-04-16",DayOfWeek.of(1), seeker, list, times, "пр-т Вернадского, 78", file);
        } catch (IOException e) {
            e.printStackTrace();
        }

        //return col; Should be void.

    }

    @Test
    public void openFirstXls() {

        ExcelFileInterface file = null;
        try {
            file = new OpenFile("tests/IIT-3k-18_19-osen.xlsx");
        } catch (IOException e)
        {
            System.out.println(e.getLocalizedMessage());
        }
        assertNotNull(file);

        try {
            System.out.println(file.getCellData(1, 1));
            System.out.println(file.getCellData(2, 1));
            System.out.println(file.getCellData(1, 2));
            System.out.println(file.getCellData(2, 2));

            file.close();

        } catch (IOException e) {
            System.out.println(e.getLocalizedMessage());
        }
    }

    @Test
    public void GetTimesTest(){
        Point point = new Point(5,3);
        ExcelFileInterface file = null;
        int [] mas = {0,0};
        try {
            file = new OpenFile("tests/IIT-3k-18_19-osen.xlsx");
        }catch (IOException e){
            System.out.println(e.getLocalizedMessage());
        }
        assertNotNull(file);

        try {
            mas = GetTimes(point, file);
        }catch (DetectiveException | IOException e){
            System.out.println(e.getLocalizedMessage());
        }

    }
}
