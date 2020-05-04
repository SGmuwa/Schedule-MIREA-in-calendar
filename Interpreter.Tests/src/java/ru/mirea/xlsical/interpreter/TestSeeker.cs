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

namespace ru.mirea.xlsical.interpreter
{

    public class TestSeeker
    {
        [Fact]
        public void StartTestSeeker()
        {

            System.Console.WriteLine("Test Seeker start.");

            Seeker test = new Seeker(
                    "name",
                    new LocalDate(2000, 5, 5),
                    new LocalDate(2000, 5, 10),
                    DateTimeZoneProviders.Tzdb["Europe/Moscow"]
            );

            Assert.Equal("name", test.NameOfSeeker);
            Assert.Equal(new LocalDate(2000, 5, 5), test.DateStart.Date);
            Assert.Equal(new LocalDate(2000, 5, 10), test.DateFinish.Date);
            Assert.Equal(DateTimeZoneProviders.Tzdb["Europe/Moscow"], test.DateStart.Zone);

            PackageToProviderHTTP cl = new PackageToProviderHTTP(0, "", 0, "Всё ок");
            Assert.Equal("", cl.CalFile);
            Assert.Equal(0, cl.Count);
            Assert.Equal("Всё ок", cl.Messages);

            PackageToMakerICal sv = new PackageToMakerICal(0, test);

            Assert.Equal(test, sv.queryCriteria);
        }
    }
}
