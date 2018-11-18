package ru.mirea.xlsical.CouplesDetective;

/**
 * Класс представляет из себя контейнер полей преподавателя.
 * @since 18.11.2018
 * @version 18.11.2018
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 */
public class Teacher {

    /**
     * Создание экземпляра преподавателя.
     * @param fullName ФИО преподавателя.
     * @param post Пост преподавателя.
     */
    public Teacher(String fullName, String post) {
        this.fullName   = fullName != null  ? fullName  : "";
        this.post       = post != null      ? post      : "";
    }

    /**
     * ФИО преподавателя.
     */
    public final String fullName;
    /**
     * Должность преподавателя.
     */
    public final String post;

    @Override
    public String toString() {
        return fullName + ' ' + post + '.';
    }
}
