package ru.mirea.xlsical.backend.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import ru.mirea.xlsical.backend.entity.ScheduleQuery;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.service.ScheduleService;
import ru.mirea.xlsical.backend.utils.ExceptionHandlerController;
import ru.mirea.xlsical.backend.utils.RestException;
import ru.mirea.xlsical.interpreter.PackageToClient;


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


            // TODO: сериализация из реального запроса
            LocalDate start = LocalDate.of(Integer.parseInt("2018"), Integer.parseInt("9"), Integer.parseInt("1"));
            LocalDate finish = LocalDate.of(Integer.parseInt("2018"), Integer.parseInt("12"), Integer.parseInt("31"));
//            LocalDate start = sq.dateStart;
//            LocalDate finish = sq.dateFinish;
//            ZoneId zoneid = ZoneId.of(sq.timezoneStart);
            ZoneId zoneid = ZoneId.of("UTC");

            try {
                ScheduleStatus res = scheduleService.add(name, start, finish, zoneid);
                return res;
            } catch (Exception e) {
                return null;
            }

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