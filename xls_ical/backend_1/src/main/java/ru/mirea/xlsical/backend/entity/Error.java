package ru.mirea.xlsical.backend.entity;

import javax.persistence.Entity;
import javax.persistence.Id;

@Entity
public class Error {
    @Id
    long id;

    public Error() {

    }

    public Error(String text) {
        this.text = text;
    }


    public String getText() {
        return text;
    }

    String text;
}