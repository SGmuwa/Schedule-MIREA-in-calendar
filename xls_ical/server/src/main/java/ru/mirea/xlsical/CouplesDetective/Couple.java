package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.interpreter.Seeker;

import java.time.*;
import java.time.temporal.ChronoField;
import java.time.temporal.ChronoUnit;
import java.time.temporal.TemporalAmount;
import java.time.temporal.TemporalField;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;


public class Couple {

    private Couple(ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple, String nameOfGroup, String nameOfTeacher, String itemTitle, String audience, String address, String typeOfLesson) {
        DateAndTimeOfCouple = dateAndTimeOfCouple;
        DateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
        NameOfGroup = nameOfGroup;
        NameOfTeacher = nameOfTeacher;
        ItemTitle = itemTitle;
        Audience = audience;
        Address = address;
        TypeOfLesson = typeOfLesson;
    }

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
     * @param seeker Критерии, в которых указано, в каких рамках необходимо составить расписание.
     * @param timeStartOfCouple Время начала пары.
     * @param timeFinishOfCouple Время конца пары.
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
        return GetCouplesByPeriod(seeker.dateStart, seeker.dateFinish, seeker.timezoneStart, seeker.startWeek, timeStartOfCouple, timeFinishOfCouple, dayOfWeek, isOdd, itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
    }

    /**
     * Функция убирает из названия предмета заметки о неделях.
     * @param itemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
     * @return Строка без цифр, "н.", "недель".
     * @deprecated TODO: Данная функция ещё не реализована.
     */
    private static String clearFromWeeks(String itemTitle) {
        return itemTitle;
    }

