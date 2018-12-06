package ru.mirea.xlsical.backend.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import ru.mirea.xlsical.backend.entity.ScheduleQuery;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.service.ScheduleService;
import ru.mirea.xlsical.backend.utils.ExceptionHandlerController;
import ru.mirea.xlsical.backend.utils.RestException;


import java.time.LocalDate;
import java.time.ZoneId;
import java.util.Optional;

@Controller
public class ScheduleController extends ExceptionHandlerController {

    @Autowired
    private ScheduleService scheduleService;

    @RequestMapping(value = "schedule", method = RequestMethod.POST)
    public @ResponseBody
    ScheduleStatus getSchedule(@RequestBody ScheduleQuery sq) throws RestException {
        try {
            scheduleService.start();
            String name = sq.getName();
            LocalDate start = sq.dateStart;
            LocalDate finish = sq.dateFinish;
            ZoneId zoneid = sq.timezoneStart;

            return scheduleService.add(name, start, finish, zoneid);

        } catch (Exception e) {
            throw new RestException(e);
        }
    }

    @RequestMapping(value = "status/{id}", method = RequestMethod.GET)
    @ResponseBody
    public Optional<ScheduleStatus> item(@PathVariable("id") long id) {
        return scheduleService.get(id);
    }

}

// TODO: эндпоинт с часовыми поясами