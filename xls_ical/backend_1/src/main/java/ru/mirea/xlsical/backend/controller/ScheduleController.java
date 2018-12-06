package ru.mirea.xlsical.backend.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.*;
import ru.mirea.xlsical.backend.entity.ScheduleQuery;
import ru.mirea.xlsical.backend.service.ScheduleService;
import ru.mirea.xlsical.backend.utils.ExceptionHandlerController;
import ru.mirea.xlsical.backend.utils.RestException;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.PackageToClient;


import java.time.LocalDate;
import java.time.ZoneId;

@Controller
public class ScheduleController extends ExceptionHandlerController {

    @Autowired
    private ScheduleService scheduleService;

    @RequestMapping(value = "schedule", method = RequestMethod.POST)
    public @ResponseBody
    PackageToClient getSchedule(@RequestBody ScheduleQuery sq) throws RestException {
        // добавляет задание и возвращает статус и id

        try {
            System.out.println(0);
            scheduleService.start();
            System.out.println(10);

            String name = sq.name;

            LocalDate start = LocalDate.of(Integer.parseInt("2018"), Integer.parseInt("9"), Integer.parseInt("1"));
            LocalDate finish = LocalDate.of(Integer.parseInt("2018"), Integer.parseInt("12"), Integer.parseInt("31"));

//            LocalDate start = sq.dateStart;
//            LocalDate finish = sq.dateFinish;

//            ZoneId zoneid = ZoneId.of(sq.timezoneStart);
            ZoneId zoneid = ZoneId.of("UTC");
            PercentReady pr = new PercentReady();
            Seeker seeker = new Seeker(name, start, finish, zoneid);

            // TODO: Завести новую запись в таблице, присвоить p2s id этой записи
            // TODO: эндпоинт для получения статуса записи

            String[] files = new String[1];
            files[0] = "backend_1/testsFiles/IIT-3k-18_19-osen.xlsx";

            PackageToServer p2s = new PackageToServer(1, pr, files, seeker);

            System.out.println(11);
            return scheduleService.add(p2s);

        } catch (Exception e) {
            throw new RestException(e);
        }
    }

}

// TODO: эндпоинт, возвращающий статус задачи по Id

// TODO: эндпоинт с часовыми поясами