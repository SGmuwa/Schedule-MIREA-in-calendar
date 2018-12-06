import org.junit.Test;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import java.time.LocalDate;
import java.time.ZoneId;

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
        TaskExecutor a = new TaskExecutor();
        a.add(new PackageToServer(null,
                new String[]{"tests\\IIT-3k-18_19-osen (2).xlsx"},
                new Seeker(
                        "ИКБО-04-16",
                        SeekerType.StudyGroup,
                        LocalDate.of(2018, 9, 1),
                        LocalDate.of(2018, 9, 1),
                        ZoneId.of("Europe/Minsk"),
                        "Москва, Проспект Вернадского, 78",
                        0
                )
        ));

        a.step();
        PackageToClient b = a.take();
        System.out.println(b.CalFile);
        assertNotNull(b.CalFile);
        assertEquals(2, b.Count);
    }

    @Test
    public void sendSampleExcelAllSem() throws InterruptedException {
        TaskExecutor a = new TaskExecutor();
        a.add(new PackageToServer(null,
                new String[]{"tests\\IIT-3k-18_19-osen (2).xlsx"},
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
        assertEquals(222, b.Count);
    }
}
