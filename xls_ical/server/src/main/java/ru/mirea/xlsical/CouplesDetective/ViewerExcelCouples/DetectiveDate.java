package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.BinarySearch;

import java.io.*;
import java.time.Duration;
import java.time.LocalDate;
import java.time.LocalTime;
import java.time.ZonedDateTime;
import java.time.format.DateTimeParseException;
import java.util.ArrayList;

/**
 * Класс отвечает за то, чтобы отвечать на вопрос,
 * когда начинается и заканчивается семестр.
 */
public class DetectiveDate {

    public DetectiveDate() {
        this(new File("settings_DetectiveDate.cfg"));
    }

    public DetectiveDate(File filename) {
        if(filename == null)
            return;
        if (filename.exists())
            loadFile(filename);
        else {
            try {
                filename.createNewFile();
            }
            catch (IOException e){
                System.out.println("Can't create file: " + filename.getAbsolutePath());
                return;
            }
            try(BufferedWriter bw = new BufferedWriter(new FileWriter(filename))) {
                bw.write("! В этом файле надо указывать ключевые точки в расписании."); bw.newLine();
                bw.write("! Здесь надо указывать следующие даты:"); bw.newLine();
                bw.write("! а) Если эта дата является началом семестра,"); bw.newLine();
                bw.write("! б) Если эта дата является концом зачётной недели."); bw.newLine();
                bw.write("! Формат записи дат: \"год-месяц-день\" (без кавычек)"); bw.newLine();
                bw.write("! Например, дата 2019-02-11 подскажет программе, что"); bw.newLine();
                bw.write("! 11 февраля 2019 года - первый день весеннего семестра."); bw.newLine();
                bw.write("! Некоторые даты программа может угадать самостоятельно (не внося в этот файл)"); bw.newLine();
                bw.write("! Например, 1 сентября 2018 года была суббота, программа"); bw.newLine();
                bw.write("! автоматически перенесёт начало семестра на 3 сентября (пн)."); bw.newLine();
                bw.write("! Данный файл необходим, так как первая дата учёбы влияет"); bw.newLine();
                bw.write("! на номер недели в расписании."); bw.newLine();
                bw.write("! Восклицательный знак в начале строки признак комментария."); bw.newLine();
                bw.write("! Я заполню начала весенних семестров, которые мне известны:");
                bw.write("2015-02-09"); bw.newLine();
                bw.write("2017-02-06"); bw.newLine();
                bw.write("2018-02-09"); bw.newLine();
                bw.write("2019-02-11"); bw.newLine();
                bw.write("! Заполните файл далее ниже самостоятельно."); bw.newLine();
            } catch (IOException e) {
                System.out.println("Can't create default setting file (" + filename.getAbsolutePath() + "). Please, fix problem: " + e.getLocalizedMessage());
            }
        }
    }

    private void loadFile(File settings) {
        try (BufferedReader br = new BufferedReader(new FileReader(settings))) {

            for (String str = br.readLine(); str != null; str = br.readLine()) {
                if (str.length() > 6 && str.charAt(0) != '!')
                    try {
                        add(str);
                    } catch (DateTimeParseException e) {
                        System.out.println(settings.getName() + ": can't read text: " + str);
                    }
            }
            points.sort(LocalDate::compareTo);
        } catch (IOException e) {
            System.out.println(e.getLocalizedMessage() + " Can't open file settings for DetectiveDate: " + settings.getAbsolutePath() + "\nPlease, visit https://github.com/SGmuwa/Schedule-MIREA-in-the-calendar and look at example settings_DetectiveDate.cfg.");
        }
    }

    /**
     * Осуществляет поиск контрольных точек.
     * @param dateToNeed Приблизительная дата, которая вам необходима
     * @param allow Границы поиска. Если точка слева или справа будут
     *              вне границ, они не будут в результате.
     * @return Контрольная точка ранее {@code dateToNeed} и контрольная
     * точка после {@code dateToNeed}.
     */
    public TwoZonedDateTime searchBeforeAfter(ZonedDateTime dateToNeed, Duration allow) {
        TwoZonedDateTime out = new TwoZonedDateTime(null, null);
        int left = BinarySearch.BinarySearch_Iter_Wrapper(points, dateToNeed.toLocalDate());
        if(left >= 0) { // Нашёлся
            out.left = ZonedDateTime.of(points.get(left), LocalTime.of(0, 0, 0), dateToNeed.getZone());
            if(left + 1 < points.size()) {
                ZonedDateTime rightValue = ZonedDateTime.of(points.get(left + 1), LocalTime.of(0, 0, 0), dateToNeed.getZone());
                if(Duration.between(dateToNeed, rightValue).compareTo(allow) < 0)
                    out.right = rightValue;
            }
        }
        else {
            left = ~left;
            if(left < points.size()) {
                ZonedDateTime leftValue;
                do {
                    leftValue = ZonedDateTime.of(points.get(left), LocalTime.of(0, 0, 0), dateToNeed.getZone());
                    if(leftValue.compareTo(dateToNeed) > 0) {
                        System.out.println("DetectiveDate.java: left move");
                        left--;
                        continue;
                    }
                    break;
                } while(true);
                if(Duration.between(leftValue, dateToNeed).compareTo(allow) < 0)
                    out.left = leftValue;
            }
            if(0 <= left + 1 && left + 1 < points.size()) {
                ZonedDateTime rightValue = ZonedDateTime.of(points.get(left + 1), LocalTime.of(0, 0, 0), dateToNeed.getZone());
                if(Duration.between(dateToNeed, rightValue).compareTo(allow) < 0)
                    out.right = rightValue;
            }
        }
        return out;
    }

    /**
     * Ключевые точки.
     * Начало первого семестра - 15 августа - 15 сентября
     * Конец первого семестра - декабрь
     * Начала второго семестра - с 15 января по 15 марта
     * Конец второго семестра - с 15 мая по 15 июня.
     */
    private ArrayList<LocalDate> points = new ArrayList<>();

    private void add(String str) throws DateTimeParseException {
        points.add(LocalDate.parse(str));
    }

    public class TwoZonedDateTime {
        private ZonedDateTime left;
        private ZonedDateTime right;

        private TwoZonedDateTime(ZonedDateTime left, ZonedDateTime right) {
            this.left = left;
            this.right = right;
        }

        /**
         * Получает точку времени левее или равная.
         */
        public ZonedDateTime getLeft() {
            return left;
        }

        /**
         * Получает точку правее.
         */
        public ZonedDateTime getRight() {
            return right;
        }
    }
}
