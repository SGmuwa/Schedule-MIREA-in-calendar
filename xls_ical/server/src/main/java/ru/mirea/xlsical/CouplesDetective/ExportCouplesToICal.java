package ru.mirea.xlsical.CouplesDetective;

import net.fortuna.ical4j.data.CalendarBuilder;
import net.fortuna.ical4j.data.CalendarOutputter;
import net.fortuna.ical4j.model.*;
import net.fortuna.ical4j.model.component.VEvent;
import net.fortuna.ical4j.model.property.*;
import ru.mirea.xlsical.interpreter.PercentReady;

import java.io.File;
import java.io.FileOutputStream;
import java.io.IOException;
import java.time.temporal.ChronoField;
import java.util.Collection;
import java.util.Random;

public class ExportCouplesToICal {

    private static Random ran = new Random();

    /**
     * Конвектирует список пар в "*.ical" формат.
     * @param couples Перечисление пар, которые необходимо перевести в .ical.
     * @param percentReady Указатель, куда отправлять процент готовности.
     * @return Путь до .ical файла. Файл может быть удалён, если он старше 24 часов.
     */
    public static String start(Collection<CoupleInCalendar> couples, PercentReady percentReady) {
        PercentReady load = new PercentReady(percentReady, 0.05f);
        load.setReady(0.01f);
        PercentReady renderIcalPercentReady = new PercentReady(percentReady, 0.9f);
        PercentReady finishPercentReady = new PercentReady(percentReady, 0.05f);
        if(ran.nextInt() % 1000 == 0)
            clearCashOlder24H(); // Очистка кэша.
        load.setReady(0.1f);
        Calendar cal = new Calendar();
        load.setReady(0.25f);
        TimeZoneRegistry registry = new CalendarBuilder().getRegistry ();
        load.setReady(0.5f);
        cal.getProperties().add(new ProdId("-//RTU Roflex Team//xls_ical//RU"));
        cal.getProperties().add(Version.VERSION_2_0);
        cal.getProperties().add(CalScale.GREGORIAN);
        boolean count = false;
        load.setReady(0.75f);

        int ready = 0;
        float size = couples.size();
        load.setReady(1.0f);
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
            ready++;
            renderIcalPercentReady.setReady(ready/size);
        }
        finishPercentReady.setReady(0.1f);

        File cachePath = new File("cache/icals");

        if(!cachePath.exists()) {
            if(!cachePath.mkdirs()) {
                // Не были созданы.
                cachePath = new File("");
            }
        }

        finishPercentReady.setReady(0.3f);
        File nameFile = new File(cachePath, java.time.Instant.now().getLong(ChronoField.INSTANT_SECONDS) + "_" + ran.nextLong() + ".ics");
        finishPercentReady.setReady(0.4f);

        if(!count) {
            try {
                if(
                nameFile.createNewFile()) {
                    renderIcalPercentReady.setReady(1.0f);
                    finishPercentReady.setReady(1.0f);
                    return nameFile.getPath();
                }
            } catch (IOException e) {
                e.printStackTrace();
                System.out.println(e.getLocalizedMessage());
            }
            renderIcalPercentReady.setReady(1.0f);
            finishPercentReady.setReady(1.0f);
            return null;
        }
        FileOutputStream file = null;
        try {
            file = new FileOutputStream(nameFile);
            finishPercentReady.setReady(0.6f);
            new CalendarOutputter().output(cal, file);
            finishPercentReady.setReady(0.8f);
            file.close();
        } catch (IOException e)
        {
            finishPercentReady.setReady(1.0f);
            e.printStackTrace();
            if(file != null)
                try {file.close(); } catch (IOException e1) { return null; }
            return null;
        }
        finishPercentReady.setReady(1.0f);
        return nameFile.getPath();
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
