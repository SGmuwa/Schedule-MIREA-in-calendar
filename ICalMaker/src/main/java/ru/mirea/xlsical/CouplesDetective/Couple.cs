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

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Класс, который описывает в общем, что общего между календарной и Excel парой.
    /// </summary>
    public class Couple
    {
        /// <summary>
        /// Название пары (предмета).
        /// </summary>
        public string ItemTitle { get; }
        
        /// <summary>
        /// Тип занятия (лекция, практика, лабораторная работа).
        /// </summary>
        public string TypeOfLesson { get; }
        
        /// <summary>
        /// Название (номер) группы.
        /// </summary>
        public string NameOfGroup { get; set; }

        /// <summary>
        /// Имя преподавателя.
        /// </summary>
        public string NameOfTeacher { get; set; }
        
        /// <summary>
        /// Номер аудитории.
        /// </summary>
        public string Audience { get; set; }
        
        /// <summary>
        /// Адрес корпуса.
        /// </summary>
        public string Address { get; }

        /// <summary>
        /// Создание в памяти экземпляров параметров вне-временных параметров пары.
        /// </summary>
        /// <param name="itemTitle">Название пары (предмета).</param>
        /// <param name="typeOfLesson">Тип занятия (лекция, практика, лабораторная работа).</param>
        /// <param name="nameOfGroup">Название (номер) группы.</param>
        /// <param name="nameOfTeacher">Имя преподавателя.</param>
        /// <param name="audience">Номер аудитории.</param>
        /// <param name="address">Адрес корпуса.</param>
        protected Couple(string itemTitle, string typeOfLesson, string nameOfGroup, string nameOfTeacher, string audience, string address)
        {
            this.ItemTitle = itemTitle;
            this.TypeOfLesson = typeOfLesson;
            this.NameOfGroup = nameOfGroup;
            this.NameOfTeacher = nameOfTeacher;
            this.Audience = audience;
            this.Address = address;
        }

        public override bool Equals(object o)
        {
            if (o == null) return false;
            if (this == o) return true;
            if (o is Couple couple)
            {
                return object.Equals(ItemTitle, couple.ItemTitle) &&
                        object.Equals(TypeOfLesson, couple.TypeOfLesson) &&
                        object.Equals(NameOfGroup, couple.NameOfGroup) &&
                        object.Equals(NameOfTeacher, couple.NameOfTeacher) &&
                        object.Equals(Audience, couple.Audience) &&
                        object.Equals(Address, couple.Address);
            }
            else return false;
        }

        public override int GetHashCode()
        => HashCode.Combine(ItemTitle, TypeOfLesson, NameOfGroup, NameOfTeacher, Audience, Address);
    }
}
