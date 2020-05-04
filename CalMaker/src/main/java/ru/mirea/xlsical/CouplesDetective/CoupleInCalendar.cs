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
using NodaTime;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Структура данных, которая представляет учебную пару в определённый день и время.
    /// Сокращённо: "Календарная пара".
    /// Время начала и конца пары, название группы и имя преподавателя,
    /// название предмета, аудитория, адрес, тип пары.
    /// </summary>
    [System.Serializable]
    public partial class CoupleInCalendar : Couple, IEnumerable<CoupleInCalendar>
    {
        public CoupleInCalendar(string itemTitle, string typeOfLesson, string nameOfGroup, string nameOfTeacher, string audience, string address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple)
        : base(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address)
        {
            this.DateAndTimeOfCouple = dateAndTimeOfCouple;
            this.DateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
        }

        /// <summary>
        /// Конструктор используется для сериализации
        /// </summary>
        /// <param name="itemTitle">Название пары (предмета).</param>
        /// <param name="typeOfLesson">Тип занятия (лекция, практика, лабораторная работа).</param>
        /// <param name="nameOfGroup">Название (номер) группы.</param>
        /// <param name="nameOfTeacher">Имя преподавателя.</param>
        /// <param name="audience">Номер аудитории.</param>
        /// <param name="address">Адрес корпуса.</param>
        /// <param name="dateAndTimeOfCouple">Дата и время пары.</param>
        /// <param name="dateAndTimeFinishOfCouple">Дата и время конца пары.</param>
        /// <param name="next">Указатель на следующий похожий элемент.</param>
        /// <param name="durationToNext"></param>
        public CoupleInCalendar(string itemTitle, string typeOfLesson, string nameOfGroup, string nameOfTeacher, string audience, string address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple, CoupleInCalendar next, Duration durationToNext)
        : base(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address)
        {
            DateAndTimeOfCouple = dateAndTimeOfCouple;
            DateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
            Next = next;
            DurationToNext = durationToNext;
        }
        
        /// <summary>
        /// Дата и время пары.
        /// </summary>
        public readonly ZonedDateTime DateAndTimeOfCouple;
        
        /// <summary>
        /// Дата и время конца пары.
        /// </summary>
        public readonly ZonedDateTime DateAndTimeFinishOfCouple;

        /// <summary>
        /// Указатель на следующий похожий элемент.
        /// </summary>
        private CoupleInCalendar Next = null;

        /// <summary>
        /// Сколько времени должно пройти до следующей такой же пары.
        /// </summary>
        private Duration? DurationToNext = null;

        /// <summary>
        /// Добавляет календарную пару в группу календарных пар.
        /// Требуется для группового редактирования пар.
        /// В случае, если <see cref="Next"/> не равен <code>null</code>, то
        /// будет вызван <code>this.next.add(next)</code>
        /// </summary>
        /// <param name="next">Следующая календарная пара.
        /// Она должна совпадать с <see cref="Couple.Equals(object)"/></param>
        public void Add(CoupleInCalendar next)
        {
            if (next == null)
                throw new System.ArgumentNullException("Argument \"next\" must be not null.");
            if (!base.Equals(next))
                throw new System.ArgumentException("Params \"this\" and \"next\" except dates must be equals!");
            if (this.Next != null)
            {
                this.Next.Add(next);
                return;
            }

            Duration duration = next.DateAndTimeOfCouple - this.DateAndTimeOfCouple;
            if (this.DurationToNext == null)
                this.DurationToNext = duration;
            else if (!DurationToNext.Equals(duration))
                throw new System.ArgumentException("Duration must be equals previous");
            this.Next = next;
            this.Next.DurationToNext = this.DurationToNext;
        }

        public override string ToString()
        => $"CoupleInCalendar{{" +
            $" {nameof(DateAndTimeOfCouple)}='{DateAndTimeOfCouple}'" +
            $", {nameof(DateAndTimeFinishOfCouple)}='{DateAndTimeFinishOfCouple}'" +
            $", {nameof(NameOfGroup)}='{NameOfGroup}'" +
            $", {nameof(NameOfTeacher)}='{NameOfTeacher}'" +
            $", {nameof(ItemTitle)}='{ItemTitle}'" +
            $", {nameof(Audience)}='{Audience}'" +
            $", {nameof(Address)}='{Address}'" +
            $", {nameof(TypeOfLesson)}='{TypeOfLesson}'" +
            $" }}";

        /// <summary>
        /// Отвечает на вопрос, эквивалентен ли этот объект с сравниваемым объектом.
        /// Для сравнения используется:
        /// <see cref="Couple.ItemTitle"/>,
        /// <see cref="Couple.TypeOfLesson"/>,
        /// <see cref="Couple.Audience"/>,
        /// <see cref="DateAndTimeOfCouple"/>,
        /// <see cref="DateAndTimeFinishOfCouple"/>.
        /// В сравнении не участвуют:
        /// <see cref="Couple.NameOfGroup"/>, так как в одной паре может участвовать несколько групп.
        /// <see cref="Couple.NameOfTeacher"/>, так как в одной паре может участвовать несколько преподавателей.
        /// <see cref="Couple.Address"/>, так как я сомневаюсь в эквиваленте в таблицах Excel. То есть я предполагаю, что в Excel таблицах адреса могут отличаться: те, которые default внизу, и те, которые пишутся посреди дня.
        /// </summary>
        /// <param name="o">Объект, с которым надо сравнивать текущий объект.</param>
        /// <returns><code>true</code>, если два объекта совпадают. Иначе — <code>false</code>.</returns>
        public override bool Equals(object o)
        {
            if (this == o) return true;
            if (o is CoupleInCalendar that)
            {
                return
                    DateAndTimeOfCouple.Equals(that.DateAndTimeOfCouple)
                    && DateAndTimeFinishOfCouple.Equals(that.DateAndTimeFinishOfCouple)
                    && ItemTitle.Equals(that.ItemTitle)
                    && TypeOfLesson.Equals(that.TypeOfLesson)
                    && Audience.Equals(that.Audience);
            }
            else return false;
        }

        /// <summary>
        /// Высчитывает хэш-код текущего объекта.
        /// </summary>
        /// <returns>Некоторая числовая маска объекта.</returns>
        /// <seealso cref="Equals(Object)">Какие поля участвуют в генерации хеша?</seealso>
        public override int GetHashCode()
        => DateAndTimeOfCouple.GetHashCode()
            ^ DateAndTimeFinishOfCouple.GetHashCode()
            ^ ItemTitle.GetHashCode()
            ^ TypeOfLesson.GetHashCode()
            ^ Audience.GetHashCode();

        /// <summary>
        /// Получает итератор пар, который проходит по парам, все поля которых
        /// одинаковы, кроме даты времени начала и конца.
        /// </summary>
        /// <returns>Перечисление сходных пар.</returns>
        public IEnumerator<CoupleInCalendar> GetEnumerator() => new CouplesEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
