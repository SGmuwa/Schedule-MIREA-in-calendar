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

    /**
     * Класс, который описывает в общем, что общего между.
     * календарной и эксель парой.
     * @since 16.11.2018
     * @version 16.11.2018
     * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
     * @see CoupleInCalendar Пара для календаря
     */
    public class Couple
    {

        /**
         * Название пары.
         */
        public string ItemTitle { get; }
        /**
         * Тип занятия (лекция, практика, лабораторная работа)
         */
        public string typeOfLesson { get; }
        /**
         * Название группы.
         */
        public string NameOfGroup { get; }
        /**
         * Имя преподавателя.
         */
        public string NameOfTeacher { get; set; }
        /**
         * Номер аудитории.
         */
        public string Audience { get; set; }
        /**
         * Адрес корпуса.
         */
        public string Address { get; }

        /**
         * Создание в памяти экземпляров параметров вне-временных параметров пары.
         * @param itemTitle Название пары.
         * @param typeOfLesson Тип занятия (лекция, практика, лабораторная работа)
         * @param nameOfGroup Название группы.
         * @param nameOfTeacher Имя преподавателя.
         * @param audience Номер аудитории.
         * @param address Адрес корпуса.
         */
        protected Couple(string itemTitle, string typeOfLesson, string nameOfGroup, string nameOfTeacher, string audience, string address)
        {
            this.ItemTitle = itemTitle;
            this.typeOfLesson = typeOfLesson;
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
                        object.Equals(typeOfLesson, couple.typeOfLesson) &&
                        object.Equals(NameOfGroup, couple.NameOfGroup) &&
                        object.Equals(NameOfTeacher, couple.NameOfTeacher) &&
                        object.Equals(Audience, couple.Audience) &&
                        object.Equals(Address, couple.Address);
            }
            else return false;
        }

        public override int GetHashCode()
            => HashCode.Combine(ItemTitle, typeOfLesson, NameOfGroup, NameOfTeacher, Audience, Address);
    }
}
