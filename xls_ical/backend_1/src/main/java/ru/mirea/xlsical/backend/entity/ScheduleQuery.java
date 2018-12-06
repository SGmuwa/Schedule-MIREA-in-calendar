package ru.mirea.xlsical.backend.entity;

import java.time.LocalDate;
import java.time.ZoneId;

public class ScheduleQuery {

    public String name;
    public LocalDate dateStart;
    public LocalDate dateFinish;
    public ZoneId timezoneStart;

}
