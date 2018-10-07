import org.junit.Test;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

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
        assertEquals("Ошибка внутри обработчика. Не было передано множество excel файлов.", ptc.Messages);
    }
}
