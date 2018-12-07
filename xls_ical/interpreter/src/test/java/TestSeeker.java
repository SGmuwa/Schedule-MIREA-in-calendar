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

        Seeker test = new Seeker(
                "name",
                LocalDate.of(2000, 5, 5),
                LocalDate.of(2000, 5, 10),
                ZoneId.systemDefault()
        );

        assertEquals("name", test.nameOfSeeker);
        assertEquals(LocalDate.of(2000, 5, 5), test.dateStart.toLocalDate());
        assertEquals(LocalDate.of(2000, 5, 10), test.dateFinish.toLocalDate());
        assertEquals(ZoneId.systemDefault(), test.dateStart.getZone());

        PackageToClient cl = new PackageToClient(0, "", 0, "Всё ок");
        assertEquals("", cl.CalFile);
        assertEquals(0, cl.Count);
        assertEquals("Всё ок", cl.Messages);

        PackageToServer sv = new PackageToServer(0, new String[]{""}, test);

        assertEquals(test, sv.queryCriteria);
        assertArrayEquals(new String[]{""}, sv.excelsFiles);
    }


}