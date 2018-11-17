package ru.mirea.xlsical.backend.controller;

import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.beans.factory.annotation.Qualifier;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;
import ru.mirea.xlsical.backend.service.DataService;
import ru.mirea.xlsical.backend.utils.Ajax;
import ru.mirea.xlsical.backend.utils.ExceptionHandlerController;
import ru.mirea.xlsical.backend.utils.RestException;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import java.time.LocalDate;
import java.time.ZoneId;
import java.util.Map;

@Controller
public class DataController extends ExceptionHandlerController {

//    private static final Logger LOG = Logger.getLogger(DataController.class);

    @Autowired
    @Qualifier("dataService")
    private DataService dataService;

//    @RequestMapping(value = "/persist", method = RequestMethod.POST)
//    public @ResponseBody
//    Map<String, Object> persist(@RequestParam("data") String data) throws RestException {
//        try {
//            if (data == null || data.equals("")) {
//                return Ajax.emptyResponse();
//            }
//            dataService.persist(data);
//            return Ajax.emptyResponse();
//        } catch (Exception e) {
//            throw new RestException(e);
//        }
//    }
//
//    @RequestMapping(value = "/getRandomData", method = RequestMethod.GET)
//    public @ResponseBody
//    Map<String, Object> getRandomData() throws RestException {
//        try {
////            Set<String> result = dataService.getRandomData();
//            Set<String> result = new HashSet<String>();
//            return Ajax.successResponse(result);
//        } catch (Exception e) {
//            throw new RestException(e);
//        }
//    }

    @RequestMapping(value = "/schedule", method = RequestMethod.POST)
    public @ResponseBody
    Map<String, Object> schedule(@RequestParam("query") String query) throws RestException {
        try {
//            if (query == null || query.equals("")) {
//                System.out.println("null query");
//                return Ajax.emptyResponse();
//            }

            String[] files = new String[3];
            files[0] = "../file";
            Seeker seeker = new Seeker
                    (
                            "1",
                            SeekerType.Teacher,
                            LocalDate.MIN,
                            LocalDate.MAX,
                            ZoneId.systemDefault(),
                            "МУУУУУУ",
                            1
                    );

            PackageToServer pack = new PackageToServer(312423, files, seeker);

//            dataService.persist(data);
            return Ajax.successResponse("Gut");
        } catch (Exception e) {
            throw new RestException(e);
        }
    }

}