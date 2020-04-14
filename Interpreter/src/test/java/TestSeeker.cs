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

import java.time.LocalDate;
import java.time.ZoneId;

import org.junit.Test;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.SeekerType;

import static org.junit.Assert.*;

public class TestSeeker {

    @Test
    public void startTestSeeker() {

        System.out.println("Test Seeker start.");

        Seeker test = new Seeker(
                "name",
                LocalDate.of(2000, 5, 5),
                LocalDate.of(2000, 5, 10),
                ZoneId.systemDefault()
        );

        assertEquals("name", test.nameOfSeeker);
        assertEquals(LocalDate.of(2000, 5, 5), test.dateStart.toLocalDate());
        assertEquals(LocalDate.of(2000, 5, 10), test.dateFinish.toLocalDate());
        assertEquals(ZoneId.systemDefault(), test.dateStart.getZone());

        PackageToClient cl = new PackageToClient(0, "", 0, "Всё ок");
        assertEquals("", cl.CalFile);
        assertEquals(0, cl.Count);
        assertEquals("Всё ок", cl.Messages);

        PackageToServer sv = new PackageToServer(0, test);

        assertEquals(test, sv.queryCriteria);
    }


}