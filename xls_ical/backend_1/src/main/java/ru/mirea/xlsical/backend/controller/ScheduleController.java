package ru.mirea.xlsical.backend.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import ru.mirea.xlsical.backend.service.ScheduleService;
import ru.mirea.xlsical.backend.utils.Ajax;
import ru.mirea.xlsical.backend.utils.ExceptionHandlerController;
import ru.mirea.xlsical.backend.utils.RestException;

import java.util.Map;

@Controller
public class ScheduleController extends ExceptionHandlerController {

    @Autowired
    private ScheduleService scheduleService;

    @RequestMapping(value = "/getSchedule", method = RequestMethod.POST)
    public @ResponseBody
    Map<String, Object> getSchedule(@RequestParam("") String query) throws RestException {
        try {
//            if (query == null || query.equals("")) {
//                System.out.println("null query");
//                return Ajax.emptyResponse();
//            }

//            PackageToServer pack = new PackageToServer(312423, files, seeker);

            return Ajax.successResponse("Gut");
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

}