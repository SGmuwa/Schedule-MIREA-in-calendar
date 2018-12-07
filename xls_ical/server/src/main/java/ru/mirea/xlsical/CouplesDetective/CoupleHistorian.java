package ru.mirea.xlsical.CouplesDetective;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.*;
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
    private DetectiveDate settingDates = new DetectiveDate();

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
        loadCache();
        updateCache();
        saveCache();
    }

    /**
     * Скачивает с сайта МИРЭА расписание.
     * Сортирует пары по дате-времени.
     * Анализирует будущие пары.
     * Сохраняет на диск.
     */
    private void updateCache() {
        LinkedList<CoupleInCalendar> outCache = new LinkedList<>(); // Итоговый кэш
        LinkedList<CoupleInCalendar> newCache = new LinkedList<>(); // То, что получили из МИРЭА
        ZonedDateTime now = ZonedDateTime.now();
        for (Iterator<ExcelFileInterface> it = edUpdater.openTablesFromExternal(); it.hasNext(); ) {
            ExcelFileInterface file = it.next();
            Detective detective = Detective.chooseDetective(file, settingDates);
            try {
                newCache.addAll(detective.startAnInvestigation(
                        detective.getStartTime(now),
                        detective.getFinishTime(now)
                ));
            } catch (IOException | DetectiveException e) {
                System.out.println(ZonedDateTime.now() + " CoupleHistorian.java: ignore detective: " + e.getLocalizedMessage());
            }
            try {
                file.close();
            } catch (IOException e) {
                System.out.println("can't close file: " + file.toString());
            }
        }
        // Всё, что позже этой метки - можно менять. Всё, что раньше - нелья.
        ZonedDateTime deadLine = now.minus(1, ChronoUnit.DAYS);
        // Добавим то, что было раньше.
        for(CoupleInCalendar couple : cache) {
            if (deadLine.compareTo(couple.dateAndTimeOfCouple) < 0) { // Раньше
                outCache.add(couple);
            }
            else break;
        }
        // Добавим то, что нового.

        for(CoupleInCalendar couple : newCache) {
            if (deadLine.compareTo(couple.dateAndTimeOfCouple) >= 0) { // Позже
                outCache.add(couple);
            }
        }
        sortByDateTime(outCache);
        mergeCouples(outCache);
        cache = outCache;
    }

    private void loadCache() {
        try {
            ObjectInputStream inObj = new ObjectInputStream(new FileInputStream("ArrayListOfCouplesInCalendar.dat"));
            Object object = inObj.readObject();
            LinkedList<CoupleInCalendar> out = (LinkedList<CoupleInCalendar>) object;
            inObj.close();
            cache = out;
        } catch(Exception error) {
            if(cache == null)
                cache = new LinkedList<>();
            System.out.println(ZonedDateTime.now() + "CoupleHistorian.java#loadCache(): " + error.getLocalizedMessage());
        }
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
                System.out.println("CoupleHistorian.java: I can't save baseData! " + e.getLocalizedMessage());
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
        ArrayList<CoupleInCalendar> out = new ArrayList<>(cache.size());
        Pattern p;
        try {
            p = Pattern.compile(queryCriteria.nameOfSeeker);
        } catch (PatternSyntaxException e) {
            p = null;
        }
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
            if (
                // queryCriteria.dateStart раньше или равно couple.dateAndTimeFinishOfCouple
                    queryCriteria.dateStart.compareTo(couple.dateAndTimeFinishOfCouple) <= 0
                            // couple.dateAndTimeOfCouple раньше или равно queryCriteria.dateFinish
                            && couple.dateAndTimeOfCouple.compareTo(queryCriteria.dateFinish) <= 0
            ) {
                if (p != null && (p.matcher(couple.nameOfGroup).find() || p.matcher(couple.nameOfTeacher).find())) {
                    // if by regex.
                    out.add(couple);
                } else if (queryCriteria.nameOfSeeker.equals(couple.nameOfGroup) || queryCriteria.nameOfSeeker.equals(couple.nameOfTeacher))
                    // if by equals.
                    out.add(couple);
            }
        }
        return out;
    }

    /**
     * Объеденяет повторяющиеся пары в {@link #sortByDateTime(List) отсортированном} масиве.
     * Повторяющимися парами являются такая пара учебных занятий, где
     * совпадает аудитория, время начала и конца пары, заголовок и тип пары.
     * @param listNeedMerge Список пар, в которых надо найти эквивалентный пары
     *                      и объединить между собой. Данный лист обязан быть отсортирован.
     * @see CoupleInCalendar#equals(Object) Подробнее об сравнении пар между собой.
     */
    private static void mergeCouples(LinkedList<CoupleInCalendar> listNeedMerge) {
        List<CoupleInCalendar> needDelete = new ArrayList<>();
        ArrayList<CoupleInCalendar> couplesInDay = new ArrayList<>();
        for(CoupleInCalendar couple : listNeedMerge) {
            /*
            Если в масиве ничего нет, то добавить пару в список пар дня.
            Если текущая пара есть в тот же день, что и все в списке пар конкретного дня,
            то добавить текущую пару в список пар дня.
             */
            if(couplesInDay.size() == 0 || couplesInDay.get(couplesInDay.size() - 1).dateAndTimeOfCouple.toLocalDate().equals(couple.dateAndTimeOfCouple.toLocalDate()))
                couplesInDay.add(couple);
            else {
                // Требуется
            }
        }
    }

    /**
     * Данный метод сортирует входные даныне по возрастанию даты.
     * @param listNeedMerge Входные данные, которые необходимо отсортировать.
     */
    private static void sortByDateTime(List<CoupleInCalendar> listNeedMerge) {
        listNeedMerge.sort(Comparator.comparing(coupleInCalendar -> coupleInCalendar.dateAndTimeOfCouple));
    }
}
