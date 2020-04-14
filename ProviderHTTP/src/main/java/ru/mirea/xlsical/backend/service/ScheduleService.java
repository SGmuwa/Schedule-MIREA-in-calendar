/*
    Schedule MIREA in calendar.
    Copyright (C) 2020
    Artemy Mikhailovich Urodovskikh
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

package ru.mirea.xlsical.backend.service;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Service;
import ru.mirea.xlsical.CouplesDetective.CoupleHistorian;
import ru.mirea.xlsical.CouplesDetective.ExternalDataUpdater;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.repository.StatusRepository;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.SampleConsoleTransferPercentReady;
import ru.mirea.xlsical.interpreter.Seeker;

import javax.annotation.PostConstruct;
import java.io.IOException;
import java.time.LocalDate;
import java.time.ZoneId;
import java.util.ArrayList;
import java.util.Optional;

@Service
public class ScheduleService {
    public int threadNumber = 10;
    TaskExecutor taskExecutor = new TaskExecutor(new PercentReady(new SampleConsoleTransferPercentReady("new TaskExecutor: ")));
    Thread[] taskExecutorArr = new Thread[threadNumber];
    Thread[] runnableExecutorArr = new Thread[threadNumber];

    public Thread.UncaughtExceptionHandler h = new Thread.UncaughtExceptionHandler() {
        @Autowired
        ScheduleService s;

        public void uncaughtException(Thread th, Throwable ex) {
            System.out.println(th + ": " + ex);
        }
    };

    @Autowired
    StatusRunnable sr;

    @Autowired
    StatusRepository sp;

    public ScheduleService() throws IOException {
    }

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

        PercentReady pr = new PercentReady(new SampleConsoleTransferPercentReady("ScheduleService.java: "));
        ScheduleStatus status = new ScheduleStatus();
        status.setStatus("Pending");
        sp.save(status);

        Seeker seeker = new Seeker(name, start, finish, zoneid);
        PackageToServer p2s = new PackageToServer(status, pr, seeker);

        this.taskExecutor.add(p2s);
        return status;
    }

    public Optional<ScheduleStatus> get(long id) {
        return sp.findById(id);
    }
}