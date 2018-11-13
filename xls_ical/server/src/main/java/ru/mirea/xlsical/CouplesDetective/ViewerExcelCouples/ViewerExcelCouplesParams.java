package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import java.time.LocalDate;
import java.time.ZoneId;

/**
 * Класс, который представляет из себя запрос о построении семестра.
 * Стоит отметить, что данный класс отличается от {@link Seeker} тем,
 * что тут приведены параметры не от пользователя расписания, а от
 * системы принятия решения: то ли нужно построить расписание семестра,
 * то ли зачётов, то ли экзаменов.
 */
public class ViewerExcelCouplesParams extends Seeker {
    public ViewerExcelCouplesParams(String nameOfSeeker, SeekerType seekerType, LocalDate dateStart, LocalDate dateFinish, ZoneId timezoneStart, int startWeek) {
        super(nameOfSeeker, seekerType, dateStart, dateFinish, timezoneStart, startWeek);
    }
}
