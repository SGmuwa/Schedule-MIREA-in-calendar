package ru.mirea.xlsical.CouplesDetective;

import java.util.Objects;

/**
 * Класс, который описывает в общем, что общего между.
 * календарной и эксель парой.
 * @since 16.11.2018
 * @version 16.11.2018
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 * @see CoupleInCalendar Пара для календаря
 */
public class Couple {

    /**
     * Название пары.
     */
    public final String itemTitle;
    /**
     * Тип занятия (лекция, практика, лабораторная работа)
     */
    public final String typeOfLesson;
    /**
     * Название группы.
     */
    public final String nameOfGroup;
    /**
     * Имя преподавателя.
     */
    public final String nameOfTeacher;
    /**
     * Номер аудитории.
     */
    public final String audience;
    /**
     * Адрес корпуса.
     */
    public final String address;

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
        this.itemTitle = itemTitle;
        this.typeOfLesson = typeOfLesson;
        this.nameOfGroup = nameOfGroup;
        this.nameOfTeacher = nameOfTeacher;
        this.audience = audience;
        this.address = address;
    }

    /**
     * Отвечат на вопрос, эквивалентен ли этот объект с сравниваемым объектом.<p/>
     *
     * Стоит отметить, что учавствуют в сравнении {@link #itemTitle}, {@link #typeOfLesson},
     * {@link #audience}. В сравнении не учавствуют:
     * <ul type="disc">
     *     <li>{@link #nameOfGroup}, так как в одной паре может учавствовать несколько групп.</li>
     * </ul>
     *
     * @param o Объект, с которым надо сравнивать текущий объект.
     * @return {@code True}, если два объекта совпадают. Иначе - {@code false}.
     */
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof Couple)) return false;
        Couple couple = (Couple) o;
        return itemTitle.equals(couple.itemTitle)
                && typeOfLesson.equals(couple.typeOfLesson)
                && audience.equals(couple.audience);
    }

    @Override
    public int hashCode() {
        return Objects.hash(itemTitle, typeOfLesson, audience);
    }
}
