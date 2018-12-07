import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.CoupleHistorian;
import ru.mirea.xlsical.CouplesDetective.ExternalDataUpdater;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.*;

import java.io.File;
import java.io.IOException;
import java.time.LocalDate;
import java.time.LocalTime;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.util.ArrayList;

import static org.junit.Assert.*;

public class TaskExecutorTest {

    @Test
    public void pullPollStep() throws InterruptedException {
        TaskExecutor te = new TaskExecutor();
        te.add(new PackageToServer(null, null, null));
        te.step();
        PackageToClient ptc = te.take();

        assertNull(ptc.CalFile);
        assertEquals(0, ptc.Count);
        assertEquals("Ошибка: отстствуют критерии поиска.", ptc.Messages);
    }

    @Test
    public void sendSampleExcel() throws InterruptedException {
        ArrayList<File> excels = new ArrayList<File>();
        excels.add(new File("tests\\IIT-3k-18_19-osen (2).xlsx"));
        TaskExecutor a = new TaskExecutor(new CoupleHistorian(new ExternalDataUpdater(
                excels,
                new ArrayList<>()
        ), new DetectiveDate(null), false));
        a.add(new PackageToServer(null,
                new PercentReady(), null,
                new Seeker(
                        "ИКБО-04-16",
                        LocalDate.of(2018, 9, 1),
                        LocalDate.of(2018, 9, 3),
                        ZoneId.of("Europe/Minsk")
                )
        ));

        a.step();
        PackageToClient b = a.take();
        assertNotNull(b.CalFile);
        System.out.println(b.CalFile);
        assertEquals(3, b.Count);
    }

    @Test
    public void sendSampleExcelAllSem() throws InterruptedException {
        ArrayList<File> excels = new ArrayList<File>();
        excels.add(new File("tests\\IIT-3k-18_19-osen (2).xlsx"));

        ZonedDateTime now = ZonedDateTime.of(
                LocalDate.of(2018, 9, 1),
                LocalTime.MIN,
                ZoneId.of("Europe/Minsk")
        );

        CoupleHistorian historian = new CoupleHistorian(new ExternalDataUpdater(
                excels,
                new ArrayList<>()
        ), new DetectiveDate(), false, now);

        TaskExecutor a = new TaskExecutor(historian);
        a.add(new PackageToServer(null,
                new PercentReady(), null,
                new Seeker(
                        "ИКБО-04-16",
                        LocalDate.of(2018, 9, 1),
                        LocalDate.of(2018, 12, 31),
                        ZoneId.of("Europe/Minsk")
                )
        ));

        a.step();
        PackageToClient b = a.take();
        System.out.println(b.CalFile);
        assertNotNull(b.CalFile);
        assertEquals(232, b.Count);
    }

    @Test
    public void sendExcelAllSem() throws InterruptedException, IOException {

        ZonedDateTime now = ZonedDateTime.of(
                LocalDate.of(2018, 9, 1),
                LocalTime.MIN,
                ZoneId.of("Europe/Minsk")
        );

        CoupleHistorian historian = new CoupleHistorian(new ExternalDataUpdater(), new DetectiveDate(), false, now);

        TaskExecutor a = new TaskExecutor(historian);
        a.add(new PackageToServer(null,
                new PercentReady(), null,
                new Seeker(
                        "ИКБО-04-16",
                        LocalDate.of(2018, 9, 1),
                        LocalDate.of(2018, 12, 31),
                        ZoneId.of("Europe/Minsk")
                )
        ));

        a.step();
        PackageToClient b = a.take();
        System.out.println(b.CalFile);
        assertNotNull(b.CalFile);
        assertEquals(232, b.Count);
    }
}
