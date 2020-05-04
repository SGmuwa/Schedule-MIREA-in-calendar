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


using System.Collections.Generic;

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{
    public static class ISetExtend
    {
        /// <summary>
        /// Добавляет элементы <paramref name="toAdd"/> в набор <paramref name="that"/>.
        /// </summary>
        /// <param name="that">Перечисление, куда надо добавить элементы.</param>
        /// <param name="toAdd">Элементы, которые надо добавить в набор.</param>
        /// <returns>Количество успешно добавленных элементов.</returns>
        public static int AddRange<T, D>(this ISet<T> that, IEnumerable<D> toAdd) where D : T
        {
            int countAdd = 0;
            foreach (D item in toAdd)
                if (that.Add(item))
                    countAdd++;
            return countAdd;
        }
    }
}