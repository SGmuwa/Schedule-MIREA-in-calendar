package ru.mirea.xlsical.CouplesDetective;

import java.time.LocalTime;

public class CoupleInExcel extends Couple {

    /**
     * Время начала пары.
     */
    public final LocalTime Start;

    /**
     * Время конца пары.
     */
    public final LocalTime Finish;

    public CoupleInExcel(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, LocalTime start, LocalTime finish) {
        super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
        Start = start;
        Finish = finish;
    }
}
