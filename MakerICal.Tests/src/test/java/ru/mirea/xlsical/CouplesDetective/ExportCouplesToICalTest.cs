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

import net.fortuna.ical4j.model.*;
import net.fortuna.ical4j.model.component.VEvent;
import org.junit.Test;

import static org.junit.Assert.*;

/// <summary>
/// Тестирование правильности экспорта данных в формат iCal.
/// </summary>
public class ExportCouplesToICalTest {

    @Test
    public void Tutorial(){
        // http://ical4j.sourceforge.net/introduction.html
        TimeZoneRegistry registry = TimeZoneRegistryFactory.getInstance().createRegistry();
        TimeZone timezone = registry.getTimeZone("Australia/Melbourne");

        java.util.Calendar cal = java.util.Calendar.getInstance(timezone);
        cal.set(java.util.Calendar.YEAR, 2005);
        cal.set(java.util.Calendar.MONTH, java.util.Calendar.NOVEMBER);
        cal.set(java.util.Calendar.DAY_OF_MONTH, 1);
        cal.set(java.util.Calendar.HOUR_OF_DAY, 15);
        cal.clear(java.util.Calendar.MINUTE);
        cal.clear(java.util.Calendar.SECOND);

        DateTime dt = new DateTime(cal.getTime());
        dt.setTimeZone(timezone);
        VEvent melbourneCup = new VEvent(dt, "Melbourne Cup");

        String[] result = melbourneCup.toString().split("\r\n");

        assertNotNull(result);
        assertEquals(5, result.length);
        assertEquals("BEGIN:VEVENT", result[0]);
        assertEquals("DTSTART;TZID=Australia/Melbourne:20051101T150000", result[2]);
        assertEquals("SUMMARY:Melbourne Cup", result[3]);
        assertEquals("END:VEVENT", result[4]);
    }
}
