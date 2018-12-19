package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import org.junit.Test;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.*;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.*;

import java.io.File;
import java.io.IOException;
import java.time.LocalDate;
import java.time.LocalTime;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.util.ArrayList;
import java.util.List;

import static org.junit.Assert.*;

public class TaskExecutorTest {

    @Test
    public void pullPollStep() throws InterruptedException {
        TaskExecutor te = GlobalTaskExecutor.taskExecutor;
        te.add(new PackageToServer(null, null));
        te.step();
        PackageToClient ptc = te.take();

        assertNull(ptc.CalFile);
        assertEquals(0, ptc.Count);
        assertEquals("Ошибка: отстствуют критерии поиска.", ptc.Messages);
    }

    @Test
    public void sendSampleExcel() throws InterruptedException {
        TaskExecutor a = GlobalTaskExecutor.taskExecutor;
        a.add(new PackageToServer(null,
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
    public void sendSampleExcelAllSem() throws InterruptedException, IOException {
        TaskExecutor a = new TaskExecutor(GlobalTaskExecutor.coupleHistorian);
        a.add(new PackageToServer(null,
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
        // В этом тесте надо уточнить, чтобы код думал, что сейчас 1 сентября 2018 года,
        // чтобы построил расписание на осенний семестр 2018 года.
        CoupleHistorian historian = GlobalTaskExecutor.coupleHistorian;

        TaskExecutor a = new TaskExecutor(historian);
        a.add(new PackageToServer(null,
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
    public void sendExcelManual() throws InterruptedException, IOException, InvalidFormatException, DetectiveException {
        // В этом тесте надо уточнить, чтобы код думал, что сейчас 1 сентября 2018 года,
        // чтобы построил расписание на осенний семестр 2018 года.
        List<? extends ExcelFileInterface> files = OpenFile.newInstances("tests/Zach_IIT-3k-18_19-osen.xlsx");
        IDetective det = new DetectiveLastWeekS(files.get(0), new DetectiveDate());

        List<CoupleInCalendar> couples = det.startAnInvestigation(
                ZonedDateTime.of(
                        LocalDate.of(2018, 12, 24),
                        LocalTime.of(0, 0, 0),
                        ZoneId.of("Europe/Moscow")
                ),
                ZonedDateTime.of(
                        LocalDate.of(2018, 12, 30),
                        LocalTime.of(0, 0, 0),
                        ZoneId.of("Europe/Moscow")
                )
        );
        couples.removeIf((coup) -> !coup.nameOfGroup.equals("ИКБО-02-16"));
        String str = ExportCouplesToICal.start(couples, new PercentReady());
        System.out.println(str);
    }
}
