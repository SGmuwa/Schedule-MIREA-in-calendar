/*
[SG]Muwa Михаил Павлович Сидоренко. 2018.
Файл представляет из себя хранилище запроса.
Суть запроса: имя искателя, тип искателя (преподаватель, студент), дата начала и конца семестра, адрес по-умолчанию.
 */

package ru.mirea.xlsical.interpreter;

import java.io.Serializable;
import java.time.LocalDate;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.time.temporal.ChronoField;
import java.time.temporal.ChronoUnit;
import java.time.temporal.TemporalUnit;

/**
 * Класс, который представляет из себя искателя.
 * В этом классе содержатся все поля, которые запрашивает пользователь с интернета.
 */
public class Seeker implements Serializable {
    /**
     * Имя искателя.
     * Тут может содержаться как имя преподавателя, так и имя группы.
     * TODO: Необходимо сделать поддержку регулярных выражений.
     */
    public final String nameOfSeeker;
    /**
     * Тип искателя. Преподаватель или студент группы?
     * @deprecated Не используется, так как выборка идёт
     * как и по преподавателям, так и по группам одновременно.
     */
    public final SeekerType seekerType;
    /**
     * Начало составления ICal расписания.
     * В этот день уже будет составляться расписание.
     */
    public final LocalDate dateStart;
    /**
     * Дата конца составления ICal.
     * В этот день будет составлено расписание в последний раз.
     */
    public final LocalDate dateFinish;
    /**
     * Часовой пояс, где будут пары. Это значение в начале семестра.
     */
    public final ZoneId timezoneStart;
    /**
     * Адрес кампуса по-умолчанию.
     * @deprecated Требуется удаления, так как можно получить адрес из самой excel таблицы.
     */
    public final String defaultAddress;
    /**
     * Первоначальный номер недели. По-умолчанию указывать = 1.
     * @deprecated Не используется. Вместо этого идёт выборка данных.
     */
    public final int startWeek;

    /**
     * Создаёт экземпляр запроса.
     * @param nameOfSeeker Имя искателя. Это либо имя преподавателя, либо название группы студента.
     * @param seekerType Тип искателя. Это либо преподаватель, либо студент.
     * @param dateStart Дата начала составления расписания. С какого календарного дня надо составлять расписание? Дата указывается по местному времени.
     * @param dateFinish Дата конца составления расписания. До какого календарного дня надо составлять расписание? Дата указывается по местному времени.
     * @param timezoneStart Часовой пояс, где будут пары. Это значение в начале семестра.
     * @param defaultAddress Какой адрес корпуса по-умолчанию?
     * @param startWeek Первоначальный номер недели. По-умолчанию указывать = 1.
     * @deprecated Поля {@link #defaultAddress}, {@link #seekerType} и {@link #startWeek} уже не используется.
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

    /**
     * Создаёт экземпляр запроса.
     * @param nameOfSeeker Имя искателя. Это либо имя преподавателя, либо название группы студента.
     * @param dateStart Дата начала составления расписания. С какого календарного дня надо составлять расписание? Дата указывается по местному времени.
     * @param dateFinish Дата конца составления расписания. До какого календарного дня надо составлять расписание? Дата указывается по местному времени.
     * @param timezoneStart Часовой пояс, где будут пары. Это значение в начале семестра.
     */
    public Seeker(String nameOfSeeker, LocalDate dateStart, LocalDate dateFinish, ZoneId timezoneStart) {
        this.nameOfSeeker = nameOfSeeker;
        this.seekerType = SeekerType.NaN;
        this.dateStart = dateStart;
        this.dateFinish = dateFinish;
        this.timezoneStart = timezoneStart;
        this.defaultAddress = null;
        this.startWeek = 1;
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
                            timezoneStart.equals(e.timezoneStart) &&
                            defaultAddress.equals(e.defaultAddress) &&
                            startWeek == e.startWeek;
        }
        return false;
    }

    static {
        int startWeek = 0;

        /*
         * Для поддержки старых устройств необходим часовой пояс "UTC+03:00" [1],
         * так как не все устройства[2] приняли обновления от 26 октября 2014 [3].
         * Поменять на Europe/Moscow 15 августа 2020 году.
         *
         * [1][3] http://www.consultant.ru/document/cons_doc_LAW_114656/b2707989c276b5a188e63bc41e7bcbcc18723de8/
         * [2] https://dentnt.windowsfaq.ru/?p=1527
         */
        ZoneId zoneId = ZoneId.of("UTC+03:00");


        ZonedDateTime start = ZonedDateTime.now(zoneId);
        // Дольше же восьми лет не может учиться студент?
        ZonedDateTime finish = start.plus(8, ChronoUnit.YEARS);



        instance = new Seeker(
                // Штука, которая расчитывает группу по-умолчанию.
                "ИКБО-04-" + start.minus(6L, ChronoUnit.MONTHS).getLong(ChronoField.YEAR) % 100,
                start.toLocalDate(),
                finish.toLocalDate(),
                zoneId
        );
    }

    private static Seeker instance;

    /**
     * Возвращает искателя по-умолчанию.
     * @return Искатель по-умолчанию.
     */
    public static Seeker getInstance() {
        return instance;
    }
}
