package ru.mirea.xlsical.CouplesDetective;

import net.fortuna.ical4j.data.CalendarOutputter;
import net.fortuna.ical4j.model.Calendar;
import net.fortuna.ical4j.model.DateTime;
import net.fortuna.ical4j.model.component.VEvent;
import net.fortuna.ical4j.model.property.CalScale;
import net.fortuna.ical4j.model.property.ProdId;
import net.fortuna.ical4j.model.property.Version;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.time.temporal.ChronoField;
import java.util.Random;

public class ExportCouplesToICal {

    private static Random ran = new Random();

    /**
     * Конвектирует список пар в "*.ical" формат.
     * @param couples Перечисление пар, которые необходимо перевести в .ical.
     * @return Путь до .ical файла. Файл может быть удалён, если он старше 24 часов.
     */
    public static String start(Iterable<Couple> couples) {
        if(ran.nextInt() % 1000 == 0)
            clearCashOlder24H(); // Очистка кэша.
        Calendar cal = new Calendar();
        cal.getProperties().add(new ProdId("-//RTU Roflex Team//xls_ical//RU"));
        cal.getProperties().add(Version.VERSION_2_0);
        cal.getProperties().add(CalScale.GREGORIAN);

        for(Couple c : couples) {
            VEvent ev = new VEvent();
            ev.getSummary().setValue(c.ItemTitle + " (" + c.TypeOfLesson + ")");
            ev.getDescription().setValue(c.Audience + "\\n" + c.NameOfGroup + "\\n" + c.NameOfTeacher);
            ev.getLocation().setValue(c.Address);
            ev.getStartDate().setDate(new DateTime(c.DateAndTimeOfCouple.getLong(ChronoField.INSTANT_SECONDS)));
            ev.getStartDate().getTimeZone().setID(c.DateAndTimeOfCouple.getZone().toString());
            ev.getEndDate().setDate(new DateTime(c.DateAndTimeFinishOfCouple.getLong(ChronoField.INSTANT_SECONDS)));
            ev.getEndDate().getTimeZone().setID(c.DateAndTimeFinishOfCouple.getZone().toString());
            cal.getComponents().add(ev);
        }

        String nameFile = "icals/" + java.time.Instant.now().toString() + "_" + ran.nextLong() + ".ics";

        FileOutputStream file = null;
        try {
            file = new FileOutputStream(nameFile);
            new CalendarOutputter().output(cal, file);
            file.close();
        } catch (IOException e)
        {
            e.printStackTrace();
            if(file != null)
                try {file.close(); } catch (IOException e1) { return null; }
            return null;
        }
        return nameFile;
    }

    /**
     * Очищает кэш, которому более 24 часа.
     * @return Количество удалённых файлов.
     */
    private static int clearCashOlder24H() {
        File[] files = new File("ical/").listFiles();
        if (files == null) return 0;
        int countDel = 0;
        for(File f : files) {
            if(Long.parseLong(f.getName().split("_")[0]) < java.time.Instant.now().getLong(ChronoField.INSTANT_SECONDS) - 60*60*24)
                if(f.delete())
                    countDel++;
        }
        return countDel;
    }
}
