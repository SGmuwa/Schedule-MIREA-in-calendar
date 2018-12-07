package ru.mirea.xlsical.backend.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.repository.StatusRepository;
import ru.mirea.xlsical.interpreter.PackageToClient;

@Service
public class StatusRunnable implements Runnable {

    @Autowired
    private ScheduleService scheduleService;

    StatusRunnable(ScheduleService scheduleService) {
        this.scheduleService = scheduleService;
    }

    @Autowired
    StatusRepository sp;


    @Override
    public void run() {
        try {
            PackageToClient p2c = this.scheduleService.taskExecutor.take();
            ScheduleStatus s = sp.findById((long) p2c.ctx).get();
            s.setStatus("Success");
            s.setFile(p2c.CalFile);
            s.setPercentReady(p2c.percentReady.getReady());
            s.setMessages(p2c.Messages);
            sp.save(s);
        } catch (Exception e) {

        }}
}