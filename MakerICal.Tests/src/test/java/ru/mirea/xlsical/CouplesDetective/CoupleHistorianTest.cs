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
using System.Collections.Generic;

namespace ru.mirea.xlsical.CouplesDetective
{
    public class CoupleHistorianTest
    {
        [Fact]
        public void testBigCache()
        {
            CoupleHistorian a1 = GlobalTaskExecutor.coupleHistorian;
            CoupleHistorian a2 = GlobalTaskExecutor.coupleHistorian;
            LinkedList<CoupleInCalendar> c11 = a1.Cache;
            long f1 = GlobalTaskExecutor.fileForCacheCoupleHistorian.Length;
            a1.SaveCache();
            a1.LoadCache();
            long f2 = GlobalTaskExecutor.fileForCacheCoupleHistorian.Length;
            LinkedList<CoupleInCalendar> c12 = a1.Cache;
            LinkedList<CoupleInCalendar> c21 = a2.Cache;
            a2.SaveCache();
            a2.LoadCache();
            long f3 = GlobalTaskExecutor.fileForCacheCoupleHistorian.Length;
            LinkedList<CoupleInCalendar> c22 = a2.Cache;
            Assert.Equal(c11, c12);
            Assert.Equal(c12, c21);
            Assert.Equal(c21, c22);
            Assert.Equal(c11, c22);
            Assert.Equal(f1, f2);
            Assert.Equal(f1, f3);
        }
    }
}
