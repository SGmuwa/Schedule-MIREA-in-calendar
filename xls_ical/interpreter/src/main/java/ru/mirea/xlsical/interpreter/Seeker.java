/*
[SG]Muwa Михаил Павлович Сидоренко. 2018.
Файл представляет из себя хранилище запроса.
Суть запроса: имя искателя, тип искателя (преподаватель, студент), дата начала и конца семестра, адрес по-умолчанию.
 */

package ru.mirea.xlsical.interpreter;

import java.io.Serializable;
import java.time.LocalDate;
import java.time.ZoneId;

public class Seeker implements Serializable {
    /**
     * Имя искателя.
     */
    public final String nameOfSeeker;
    /**
     * Тип искателя. Преподаватель или студент группы?
     */
    public final SeekerType seekerType;
    /**
     * Начало семестра.
     */
    public final LocalDate dateStart;
    /**
     * Конец семестра.
     */
    public final LocalDate dateFinish;
    /**
     * Часовой пояс, где будут пары. Это значение в начале семестра.
     */
    public final ZoneId timezoneStart;
    /**
     * Адрес кампуса по-умолчанию.
     */
    public final String defaultAddress;
    /**
     * Первоначальный номер недели. По-умолчанию указывать = 1.
     */
    public final int startWeek;


    //public List<Couple> Couples = new LinkedList<>(); Нельзя здесь зависимость от Couple.

    /**
     * Создаёт экземпляр запроса.
     * @param nameOfSeeker Имя искателя. Это либо имя преподавателя, либо имя студента.
     * @param seekerType Тип искателя. Это либо преподаватель, либо студент.
     * @param dateStart Дата начала составления расписания. С какого календарного дня надо составлять расписание? Дата указывается по местному времени.
     * @param dateFinish Дата конца составления расписания. До какого календарного дня надо составлять расписание? Дата указывается по местному времени.
     * @param timezoneStart Часовой пояс, где будут пары. Это значение в начале семестра.
     * @param defaultAddress Какой адрес корпуса по-умолчанию?
     * @param startWeek Первоначальный номер недели. По-умолчанию указывать = 1.
     */
    public Seeker(String nameOfSeeker, SeekerType seekerType, LocalDate dateStart, LocalDate dateFinish, ZoneId timezoneStart, String defaultAddress, int startWeek) {
        this.nameOfSeeker = nameOfSeeker;
        this.seekerType = seekerType;
        this.dateStart = dateStart;
        this.dateFinish = dateFinish;
        this.timezoneStart = timezoneStart;
        this.defaultAddress = defaultAddress;
        this.startWeek = startWeek;
    }

    @Override
    public boolean equals(Object ex) {
        if (this == ex) {
            return true;
        }
        if(ex instanceof Seeker) {
            Seeker e = (Seeker) ex;
            return
                    nameOfSeeker.equals(e.nameOfSeeker) &&
                            seekerType.equals(e.seekerType) &&
                            dateStart.equals(e.dateStart) &&
                            dateFinish.equals(e.dateFinish) &&
                            timezoneStart == e.timezoneStart &&
                            defaultAddress.equals(e.defaultAddress);
        }
        return false;
    }
}