    /**
     * Получает на входе данные про одну строку. Принимает решение, в какие дни будут пары. Не делает выборку данных.
     * @param start Дата и время начала сессии. Расписание будет составлено с этого дня и времени.
     * @param finish Дата и время окончания сессии. Расписание будет составлено до этого дня и времени.
     * @param startZoneId Часовой пояс, в котором начинается учебный план.
     * @param startWeek С какого номера недели начать построение расписания?
     * @param timeStartOfCouple Время начала пары.
     * @param timeFinishOfCouple Время окончания пары.
     * @param dayOfWeek Рассматриваемый день недели. Использование: Напрмер, Calendar.MUNDAY.
     * @param isOdd True, если это для нечётной недели. False, если эта строка для чётной недели.
     * @param itemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
     * @param typeOfLesson Первая строка типа занятия.
     * @param nameOfGroup Рассматриваемая группа.
     * @param nameOfTeacher Первая строка данных преподавателя.
     * @param audience Первая строка аудитории.
     * @param address Адрес корпуса.
     * @return Возвращает, в какие дни будут пары.
     */
    public static List<Couple> GetCouplesByPeriod(LocalDate start, LocalDate finish, ZoneId startZoneId, int startWeek, LocalTime timeStartOfCouple, LocalTime timeFinishOfCouple, DayOfWeek dayOfWeek, boolean isOdd, String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address) {
        List<Couple> out;
        ZonedDateTime startT =  ZonedDateTime.of(LocalDateTime.of(start, LocalTime.of(0, 0)), startZoneId);
        ZonedDateTime finishT = ZonedDateTime.of(LocalDateTime.of(start, LocalTime.of(23, 50)), startZoneId);
        ZonedDateTime current = startT;
        long durationBetweenStartAndFinish = Duration.between(timeStartOfCouple, timeFinishOfCouple).toNanos();

        itemTitle = normalizeString(itemTitle);
        typeOfLesson = normalizeString(typeOfLesson);
        nameOfGroup = normalizeString(nameOfGroup);
        nameOfTeacher = normalizeString(nameOfTeacher);
        audience = normalizeString(audience);
        address = normalizeString(address);

        List<Integer> weeks = getWeeks(itemTitle, startWeek, startT.get(ChronoField.ALIGNED_WEEK_OF_YEAR) - finishT.get(ChronoField.ALIGNED_WEEK_OF_YEAR), isOdd);
        itemTitle = clearFromWeeks(itemTitle);
        out = new ArrayList<>(weeks.size() + 1);
        for(Integer numberOfWeek /*Номер недели*/ : weeks) {
            // Передвигаемся на неделю.
            current = startT.plus(numberOfWeek - 1, ChronoUnit.WEEKS);
            // Двигаемся к 00:00 dayOfWeek.
            current = current.minusNanos(current.getNano()).minusSeconds(current.getSecond()).minusMinutes(current.getMinute()).minusHours(current.getHour());
            int needAddDayOfWeek = current.getDayOfWeek().getValue() - dayOfWeek.getValue();
            current = current.plusDays(needAddDayOfWeek);
            current = current.plusNanos(timeStartOfCouple.getNano()).plusSeconds(timeStartOfCouple.getSecond()).plusMinutes(timeStartOfCouple.getMinute()).plusHours(timeStartOfCouple.getHour());
            if(
                    current.get(ChronoField.INSTANT_SECONDS) < ZonedDateTime.of(start, LocalTime.MIN, startZoneId).get(ChronoField.INSTANT_SECONDS) ||  // Использование LocalTime.MAX не безопасно: в дне может и не быть максимального локального времени. Использовано вместо этого прибавление одного дня и время 00:00.
                            current.get(ChronoField.INSTANT_SECONDS) >= ZonedDateTime.of(finish.plusDays(1), LocalTime.MIN, startZoneId).get(ChronoField.INSTANT_SECONDS)) {
                continue;
            }
            out.add(new Couple(
                    current,
                    current.plusNanos(durationBetweenStartAndFinish),
                    nameOfGroup,
                    nameOfTeacher,
                    itemTitle,
                    audience,
                    address,
                    typeOfLesson
            ));
        }
        return out;
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
        if(limitWeek < startWeek) return new ArrayList<>(1);
        ArrayList<Integer> goodWeeks = new ArrayList<>(limitWeek/2 + 1); // Контейнер с хорошими неделями
        List<Integer> exc = getAllExceptionWeeks(itemTitle);
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
     * Функция ищет, до какой недели идут пары. Ищет "До %d", где %d - целое число.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return Возвращает %d. В случае, если не найдено - возвращается {@code null}.
     */
    private static Integer getFromStringFinishWeek(String itemTitle) {
        Pattern p = Pattern.compile("(^| )[Дд]о ?+\\d+");
        Matcher m = p.matcher(itemTitle);
        if(m.find())
            return Integer.parseInt(m.group().substring(3));
        else
            return null;
    }

    /**
     * Функция ищет, с какой недели идут пары. Ищет "C %d", где %d - целое число.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return Возвращает %d. В случае, если не найдено - возвращается {@code null}.
     */
    private static Integer getFromStringStartWeek(String itemTitle) {
        Pattern p = Pattern.compile("(^| )[СсCc] ?+\\d+");
        Matcher m = p.matcher(itemTitle);
        if(m.find())
            return Integer.parseInt(m.group().substring(2));
        else return null;
    }

    /**
     * Функция ищет, в каких неделях есть исключения. Ищет "Кроме %d" или "кр. %d", где %d - целое число.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return Возвращает %d. В случае, если не найдено - возвращается пустой список.
     */
    private static List<Integer> getAllExceptionWeeks(String itemTitle) {
        Pattern p = Pattern.compile("[Кк]р(оме)?.? ?\\d+([,;] ?\\d+)+");
        Matcher m = p.matcher(itemTitle);
        if(m.find())
            return getAllIntsFromString(m.group());
        else
            return new LinkedList<>();
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
        Pattern p = Pattern.compile("($| )[нН](\\.| |$)");
        Matcher m = p.matcher(itemTitle);
        return m.find();
    }

    /**
     * Находит отрицательные и положительные целые числа в строке и выдаёт их список.
     * @param input Входная строка, где следует проводить поиск целых чисел.
     * @return Перечень найденных целых чисел. Если не найдено - пустой список.
     */
    private static List<Integer> getAllIntsFromString(String input) {
        LinkedList<Integer> numbers = new LinkedList<Integer>();
        Pattern p = Pattern.compile("-?\\d+");
        Matcher m = p.matcher(input);
        while (m.find()) {
            numbers.add(Integer.parseInt(m.group()));
        }
        return numbers;
    }

    private static Integer getFirstIntFromString(String input) {
        Pattern p = Pattern.compile("-?\\d+");
        Matcher m = p.matcher(input);
        if (m.find())
            return Integer.parseInt(m.group());
        else
            return null;
    }

    /**
     * Данная функция отвечает, содержится ли в тексте (например, в названии предмета) заметки о том, в каких неделях не проходят пары.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return True, если стоит учесть внимание на исключения. Иначе - false.
     */
    public static boolean isStringHaveWeekException(String itemTitle){
        // н\\.? |^н\\.? | н\\.?\b
        if(!isStringHaveWeek(itemTitle)) return false;
        Pattern p = Pattern.compile("(^| )[кК]р(\\.| |$)");
        Matcher m = p.matcher(itemTitle);
        return m.find();
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
