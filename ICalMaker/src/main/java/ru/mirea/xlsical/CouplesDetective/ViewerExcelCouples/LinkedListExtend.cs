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

using System;
using System.Collections.Generic;

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{
    public static class LinkedListExtend
    {
        /// <summary>
        /// Добавляет всё перечисление в конец связанного списка.
        /// </summary>
        /// <param name="that">Куда надо добавить элементы?</param>
        /// <param name="toAdd">Перечисление элементов для добавления.</param>
        public static void AddLastRange<T, D>(this LinkedList<T> that, IEnumerable<D> toAdd) where D : T
        {
            foreach (D item in toAdd)
                that.AddLast(item);
        }

        /// <summary>
        /// Удаляет все элементы из связного списка по предикату.
        /// </summary>
        /// <param name="that">Лист, из которого надо удалить элементы.</param>
        /// <param name="predicate">Предикат, который возвращает <code>true</code>,
        /// если входящий элемент необходимо удалить. Иначе возвращает <code>false</code>.</param>
        public static void RemoveAll<T>(this LinkedList<T> that, Func<T, bool> predicate)
        {
            for (LinkedListNode<T> node = that.First; node != null;)
            {
                LinkedListNode<T> next = node.Next;
                if (predicate(node.Value))
                    that.Remove(node);
                node = next;
            }
        }
    }
}