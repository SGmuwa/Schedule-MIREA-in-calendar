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
using NodaTime;
using static ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective;

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{

    public class DetectiveTest
    {
        /// <summary>
        /// Функция разработана для математического вычисления,
        /// как надо реализовать расстановку начала и конца семестра.
        /// </summary>
        [Fact]
        public void calculateDays()
        {
            Assert.Equal(2, GetCounts6Days(
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 3)),
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 4))));

            Assert.Equal(1, GetCounts6Days(
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 1)),
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 2))));

            Assert.Equal(1, GetCounts6Days(
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 2)),
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 3))));

            Assert.Equal(6, GetCounts6Days(
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2000, 1, 1)),
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2000, 1, 7))));

            Assert.Equal(
                DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 4)),
                AddBusinessDaysToDate(DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2018, 12, 2)), 1));

            {
                ZonedDateTime a = DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2019, 1, 1));
                ZonedDateTime b = DateTimeZoneProviders.Tzdb["UTC"].AtStartOfDay(new LocalDate(2019, 2, 10));
                Assert.Equal(GetCounts6Days(a, b), GetCounts6Days(a, AddBusinessDaysToDate(a, GetCounts6Days(a, b) - 1)));
            }
        }

        [Fact]
        public void StartAndFinishDays()
        {
            DetectiveDate detectiveDate = new DetectiveDate();
            IDetective detective = new DetectiveSemester(null, detectiveDate);
            ZonedDateTime current = DateTimeZoneProviders.Tzdb["Europe/Minsk"].AtStrictly(new LocalDate(2019, 1, 1).At(LocalTime.Noon));

            Assert.Equal(
                    DateTimeZoneProviders.Tzdb["Europe/Minsk"].AtStrictly(new LocalDate(2018, 9, 3).At(LocalTime.Midnight)),
                    detective.GetStartTime(current));

            Assert.Equal(
                    DateTimeZoneProviders.Tzdb["Europe/Minsk"].AtStrictly(new LocalDate(2018, 12, 22).At(new LocalTime(23, 59, 59))),
                    detective.GetFinishTime(current));
        }
    }
}
