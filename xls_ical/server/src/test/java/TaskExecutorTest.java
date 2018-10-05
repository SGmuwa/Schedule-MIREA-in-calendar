import org.junit.Test;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

public class TaskExecutorTest {

    @Test
    public void pullPollStep() {
        TaskExecutor te = new TaskExecutor();
        te.add(new PackageToServer(null, null, null));
        te.step();
        PackageToClient ptc = te.poll();
        System.out.println(ptc.toString());
    }
}
