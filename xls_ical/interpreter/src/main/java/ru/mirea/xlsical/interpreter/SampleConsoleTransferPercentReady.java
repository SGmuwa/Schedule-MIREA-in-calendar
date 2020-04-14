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

package ru.mirea.xlsical.interpreter;

public class SampleConsoleTransferPercentReady implements ICanUsePercentReady {

    public SampleConsoleTransferPercentReady() {

    }
    public SampleConsoleTransferPercentReady(String message) {
        this.message = message;
    }

    private String oldValue = "";
    private String message = "";
    /**
     * Вызывается всегда, когда используется setValue.
     *
     * @param pr Объект, который был изменён.
     */
    @Override
    public void transferValue(PercentReady pr) {
        String newValue = pr.toString();
        if(!newValue.equals(oldValue)) {
            oldValue = newValue;
            System.out.println(message + newValue);
        }
    }
}
