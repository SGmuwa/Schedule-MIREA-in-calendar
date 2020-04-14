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
import javax.persistence.GeneratedValue;
import javax.persistence.GenerationType;
import javax.persistence.Id;

@Entity
public class ScheduleStatus {
    @Id
    @GeneratedValue(strategy = GenerationType.AUTO)
    long id;

    public ScheduleStatus() {

    }

    public long getId() {
        return id;
    }


    public String getFile() {
        return file;
    }

    public float getPercentReady() {
        return percentReady;
    }

    public String getStatus() {
        return status;
    }

    public void setStatus(String status) {
        this.status = status;
    }

    public void setFile(String file) {
        this.file = file;
    }

    public void setMessages(String messages) {
        this.messages = messages;
    }

    public void setPercentReady(float percentReady) {
        this.percentReady = percentReady;
    }

    String status;
    String file;
    String messages;
    float percentReady;
}