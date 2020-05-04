/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)
    George Andreevich Falileev

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

using System.Collections;
using System.Collections.Generic;

namespace ru.mirea.xlsical.CouplesDetective
{
    public partial class CoupleInCalendar
    {
        /// <summary>
        /// Класс, который реализует переход к следующим календарным парам.
        /// </summary>
        private class CouplesEnumerator : IEnumerator<CoupleInCalendar>
        {
            public CouplesEnumerator(CoupleInCalendar first) => Current = First = first;

            public CoupleInCalendar First { get; }

            public CoupleInCalendar Current { get; private set; }

            object IEnumerator.Current => Current;

            public void Dispose() => Current = null;

            public bool MoveNext()
            {
                if (Current.Next == null)
                    return false;
                Current = Current.Next;
                return true;
            }

            public void Reset() => Current = First;
        }
    }
}
