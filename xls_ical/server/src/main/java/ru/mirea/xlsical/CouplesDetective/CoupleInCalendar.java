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
