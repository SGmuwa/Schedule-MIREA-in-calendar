package ru.mirea.xlsical.backend.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.format.annotation.DateTimeFormat;
import org.springframework.stereotype.Controller;
import org.springframework.util.FileCopyUtils;
import org.springframework.web.bind.annotation.*;
import ru.mirea.xlsical.backend.entity.ScheduleQuery;
import ru.mirea.xlsical.backend.entity.ScheduleStatus;
import ru.mirea.xlsical.backend.service.ScheduleService;
import ru.mirea.xlsical.backend.utils.ExceptionHandlerController;
import ru.mirea.xlsical.backend.utils.RestException;


import javax.servlet.http.HttpServletResponse;
import java.io.File;
import java.io.FileInputStream;
import java.io.IOException;
import java.io.InputStream;
import java.time.LocalDate;
import java.time.ZoneId;
import java.util.Optional;

@Controller
public class ScheduleController extends ExceptionHandlerController {

    @Autowired
    private ScheduleService scheduleService;

//    @RequestMapping(value = "schedule", method = RequestMethod.POST)
    public @ResponseBody
    ScheduleStatus getSchedule(@RequestBody ScheduleQuery sq) throws RestException {
        try {
            String name = sq.getName();
            LocalDate start = sq.dateStart;
            LocalDate finish = sq.dateFinish;
            ZoneId zoneid = sq.timezoneStart;

            return scheduleService.add(name, start, finish, zoneid);

        } catch (Exception e) {
            throw new RestException(e);
        }
    }

//    @RequestMapping(value = "status/{id}", method = RequestMethod.GET)
    @ResponseBody
    public Optional<ScheduleStatus> item(@PathVariable("id") long id) {
        return scheduleService.get(id);
    }

    @RequestMapping(value = "schedule", method = RequestMethod.GET)
    public void getFile(
            @RequestParam("name") String name,
            @RequestParam(name = "dateStart" , required = true) @DateTimeFormat(pattern = "yyyy-MM-dd") LocalDate start,
            @RequestParam(name = "dateFinish", required = true) @DateTimeFormat(pattern = "yyyy-MM-dd") LocalDate finish,
            @RequestParam("timezoneStart") ZoneId zoneid,
            HttpServletResponse response) {

        try {
            ScheduleStatus addStatus = scheduleService.add(name, start, finish, zoneid);
            long id = addStatus.getId();
            ScheduleStatus getStatus = scheduleService.get(id).get();
            String res = (getStatus.getStatus());

            while (!(res.equals("Success"))) {
                getStatus = scheduleService.get(id).get();
                res = getStatus.getStatus();
                Thread.sleep(1000);
            }

            File file = new File(getStatus.getFile());
            InputStream is = new FileInputStream(file);
            FileCopyUtils.copy(is, response.getOutputStream());
            response.flushBuffer();
        } catch (IOException ex) {
            System.out.println("IOError writing file to output stream: " + ex);
        } catch (Exception e) {
            System.out.println(e);
        }
    }
}

// TODO: эндпоинт с часовыми поясами