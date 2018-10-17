package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.interpreter.Seeker;

import java.time.*;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.LinkedList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;


public class Couple {
    /**
     * Дата и время пары.
     */
    public ZonedDateTime DateAndTimeOfCouple;
    /**
     * Количество времени, сколько длится пара.
     */
    public ZonedDateTime DateAndTimeFinishOfCouple;
    /**
     * Название группы.
     */
    public String NameOfGroup;
    /**
     * Имя преподавателя.
     */
    public String NameOfTeacher;
    /**
     * Название пары.
     */
    public String ItemTitle;
    /**
     * Номер аудитории.
     */
    public String Audience;
    /**
     * Адрес корпуса.
     */
    public String Address;
    /**
     * Тип занятия (лекция, практика, лабораторная работа)
     */
    public String TypeOfLesson;

    /**
     * Получает на входе данные про одну строку. Принимает решение, в какие дни будут пары. Не делает выборку данных.
     * @param timeStartOfCouple Время начала пары.
     * @param timeFinishOfCouple Время конца пары.
     * @param seeker Критерии, в которых указано, в каких рамках необходимо составить расписание.
     * @param nameOfGroup Рассматриваемая группа.
     * @param dayOfWeek Рассматриваемый день недели.
     * @param isOdd True, если это для не чётной недели. False, если эта строка для чётной недели.
     * @param itemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
     * @param typeOfLesson Первая строка типа занятия.
     * @param nameOfTeacher Первая строка данных преподавателя.
     * @param audience Первая строка аудитории.
     * @param address Адрес корпуса.
     * @return Возвращает, в какие дни будут пары.
     */
    public static List<Couple> GetCouplesByPeriod(Seeker seeker, LocalTime timeStartOfCouple, LocalTime timeFinishOfCouple, DayOfWeek dayOfWeek, boolean isOdd, String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address) {
        ZonedDateTime startT =  ZonedDateTime.of(LocalDateTime.of(seeker.dateStart, LocalTime.of(0, 0)), seeker.timezoneStart);
        ZonedDateTime finishT = ZonedDateTime.of(LocalDateTime.of(seeker.dateFinish, LocalTime.of(23, 50)), seeker.timezoneStart);
        ZonedDateTime current = startT;
        int currentW = seeker.startWeek;

        itemTitle = normalizeString(itemTitle);
        typeOfLesson = normalizeString(typeOfLesson);
        nameOfTeacher = normalizeString(nameOfTeacher);
        audience = normalizeString(audience);
        address = normalizeString(address);

        return null;
    }

    /**
     * Получает на входе данные про одну строку. Принимает решение, в какие дни будут пары. Не делает выборку данных.
     * @param start Дата и время начала сессии. Расписание будет составлено с этого дня и времени.
     * @param finish Дата и время окончания сессии. Расписание будет составлено до этого дня и времени.
     * @param timeStartOfCouple Время начала пары.
     * @param timeFinishOfCouple Время окончания пары.
     * @param timezoneStart Часовой пояс, в котором начинается учебный план.
     * @param nameOfGroup Рассматриваемая группа.
     * @param dayOfWeek Рассматриваемый день недели. Использование: Напрмер, Calendar.MUNDAY.
     * @param isOdd True, если это для нечётной недели. False, если эта строка для чётной недели.
     * @param itemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
     * @param typeOfLesson Первая строка типа занятия.
     * @param nameOfTeacher Первая строка данных преподавателя.
     * @param audience Первая строка аудитории.
     * @param address Адрес корпуса.
     * @return Возвращает, в какие дни будут пары.
     */
    public static List<Couple> GetCouplesByPeriod(LocalDate start, LocalDate finish, LocalTime timeStartOfCouple, LocalTime timeFinishOfCouple, ZoneId timezoneStart, DayOfWeek dayOfWeek, boolean isOdd, String nameOfGroup, String itemTitle, String typeOfLesson, String nameOfTeacher, String audience, String address) {
        ///.get(Calendar.DAY_OF_WEEK);
        // TODO: Данная функция ещё не разработана.
        itemTitle = itemTitle.trim();
        if(itemTitle.contains(" н. ") || itemTitle.contains(" н "))
        {
            if(itemTitle.contains("кр. "));
        }
        return null;
    }

