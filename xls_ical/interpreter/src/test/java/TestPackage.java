import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;
import org.junit.Test;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import java.time.LocalDate;
import java.time.ZoneId;
import java.time.temporal.ChronoUnit;

import static org.junit.Assert.*;

public class TestPackage {

    @Test
    public void StartTestClient() throws ClassNotFoundException {
        PackageToClient a = PackageToClient.fromByteArray(new PackageToClient(123, "My Testing из изи", 777, "Всё ок!").toByteArray());

        assertNotNull(a);
        assertEquals(123, a.ctx);
        assertEquals("My Testing из изи", a.CalFile);
        assertEquals(777, a.Count);
        assertEquals("Всё ок!", a.Messages);
    }

    @Test
    public void StartTestServer() throws ClassNotFoundException {
        PackageToServer a = PackageToServer.fromByteArray
                (
                        new PackageToServer
                                (
                                        312423,
                                        new Seeker
                                                (
                                                        "1",
                                                        LocalDate.MIN,
                                                        LocalDate.MAX.minus(1, ChronoUnit.DAYS),
                                                        ZoneId.systemDefault()
                                                )
                                ).
                                toByteArray()
                );

        assertNotNull(a);
        assertEquals(new Seeker
                (
                        "1",
                        LocalDate.MIN,
                        LocalDate.MAX.minus(1, ChronoUnit.DAYS),
                        ZoneId.systemDefault()
                ), a.queryCriteria);
    }
}
