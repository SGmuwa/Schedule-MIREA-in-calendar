package ru.mirea.xlsical.backend.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.repository.StatusRepository;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.Seeker;

import javax.annotation.PostConstruct;
import java.time.LocalDate;
import java.time.ZoneId;
import java.util.Optional;

@Service
public class ScheduleService {
    public int threadNumber = 10;
    TaskExecutor taskExecutor = new TaskExecutor();
    Thread[] taskExecutorArr = new Thread[threadNumber];
    Thread[] runnableExecutorArr = new Thread[threadNumber];

    public Thread.UncaughtExceptionHandler h = new Thread.UncaughtExceptionHandler() {
        @Autowired
        ScheduleService s;

        public void uncaughtException(Thread th, Throwable ex) {
        }
    };

    @Autowired
    StatusRunnable sr;

    @Autowired
    StatusRepository sp;

    @PostConstruct
    public void start() throws Exception
    {
        for (int i=0; i<threadNumber;++i) {
            taskExecutorArr[i] = new Thread(taskExecutor);
            runnableExecutorArr[i] = new Thread(sr);
        }

        for (int i=0; i<threadNumber;++i) {
            taskExecutorArr[i].setUncaughtExceptionHandler(h);
            taskExecutorArr[i].start();
            runnableExecutorArr[i].start();
        }
    }

    public ScheduleStatus add(String name, LocalDate start, LocalDate finish, ZoneId zoneid) {

        PercentReady pr = new PercentReady();
        ScheduleStatus status = new ScheduleStatus();
        status.setStatus("Pending");
        sp.save(status);

        Seeker seeker = new Seeker(name, start, finish, zoneid);
        String[] files = new String[1];
        files[0] = "/Users/artemy/Downloads/IIT-3k-18_19-osen.xlsx";
        PackageToServer p2s = new PackageToServer(status.getId(), pr, files, seeker);

        this.taskExecutor.add(p2s);
        return status;
    }

    public Optional<ScheduleStatus> get(long id) {
        return sp.findById(id);
    }
}