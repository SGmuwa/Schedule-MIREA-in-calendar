package ru.mirea.xlsical.backend.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.repository.StatusRepository;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.Seeker;

import java.time.LocalDate;
import java.time.ZoneId;
import java.util.Optional;

@Service
public class ScheduleService {
    public int threadNumber = 10;
    TaskExecutor taskExecutor = new TaskExecutor();
    Thread[] threadExecutorArr = new Thread[threadNumber];

    @Autowired
    StatusRunnable sr;

    @Autowired
    StatusRepository sp;

    // должно запускаться при инициализации
    public void start() throws Exception
    {
        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i] = new Thread(taskExecutor);

        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i].start();

    }

    public ScheduleStatus add(String name, LocalDate start, LocalDate finish, ZoneId zoneid) {

        PercentReady pr = new PercentReady();
        ScheduleStatus status = new ScheduleStatus();
        status.setStatus("Pending");
        sp.save(status);

        Seeker seeker = new Seeker(name, start, finish, zoneid);
        String[] files = new String[1];
        files[0] = "backend_1/testsFiles/IIT-3k-18_19-osen.xlsx";
        PackageToServer p2s = new PackageToServer(status.getId(), pr, files, seeker);

        this.taskExecutor.add(p2s);
        return status;
    }

    public Optional<ScheduleStatus> get(long id) {
        return sp.findById(id);
    }
}