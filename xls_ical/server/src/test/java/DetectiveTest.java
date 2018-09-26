import ru.mirea.xlsical.CouplesDetective.Detective;
import org.junit.Assert;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;

import static org.junit.Assert.*;

import java.io.IOException;
import java.util.*;
import java.awt.*;

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
        assertEquals(true, Detective.IsStringNumber("8"));
        assertEquals(true,  Detective.IsStringNumber(""));
        assertEquals(true, Detective.IsStringNumber("85"));
        assertEquals(true,  Detective.IsStringNumber("0"));
        assertEquals(false,  Detective.IsStringNumber("-3"));
        assertEquals(false, Detective.IsStringNumber("f"));
        assertEquals(false,  Detective.IsStringNumber("."));
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
    public void openFirstXls() {

        ExcelFileInterface file = null;
        try {
            file = new OpenFile("xls-test\\IIT-3k-18_19-osen.xlsx");
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
}
