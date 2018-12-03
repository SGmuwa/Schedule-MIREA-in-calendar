package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import java.io.*;
import java.time.LocalDate;
import java.time.ZonedDateTime;
import java.time.format.DateTimeParseException;
import java.util.ArrayList;

/**
 * Класс отвечает за то, чтобы отвечать на вопрос,
 * когда начинается и заканчивается семестр.
 */
public class DetectiveDate {
    public DetectiveDate() {
        File settings = new File("settings_DetectiveDate.cfg");
        try {
            if (settings.exists())
                loadFile(settings);
        } catch (IOException e) {
            System.out.println("Can't load settings for DetectiveDate.\nPlease, visit https://github.com/SGmuwa/Schedule-MIREA-in-the-calendar and look at example settings_DetectiveDate.cfg.\n" + settings.getAbsolutePath());
        }
    }

    private void loadFile(File settings) throws IOException {
        BufferedReader br = new BufferedReader(new FileReader(settings));
        for(String str = br.readLine(); str != null; str = br.readLine()) {
            if (str.length() > 6)
                try {
                    add(str);
                } catch (DateTimeParseException e) {
                    System.out.println(settings.getName() + ": can't read text: " + str);
                }
        }
        points.sort(LocalDate::compareTo);
    }

    /**
     * Осуществляет поиск начала семестра.
     * @param dateBeforeFinishNeedSemester Любая дата, которая указывает на промежуток: июль...декабрь (включительно) - для поиска конца осеннего семестра, январь...июнь - для поиска весеннего семестра.
     * @return Дата конца указанного семестра.
     */
    public ZonedDateTime searchStartSem(ZonedDateTime dateBeforeFinishNeedSemester) {
        throw new UnsupportedOperationException();
    }

    /**
     * Ключевые точки.
     * Начало первого семестра - 15 августа - 15 сентября
     * Конец первого семестра - декабрь
     * Начала второго семестра - с 15 января по 15 марта
     * Конец второго семестра - с 15 мая по 15 июня.
     */
    ArrayList<LocalDate> points = new ArrayList<>();

    private void add(String str) throws DateTimeParseException {
        points.add(LocalDate.parse(str));
    }

    private static class BinarySearch {
        private static <T extends Comparable<T>> int BinarySearch_Iter(ArrayList<T> array, boolean descendingOrder, T key) {
            int left = 0;
            int right = array.size();
            int mid = 0;

            while (!(left >= right))
            {
                mid = left + (right - left) / 2;

                if (array.get(mid) == key)
                    return mid;

                if ((array.get(mid).compareTo(key) > 0) ^ descendingOrder)
                    right = mid;
                else
                    left = mid + 1;
            }

            return ~left;
        }

        /**
         * Бинарный поиск. Если не нашёл - показывает ближайший индекс.
         * @param array
         * @param key
         * @param <E>
         * @return
         */
        public static <E extends Comparable<E>> int BinarySearch_Iter_Wrapper(ArrayList<E> array, E key)
        {
            if (array.size() == 0)
                return -1;
            return BinarySearch_Iter(array, array.get(0).compareTo(array.get(array.size() - 1)) > 0, key);
        }
    }
}
