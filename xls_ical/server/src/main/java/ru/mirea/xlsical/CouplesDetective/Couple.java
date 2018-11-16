package ru.mirea.xlsical.CouplesDetective;

/**
 * Класс, который описывает в общем, что общего между.
 * календарной и эксель парой.
 * @since 16.11.2018
 * @version 16.11.2018
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 * @see CoupleInCalendar Пара для календаря
 * @see CoupleInExcel Пара для Excel
 */
public class Couple {

    /**
     * Название пары.
     */
    public final String ItemTitle;
    /**
     * Тип занятия (лекция, практика, лабораторная работа)
     */
    public final String TypeOfLesson;
    /**
     * Название группы.
     */
    public final String NameOfGroup;
    /**
     * Имя преподавателя.
     */
    public final String NameOfTeacher;
    /**
     * Номер аудитории.
     */
    public final String Audience;
    /**
     * Адрес корпуса.
     */
    public final String Address;

    /**
     * Создание в памяти экземпляров параметров вне-временых параметров пары.
     * @param itemTitle Название пары.
     * @param typeOfLesson Тип занятия (лекция, практика, лабораторная работа)
     * @param nameOfGroup Название группы.
     * @param nameOfTeacher Имя преподавателя.
     * @param audience Номер аудитории.
     * @param address Адрес корпуса.
     */
    protected Couple(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address) {
        ItemTitle = itemTitle;
        TypeOfLesson = typeOfLesson;
        NameOfGroup = nameOfGroup;
        NameOfTeacher = nameOfTeacher;
        Audience = audience;
        Address = address;
    }
}
