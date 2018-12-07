package ru.mirea.xlsical.backend.entity;

import javax.persistence.Entity;
import javax.persistence.Id;
import java.time.LocalDate;
import java.time.ZoneId;

@Entity
public class ScheduleQuery {
    @Id
    long id;

    public ScheduleQuery() {

    }

    public String name;
    public LocalDate dateStart;
    public LocalDate dateFinish;
    public ZoneId timezoneStart;

    public String getName() {
        return name;
    }

    public LocalDate getDateStart() {
        return dateStart;
    }

    public LocalDate getDateFinish() {
        return dateFinish;
    }

    public ZoneId getZoneId() {
        return timezoneStart;
    }
}
