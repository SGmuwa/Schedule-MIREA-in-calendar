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

using Xunit;
using Ical.Net;
using Ical.Net.CalendarComponents;
using NodaTime;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Тестирование правильности экспорта данных в формат iCal.
    /// </summary>
    public class ExportCouplesToICalTest
    {
        [Fact]
        public void Tutorial()
        {
            ZonedDateTime start = DateTimeZoneProviders.Tzdb["Australia/Melbourne"].AtStartOfDay(new LocalDate(2005, (int)IsoMonth.November, 15));
            CalendarEvent ev = new CalendarEvent
            {
                Start = start.ToCalDateTime(),
                Summary = "Melbourne Cup"
            };
            Calendar cal = new Calendar();
            cal.AddChild(ev);

            Assert.Equal(1, cal.Events.Count);

            ev = cal.Events[0];

            Assert.Equal(start.ToCalDateTime(), ev.Start);
            Assert.Equal("Melbourne Cup", ev.Summary);
        }
    }
}
