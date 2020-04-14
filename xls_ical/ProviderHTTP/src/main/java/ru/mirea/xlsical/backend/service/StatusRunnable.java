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