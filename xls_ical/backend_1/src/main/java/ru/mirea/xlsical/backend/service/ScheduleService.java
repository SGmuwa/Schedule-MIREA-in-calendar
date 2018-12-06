package ru.mirea.xlsical.backend.service;

import org.springframework.stereotype.Service;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

@Service
public class ScheduleService {
    public int threadNumber = 10;
    TaskExecutor taskExecutor = new TaskExecutor();
    Thread[] threadExecutorArr = new Thread[threadNumber];


    public void start() throws Exception
    {
        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i] = new Thread(taskExecutor);

        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i].start();

    }

    public PackageToClient add(PackageToServer p2s) {
        this.taskExecutor.add(p2s);
        try {
            PackageToClient p2c = this.taskExecutor.take();
            return p2c;
        } catch (Exception e) {

        }
        return null;
    }
}