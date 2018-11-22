package ru.mirea.xlsical.CouplesDetective;

import net.fortuna.ical4j.data.CalendarBuilder;
import net.fortuna.ical4j.data.CalendarOutputter;
import net.fortuna.ical4j.model.*;
import net.fortuna.ical4j.model.component.VEvent;
import net.fortuna.ical4j.model.property.*;

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
    public static String start(Iterable<CoupleInCalendar> couples) {
        if(ran.nextInt() % 1000 == 0)
            clearCashOlder24H(); // Очистка кэша.
        Calendar cal = new Calendar();
        TimeZoneRegistry registry = new CalendarBuilder().getRegistry ();
        cal.getProperties().add(new ProdId("-//RTU Roflex Team//xls_ical//RU"));
        cal.getProperties().add(Version.VERSION_2_0);
        cal.getProperties().add(CalScale.GREGORIAN);
        boolean count = false;

        for(CoupleInCalendar c : couples) {
            count = true;
            VEvent ev = new VEvent();
            ev.getProperties().add(new Summary((c.itemTitle + " (" + c.typeOfLesson + ")")));
            ev.getProperties().add(new Description(c.audience + "\n" + c.nameOfGroup + "\n" + c.nameOfTeacher));
            ev.getProperties().add(new Location(c.address));
            ev.getProperties().add(new Uid(String.format("%d_%d@%s", java.time.ZonedDateTime.now().getLong(ChronoField.INSTANT_SECONDS), ran.nextLong(), "ru.mirea.xlsical")));

            DateProperty date = new DtStart(new DateTime(1000L*c.dateAndTimeOfCouple.getLong(ChronoField.INSTANT_SECONDS)), false);
            date.setTimeZone(registry.getTimeZone(c.dateAndTimeOfCouple.getZone().toString()));
            ev.getProperties().add(date);

            date = new DtEnd(new DateTime(1000L*c.dateAndTimeFinishOfCouple.getLong(ChronoField.INSTANT_SECONDS)), false);
            date.setTimeZone(registry.getTimeZone(c.dateAndTimeFinishOfCouple.getZone().toString()));
            ev.getProperties().add(date);

            cal.getComponents().add(ev);
        }
        if(!count)
            return null;

        String nameFile = "icals\\" + java.time.Instant.now().getLong(ChronoField.INSTANT_SECONDS) + "_" + ran.nextLong() + ".ics";

        if(!new File("icals").exists())
            new File("icals").mkdir();

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
        File[] files = new File("icals\\").listFiles();
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
