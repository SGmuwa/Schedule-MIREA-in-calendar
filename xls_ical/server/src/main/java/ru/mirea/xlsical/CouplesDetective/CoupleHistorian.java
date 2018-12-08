package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveDate;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.DetectiveException;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.Seeker;

import java.io.*;
import java.time.ZonedDateTime;
import java.time.temporal.ChronoUnit;
import java.util.*;
import java.util.concurrent.atomic.AtomicInteger;
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
    private DetectiveDate settingDates;



    public CoupleHistorian(ExternalDataUpdater edUpdater, DetectiveDate detectiveDate, boolean isNeedLoadSaveCache) {
        this(edUpdater, detectiveDate, isNeedLoadSaveCache, null);
    }

    public CoupleHistorian(ExternalDataUpdater edUpdater, DetectiveDate detectiveDate, boolean isNeedLoadSaveCache, ZonedDateTime now) {
        this(edUpdater, detectiveDate, isNeedLoadSaveCache, null, new PercentReady());
    }

    public CoupleHistorian(ExternalDataUpdater edUpdater, DetectiveDate detectiveDate, boolean isNeedLoadSaveCache, ZonedDateTime now, PercentReady pr) {
        this.edUpdater = edUpdater;
        this.settingDates = detectiveDate;
        if(this.settingDates == null)
            this.settingDates = new DetectiveDate();
        if(isNeedLoadSaveCache)
            loadCache();
        this.now = now;
        updateCache(pr);
        if(isNeedLoadSaveCache)
            saveCache();
    }

    public CoupleHistorian() {
        this(new PercentReady(), true);
    }

    public CoupleHistorian(PercentReady pr, boolean isNeedLoadSaveCache) {
        PercentReady PR_constructor = new PercentReady(pr, 0.000025f);
        PercentReady PR_external = new PercentReady(pr, 0.0025f - 0.000025f);
        try {
            this.settingDates = new DetectiveDate();
            PR_constructor.setReady(0.2f);
            this.edUpdater = new ExternalDataUpdater(PR_external);
            PR_constructor.setReady(0.4f);
        } catch (IOException e) {
            // Что делать, если не удаётся создать директорию кэша?
            // Переносим ответственность на администратора сервера.
            e.printStackTrace();
            System.out.println(e.getLocalizedMessage());
            Scanner sc = new Scanner(System.in);
            do {
                try {
                    System.out.print("Write path cache dir: ");
                    this.edUpdater = new ExternalDataUpdater(new File(sc.nextLine()), true, PR_external);
                } catch (IOException er) {
                    System.out.println(er.getLocalizedMessage());
                }
            } while(edUpdater == null);
        }
        if(isNeedLoadSaveCache)
            loadCache();
        PR_constructor.setReady(0.6f);
        updateCache(new PercentReady(pr, 1f-0.0025f));
        PR_constructor.setReady(0.8f);
        if(isNeedLoadSaveCache)
            saveCache();
        PR_constructor.setReady(1.0f);
    }

    private ZonedDateTime now = null;

    public void setNow(ZonedDateTime now) {
        this.now = now;
    }

    /**
     * Скачивает с сайта МИРЭА расписание.
     * Сортирует пары по дате-времени.
     * Анализирует будущие пары.
     * Сохраняет на диск.
     */
    private void updateCache(PercentReady pr) {
        PercentReady[] cycles = new PercentReady[] {
                new PercentReady(pr, 12f/16f),
                new PercentReady(pr, 1f/16f),
                new PercentReady(pr, 1f/16f)
        };
        PercentReady sort = new PercentReady(pr, 2f/16f);
        LinkedList<CoupleInCalendar> outCache = new LinkedList<>(); // Итоговый кэш
        LinkedList<CoupleInCalendar> newCache = new LinkedList<>(); // То, что получили из МИРЭА
        if(this.cache == null)
            this.cache = new LinkedList<>();
        ZonedDateTime now = this.now == null ? ZonedDateTime.now() : this.now;

        {
            IteratorExcels it = edUpdater.openTablesFromExternal();
            float size = it.size * 3;
            int i = 0;
            while(it.hasNext()) {
                ExcelFileInterface file = it.next();
                Detective detective = Detective.chooseDetective(file, settingDates);
                try {
                    newCache.addAll(detective.startAnInvestigation(
                            detective.getStartTime(now),
                            detective.getFinishTime(now)
                    ));
                } catch (IOException | DetectiveException e) {
                    //System.out.println(ZonedDateTime.now() + " CoupleHistorian.java: ignore detective: " + e.getLocalizedMessage());
                }
                try {
                    file.close();
                } catch (IOException e) {
                    System.out.println("can't close file: " + file.toString());
                }
                if(i + 1 < size)
                    cycles[0].setReady(++i/size);
            }
            cycles[0].setReady(1.0f);
        }
        // Всё, что позже этой метки - можно менять. Всё, что раньше - нелья.
        ZonedDateTime deadLine = now.minus(4, ChronoUnit.DAYS);
        // Добавим то, что было раньше.
        {
            float size = cache.size();
            int i = 0;
            for (CoupleInCalendar couple : cache) {
                if (couple.dateAndTimeOfCouple.compareTo(deadLine) < 0) { // Добавим то, что было до обновления.
                    outCache.add(couple);
                } else break;
                cycles[1].setReady(++i/size);
            }
            cycles[1].setReady(1.0f);
        }
        // Добавим то, что нового.

        {
            float size = newCache.size();
            int i = 0;
            for (CoupleInCalendar couple : newCache) {
                if (deadLine.compareTo(couple.dateAndTimeOfCouple) <= 0) { // Добавим новые данные
                    outCache.add(couple);
                }
                cycles[2].setReady(++i/size);
            }
            cycles[2].setReady(1.0f);
        }
        sortByDateTime(outCache, sort);
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
     * @param percentReady Указатель, куда помещать % готовности.
     * @return Новый список с календарными парами определённой группы или определённого
     *         преподавателя. Начиная с даты начала и заканчивая датой конца.
     */
    public ArrayList<CoupleInCalendar> getCouples(Seeker queryCriteria, PercentReady percentReady) {
        percentReady.setReady(0.0f);
        ArrayList<CoupleInCalendar> out = new ArrayList<>(cache.size());
        Pattern p;
        try {
            p = Pattern.compile(queryCriteria.nameOfSeeker);
        } catch (PatternSyntaxException e) {
            p = null;
        }
        int ready = 0;
        int size = cache.size();
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
            ready++;
            percentReady.setReady((float)ready/(float)size);
        }
        percentReady.setReady(1.0f);
        return out;
    }

    /**
     * Объеденяет повторяющиеся пары в {@link #sortByDateTime(List, PercentReady) отсортированном} масиве.
     * Повторяющимися парами являются такая пара учебных занятий, где
     * совпадает аудитория, время начала и конца пары, заголовок и тип пары.
     * @param listNeedMerge Список пар, в которых надо найти эквивалентный пары
     *                      и объединить между собой. Данный лист обязан быть отсортирован.
     * @see CoupleInCalendar#equals(Object) Подробнее об сравнении пар между собой.
     */
    private static void mergeCouples(LinkedList<CoupleInCalendar> listNeedMerge) {
        /*
        List<CoupleInCalendar> needDelete = new ArrayList<>();
        ArrayList<CoupleInCalendar> couplesInDay = new ArrayList<>();
        for(CoupleInCalendar couple : listNeedMerge) {*/
            /*
            Если в масиве ничего нет, то добавить пару в список пар дня.
            Если текущая пара есть в тот же день, что и все в списке пар конкретного дня,
            то добавить текущую пару в список пар дня.
             *//*
            if(couplesInDay.size() == 0 || couplesInDay.get(couplesInDay.size() - 1).dateAndTimeOfCouple.toLocalDate().equals(couple.dateAndTimeOfCouple.toLocalDate()))
                couplesInDay.add(couple);
            else {
                // Требуется
            }
        }*/
    }

    /**
     * Данный метод сортирует входные даныне по возрастанию даты.
     * @param listNeedMerge Входные данные, которые необходимо отсортировать.
     * @param pr Ссылка, куда надо отправлять отчёт о готовности.
     */
    private static void sortByDateTime(List<CoupleInCalendar> listNeedMerge, PercentReady pr) {
        float max = listNeedMerge.size() * (float)Math.log(listNeedMerge.size()) + 1;
        AtomicInteger i = new AtomicInteger(0);
        listNeedMerge.sort(Comparator.comparing(coupleInCalendar -> {
            if(i.get() < max) {
                pr.setReady(i.getAndIncrement() / max);
            }
            return coupleInCalendar.dateAndTimeOfCouple;
        }));
        pr.setReady(1.0f);
    }
}
