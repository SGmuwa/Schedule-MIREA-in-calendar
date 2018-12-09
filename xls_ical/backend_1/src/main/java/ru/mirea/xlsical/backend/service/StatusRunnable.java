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
        while (true) {
            try {
                PackageToClient p2c = this.scheduleService.taskExecutor.take();
                ScheduleStatus s = (ScheduleStatus)p2c.ctx;
                s.setStatus("Success");
                s.setFile(p2c.CalFile);
                s.setPercentReady(1);
                // s.setPercentReady(p2c.percentReady.getReady()); percentReady в таком случае равен всегда 1.
                s.setMessages(p2c.Messages);
                sp.save(s);
            } catch (Exception e) {

            }
        }
    }
}