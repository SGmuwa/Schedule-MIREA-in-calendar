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