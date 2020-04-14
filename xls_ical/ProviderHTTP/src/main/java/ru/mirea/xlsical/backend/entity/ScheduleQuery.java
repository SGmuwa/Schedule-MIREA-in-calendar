/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Artemy Mikhailovich Urodovskikh

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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
