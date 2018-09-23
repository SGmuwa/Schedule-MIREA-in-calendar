import java.time.LocalDate;
import java.time.ZoneId;

import org.junit.Test;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import static org.junit.Assert.*;

public class TestSeeker {

    @Test
    public void startTestSeeker() {

        System.out.println("Test Seeker start.");

        Seeker test = new Seeker("name",
                SeekerType.Teacher,
                LocalDate.of(2000, 5, 5),
                LocalDate.of(2000, 5, 10),
                ZoneId.systemDefault(),
                "Москва, проспект Вернадского, 78, РТУ МИРЭА", 1);

        assertEquals("name", test.nameOfSeeker);
        assertEquals(SeekerType.Teacher, test.seekerType);
        assertEquals(LocalDate.of(2000, 5, 5), test.dateStart);
        assertEquals(LocalDate.of(2000, 5, 10), test.dateFinish);
        assertEquals(ZoneId.systemDefault(), test.timezoneStart);
        assertEquals("Москва, проспект Вернадского, 78, РТУ МИРЭА", test.defaultAddress);
        assertEquals(1, test.startWeek);

        PackageToClient cl = new PackageToClient(new byte[]{0, 0}, 0, "Всё ок");
        assertArrayEquals(new byte[]{0, 0}, cl.CalFile);
        assertEquals(0, cl.Count);
        assertEquals("Всё ок", cl.Messages);

        PackageToServer sv = new PackageToServer(new byte[][] {{0, 0}}, test);

        assertEquals(test, sv.queryCriteria);
        assertArrayEquals(new byte[][] {{0, 0}}, sv.excelsFiles);
    }


}