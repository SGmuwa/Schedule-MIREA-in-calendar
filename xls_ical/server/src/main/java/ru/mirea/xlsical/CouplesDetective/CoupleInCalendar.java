package ru.mirea.xlsical.CouplesDetective;

import java.time.*;


/**
 * Структура данных, которая представляет учебную пару в определённый день и время.
 * Сокращённо: "Календарная пара".
 * Время начала и конца пары, название группы и имя преподавателя,
 * название предмета, аудитория, адрес, тип пары.
 */
public class CoupleInCalendar extends Couple {

    public CoupleInCalendar(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple) {
        super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
        this.dateAndTimeOfCouple = dateAndTimeOfCouple;
        this.dateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
    }

    /**
     * Дата и время пары.
     */
    public final ZonedDateTime dateAndTimeOfCouple;
    /**
     * Количество времени, сколько длится пара.
     */
    public final ZonedDateTime dateAndTimeFinishOfCouple;

    @Override
    public String toString() {
        return "CoupleInCalendar{" +
                "dateAndTimeOfCouple=" + dateAndTimeOfCouple +
                ", dateAndTimeFinishOfCouple=" + dateAndTimeFinishOfCouple +
                ", nameOfGroup='" + nameOfGroup + '\'' +
                ", nameOfTeacher='" + nameOfTeacher + '\'' +
                ", itemTitle='" + itemTitle + '\'' +
                ", audience='" + audience + '\'' +
                ", address='" + address + '\'' +
                ", typeOfLesson='" + typeOfLesson + '\'' +
                '}';
    }
}