    /**
     * Данная функция вернёт, на каких неделях пара есть.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @param startWeek С какой недели начинается учёба?
     * @param limitWeek Максимальный доступный номер недели.
     * @param isOdd True, если это для нечётной недели. False, если эта строка для чётной недели.
     * @return Список необходимых недель.
     */
    private static List<Integer> getWeeks(String itemTitle, int startWeek, int limitWeek, boolean isOdd){
        if(limitWeek < startWeek) return null;
        ArrayList<Integer> goodWeeks = new ArrayList<>(limitWeek/2 + 1); // Контейнер с хорошими неделями
        List<Integer> exc = getAllExcetionWeeks(itemTitle);
        // Изменение входных параметров в зависимости от itemTitle.
        {
            Integer startWeekFromString = getFromStringStartWeek(itemTitle); // Получаем, с какой недели идут пары.
            if (startWeekFromString != null && startWeekFromString > startWeek) startWeek = startWeekFromString;
            Integer finishWeekFromString = getFromStringFinishWeek(itemTitle); // Получаем, с какой недели идут пары.
            if (finishWeekFromString != null && finishWeekFromString < limitWeek) limitWeek = finishWeekFromString;
        }
        for(int i = startWeek; i < limitWeek + 1; i += 2) {
            if(!exc.contains((Integer) i)) // Если это не исключение
                goodWeeks.add(i); // Пусть все недели - хорошие.
        }
        return goodWeeks;
    }

    /**
     * Данная функция отвечает, содержится ли в тексте (например, в названии предмета) запись о том, что пара начинается только с или до какой-то недели.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return True, если есть комментарий о начале или конце недель. Иначе - false.
     */
    public static boolean isStringBeginEndWeek(String itemTitle){
        // ^.+ н\.? .+$|^н\.? .+$|^.+ н\.?\b.+$
        Pattern p = Pattern.compile("($| )(До|до|с|С) \\d+(\\.| |$)"); // TODO: ERROR!
        Matcher m = p.matcher(itemTitle);
        return m.matches();
    }

    /**
     * Данная функция отвечает, содержится ли в тексте (например, в названии предмета) заметки о том, в каких неделях проходят пары.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return True, если стоит учесть внимание на исключения. Иначе - false.
     */
    public static boolean isStringHaveWeek(String itemTitle){
        // ^.+ н\.? .+$|^н\.? .+$|^.+ н\.?\b.+$
        Pattern p = Pattern.compile("($| )[нН](\\.| |$)"); // TODO: ERROR!
        Matcher m = p.matcher(itemTitle);
        return m.matches();
    }

    public static Integer[] getAllIntsFromString(String input) {
        LinkedList<Integer> numbers = new LinkedList<Integer>();
        Pattern p = Pattern.compile("-?\\d+");
        Matcher m = p.matcher(input);
        while (m.find()) {
            numbers.add(Integer.parseInt(m.group()));
        }
        return ((Integer[])numbers.toArray());
    }

    /**
     * Данная функция отвечает, содержится ли в тексте (например, в названии предмета) заметки о том, в каких неделях не проходят пары.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return True, если стоит учесть внимание на исключения. Иначе - false.
     */
    public static boolean isStringHaveWeekException(String itemTitle){
        // н\\.? |^н\\.? | н\\.?\b
        if(!isStringHaveWeek(itemTitle)) return false;
        Pattern p = Pattern.compile("($| )[кК]р(\\.| |$)"); // TODO: ERROR!
        Matcher m = p.matcher(itemTitle);
        return m.matches();
    }

    /**
     * Удаляет символы 0..0x1F, заменяя на пробелы. Удаляет пробелы слева и справа.
     * @param input Входная строка, из которой необходимо удалить управляющие символы.
     * @return Новый экземпляр строки без управляющих символов и левых-правых пробелов.
     */
    private static String normalizeString(String input) {
        if(input == null)
            return null;
        StringBuilder in = new StringBuilder(input);
        for(int i = in.length() - 1; i >= 0; i--) {
            if(in.charAt(i) < 32) {
                in.replace(i, i, " ");
            }
        }
        return in.toString().trim();
    }
}
