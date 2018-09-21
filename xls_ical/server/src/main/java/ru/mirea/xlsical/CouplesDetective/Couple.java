package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.interpreter.Seeker;

import java.time.*;
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
    public Duration DurationOfCouple;
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
     * @param nameOfGroup Рассматриваемая группа.
     * @param dayOfWeek Рассматриваемый день недели. Использование: Напрмер, Calendar.MUNDAY.
     * @param isOdd True, если это для не чётной недели. False, если эта строка для чётной недели.
     * @param itemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
     * @param typeOfLesson Первая строка типа занятия.
     * @param nameOfTeacher Первая строка данных преподавателя.
     * @param audience Первая строка аудитории.
     * @param address Адрес корпуса.
     * @return Возвращает, в какие дни будут пары.
     */
    public static List<Couple> GetCouplesByPeriod(Seeker seeker, DayOfWeek dayOfWeek, boolean isOdd, String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address) {
        ZonedDateTime startT =  ZonedDateTime.of(LocalDateTime.of(seeker.dateStart, LocalTime.of(0, 0)), seeker.timezoneStart);
        ZonedDateTime finishT = ZonedDateTime.of(LocalDateTime.of(seeker.dateFinish, LocalTime.of(23, 50)), seeker.timezoneStart);
        ZonedDateTime current = startT;
        int currentW = seeker.startWeek;
        itemTitle = itemTitle.trim();
        typeOfLesson = typeOfLesson.trim();
        nameOfTeacher = nameOfTeacher.trim();
        audience = audience.trim();
        address = address.trim();
        
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
     * @param isOdd True, если это для не чётной недели. False, если эта строка для чётной недели.
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
     * Данная функция отвечает, содержится ли в тексте (например, в названии предмета) заметки о том, в каких неделях проходят пары.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return True, если стоит учесть внимание на исключения. Иначе - false.
     */
    public static boolean isStringHaveWeek(String itemTitle){
        // ^.+ н\.? .+$|^н\.? .+$|^.+ н\.?\b.+$
        Pattern p = Pattern.compile("((^.+\\s)|(^))[нН]\\.?.+$");
        Matcher m = p.matcher(itemTitle.replaceAll("\n", " "));
        return m.matches();
    }

    /**
     * Данная функция отвечает, содержится ли в тексте (например, в названии предмета) заметки о том, в каких неделях не проходят пары.
     * @param itemTitle Заголовок названия предмета из таблицы расписания.
     * @return True, если стоит учесть внимание на исключения. Иначе - false.
     */
    public static boolean isStringHaveWeekException(String itemTitle){
        // н\\.? |^н\\.? | н\\.?\b
        if(!isStringHaveWeek(itemTitle)) return false;
        Pattern p = Pattern.compile("((^.+\\s)|(^))кр\\.?.+$");
        Matcher m = p.matcher(itemTitle.replaceAll("\n", " "));
        return m.matches();
    }
}
