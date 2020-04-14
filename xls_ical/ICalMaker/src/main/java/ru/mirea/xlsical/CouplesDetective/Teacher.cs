/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

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

package ru.mirea.xlsical.CouplesDetective;

import java.util.ArrayList;

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

    public static String[] ConvertNameFromStrToArray(String fullNameStr) {
        final int
                surname = 0, // Фамилия
                firstName = 1, // Имя
                patronymic = 2; // Отчество
        String[] fullNameArr = new String[]{"", "", ""};
        int currentFiledName = 0;
        for(int i = 0; i < fullNameStr.length() && currentFiledName <= patronymic; i++) {
            if(Character.isUpperCase(fullNameStr.charAt(i))) {
                int indexOfSpace = fullNameStr.indexOf(' ', i);
                int indexOfDot = fullNameStr.indexOf('.', i);
                int lastIndex = indexOfSpace < indexOfDot ? indexOfSpace : indexOfDot;
                lastIndex--;
                fullNameArr[currentFiledName] = fullNameStr.substring(i, lastIndex);
                i = lastIndex + 2;
                currentFiledName++;
            }
        }
        return fullNameArr;
    }

    private String surname;
    private String firstName;
    private String patronymic;

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
