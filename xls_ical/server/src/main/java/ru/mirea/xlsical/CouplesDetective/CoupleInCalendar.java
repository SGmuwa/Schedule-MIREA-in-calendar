package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.interpreter.Seeker;

import java.time.*;
import java.time.temporal.ChronoField;
import java.time.temporal.ChronoUnit;
import java.util.ArrayList;
import java.util.LinkedList;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;


/**
 * Структура данных, которая представляет учебную пару в определённый день и время.
 * Сокращённо: "Календарная пара".
 * Время начала и конца пары, название группы и имя преподавателя,
 * название предмета, аудитория, адрес, тип пары.
 */
public class CoupleInCalendar extends Couple {

    public CoupleInCalendar(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple) {
        super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
        DateAndTimeOfCouple = dateAndTimeOfCouple;
        DateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
    }

    /**
     * Дата и время пары.
     */
    public final ZonedDateTime DateAndTimeOfCouple;
    /**
     * Количество времени, сколько длится пара.
     */
    public final ZonedDateTime DateAndTimeFinishOfCouple;

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
    public static List<CoupleInCalendar> getCouplesByPeriod(Seeker seeker, LocalTime timeStartOfCouple, LocalTime timeFinishOfCouple, DayOfWeek dayOfWeek, boolean isOdd, String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address) {
        return getCouplesByPeriod(seeker.dateStart, seeker.dateFinish, seeker.timezoneStart, seeker.startWeek, timeStartOfCouple, timeFinishOfCouple, dayOfWeek, isOdd, itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
    }

    @Override
    public String toString() {
        return "CoupleInCalendar{" +
                "DateAndTimeOfCouple=" + DateAndTimeOfCouple +
                ", DateAndTimeFinishOfCouple=" + DateAndTimeFinishOfCouple +
                ", NameOfGroup='" + NameOfGroup + '\'' +
                ", NameOfTeacher='" + NameOfTeacher + '\'' +
                ", ItemTitle='" + ItemTitle + '\'' +
                ", Audience='" + Audience + '\'' +
                ", Address='" + Address + '\'' +
                ", TypeOfLesson='" + TypeOfLesson + '\'' +
                '}';
    }
}
