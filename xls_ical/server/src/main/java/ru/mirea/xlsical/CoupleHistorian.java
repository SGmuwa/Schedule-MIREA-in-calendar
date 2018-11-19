package ru.mirea.xlsical;

import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.ExternalDataUpdater;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.time.ZonedDateTime;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.Scanner;

/**
 * Класс реализует защиту от записи прошедших пар.
 * Также предсказывает, какие пары будут в будущем.
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>.
 */
public class CoupleHistorian {

    private ExternalDataUpdater edUpdater = null;
    private LinkedList<CoupleInCalendar> cache;

    public CoupleHistorian() {
        try {
            this.edUpdater = new ExternalDataUpdater();
        } catch (IOException e) {
            // Что делать, если не удаётся создать директорию кэша?
            // Переносим ответственность на администратора сервера.
            e.printStackTrace();
            System.out.println(e.getLocalizedMessage());
            Scanner sc = new Scanner(System.in);
            do {
                try {
                    System.out.print("Write path cache dir: ");
                    this.edUpdater = new ExternalDataUpdater(sc.nextLine());
                } catch (IOException er) {
                    System.out.println(er.getLocalizedMessage());
                }
            } while(edUpdater == null);
        }
    }

    /**
     * Скачивает с сайта МИРЭА расписание.
     * Анализирует будущие пары.
     * Сохраняет на диск.
     */
    private void updateCashe() {
        LinkedList<CoupleInCalendar> outCache = new LinkedList<>(); // Итоговый кэш
        LinkedList<CoupleInCalendar> newCache = new LinkedList<>(); // То, что получили из МИРЭА
        for(ExcelFileInterface file : edUpdater.openTablesFromExternal()) {
            Detective detective = Detective.chooseDetective(file);
            newCache.addAll(detective.startAnInvestigation(
                    getDateStart(detective),
                    getDateFinish(detective)
            ));
        }
        // Всё, что позже этой метки - можно менять. Всё, что раньше - нелья.
        ZonedDateTime deadLine = ZonedDateTime.now().minus(1, ChronoUnit.DAYS);
        // Добавим то, что было раньше.
        for(CoupleInCalendar couple : cache) {
            if (deadLine.compareTo(couple.DateAndTimeOfCouple) < 0) { // Раньше
                outCache.add(couple);
            }
        }
        // Добавим то, что нового.
        for(CoupleInCalendar couple : newCache) {
            if (deadLine.compareTo(couple.DateAndTimeOfCouple) >= 0) { // Позже
                outCache.add(couple);
            }
        }
        cache = outCache;
        saveCache();
    }

    private void saveCache() {
        do {
            try {
                FileOutputStream fout = new FileOutputStream("ArrayListOfCouplesInCalendar.dat");
                ObjectOutputStream oos = new ObjectOutputStream(fout);
                oos.writeObject(cache);
                oos.close();
                fout.close();
                break;
            } catch (IOException e) {
                e.printStackTrace();
                System.out.println("I can't save baseData! Again?");
                new Scanner(System.in).next();
            }
        } while(true);
    }

    public ArrayList<CoupleInCalendar> getCouples(Seeker queryCriteria) {
    }
}
