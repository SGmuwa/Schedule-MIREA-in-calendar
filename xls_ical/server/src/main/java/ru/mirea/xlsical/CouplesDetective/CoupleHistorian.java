package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.FileOutputStream;
import java.io.IOException;
import java.io.ObjectOutputStream;
import java.time.ZonedDateTime;
import java.time.temporal.ChronoUnit;
import java.util.*;
import java.util.regex.Pattern;
import java.util.regex.PatternSyntaxException;

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
     * Фильтрует пары по типу запроса.
     * @param seeker Критерий. Регулярное выражение искателя.
     * @return Отфильтрованный по критерию.
     */
    private List<CoupleInCalendar> filterCouplesBySeekerType(Seeker seeker) throws PatternSyntaxException {
        List<CoupleInCalendar> output = new LinkedList<>();
        Pattern p = Pattern.compile(seeker.nameOfSeeker);
        for (CoupleInCalendar couple : cache) {
            if(p.matcher(couple.nameOfGroup).find() || p.matcher(couple.nameOfTeacher).find())
                output.add(couple);
        }
        return output;
    }

    /**
     * Скачивает с сайта МИРЭА расписание.
     * Анализирует будущие пары.
     * Сохраняет на диск.
     */
    private void updateCashe() throws IOException, DetectiveException {
        LinkedList<CoupleInCalendar> outCache = new LinkedList<>(); // Итоговый кэш
        LinkedList<CoupleInCalendar> newCache = new LinkedList<>(); // То, что получили из МИРЭА
        ZonedDateTime now = ZonedDateTime.now();
        for(ExcelFileInterface file : edUpdater.openTablesFromExternal()) {
            Detective detective = Detective.chooseDetective(file);
            newCache.addAll(detective.startAnInvestigation(
                    detective.getStartTime(now),
                    detective.getFinishTime(now)
            ));
        }
        // Всё, что позже этой метки - можно менять. Всё, что раньше - нелья.
        ZonedDateTime deadLine = now.minus(1, ChronoUnit.DAYS);
        // Добавим то, что было раньше.
        for(CoupleInCalendar couple : cache) {
            if (deadLine.compareTo(couple.dateAndTimeOfCouple) < 0) { // Раньше
                outCache.add(couple);
            }
        }
        // Добавим то, что нового.
        for(CoupleInCalendar couple : newCache) {
            if (deadLine.compareTo(couple.dateAndTimeOfCouple) >= 0) { // Позже
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

    /**
     * Получение календарного расписания по заданным критериям.
     * @param queryCriteria Критерии запроса, по которым будет происходить выборка данных.
     * @return Новый список с календарными парами определённой группы или определённого
     *         преподавателя. Начиная с даты начала и заканчивая датой конца.
     */
    public ArrayList<CoupleInCalendar> getCouples(Seeker queryCriteria) {
        for (CoupleInCalendar couple :
                cache) {
            /*

            -------------(A)-----------(T)-----------(B)-----------------(t)>

            A = queryCriteria.start
            B = queryCriteria.finish
            T = ...t... = couple
            out.add if A <= T && T <= B
            out.add if A раньше или равен T && T раньше или равен B

            if (time1.compareTo(time2) < 0) { // Если time1 раньше time2.

             */
            if (queryCriteria.dateStart.compareTo(couple.dateAndTimeOfCouple) <= 0) { // Раньше или равно

            }
        }
    }
}
