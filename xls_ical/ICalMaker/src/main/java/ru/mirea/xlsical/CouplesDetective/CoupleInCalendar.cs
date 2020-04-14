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

package ru.mirea.xlsical.CouplesDetective;

import java.io.Serializable;
import java.time.*;
import java.util.Iterator;
import java.util.NoSuchElementException;


/**
 * Структура данных, которая представляет учебную пару в определённый день и время.
 * Сокращённо: "Календарная пара".
 * Время начала и конца пары, название группы и имя преподавателя,
 * название предмета, аудитория, адрес, тип пары.
 */
public class CoupleInCalendar extends Couple implements Iterable<CoupleInCalendar>, Serializable {

    public CoupleInCalendar(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple) {
        super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
        this.dateAndTimeOfCouple = dateAndTimeOfCouple;
        this.dateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
    }

    /**
     * Конструктор используется для сериализации
     */
    public CoupleInCalendar(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple, CoupleInCalendar next, Duration durationToNext) {
        super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
        this.dateAndTimeOfCouple = dateAndTimeOfCouple;
        this.dateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
        this.next = next;
        this.durationToNext = durationToNext;
    }

    /**
     * Дата и время пары.
     */
    public final ZonedDateTime dateAndTimeOfCouple;
    /**
     * Количество времени, сколько длится пара.
     */
    public final ZonedDateTime dateAndTimeFinishOfCouple;

    /**
     * Указатель на следующий похожий элемент.
     */
    private CoupleInCalendar next = null;

    private Duration durationToNext = null;

    /**
     * Добавляет календарную пару в группу календарных пар.
     * Требуется для группового редактирования пар.
     * В случае, если {@link #next} не равен {@code null}, то
     * будет вызван {@code this.next.add(next)}
     * @param next Следующая календарная пара.
     *             Она должна совпадать с {@link Couple#equals(Object) super.equals(Object)}
     */
    public void add(CoupleInCalendar next) {
        if (next == null)
            throw new NullPointerException("Argument \"next\" must be not null.");
        if (!super.equals(next))
            throw new IllegalArgumentException("Params \"this\" and \"next\" except dates must be equals!");
        if (this.next != null) {
            this.next.add(next);
            return;
        }

        Duration duration = Duration.between(this.dateAndTimeOfCouple, next.dateAndTimeOfCouple);
        if (this.durationToNext == null)
            this.durationToNext = duration;
        else if (!durationToNext.equals(duration))
            throw new IllegalArgumentException("Duration must be equals previous");
        this.next = next;
        this.next.durationToNext = this.durationToNext;
    }

    @Override
    public String toString() {
        return "CoupleInCalendar{" +
                "dateAndTimeOfCouple=" + dateAndTimeOfCouple +
                ", dateAndTimeFinishOfCouple=" + dateAndTimeFinishOfCouple +
                ", nameOfGroup='" + nameOfGroup + '\'' +
                ", nameOfTeacher='" + nameOfTeacher + '\'' +
                ", itemTitle='" + itemTitle + '\'' +
                ", audience='" + audience + '\'' +
                ", address='" + address + '\'' +
                ", typeOfLesson='" + typeOfLesson + '\'' +
                '}';
    }

    /**
     * Отвечат на вопрос, эквивалентен ли этот объект с сравниваемым объектом.
     * Для сравнения используется:
     * <ul type="disc">
     *     <li>{@link #itemTitle},</li>
     *     <li>{@link #typeOfLesson},</li>
     *     <li>{@link #audience},</li>
     *     <li>{@link #dateAndTimeOfCouple},</li>
     *     <li>{@link #dateAndTimeFinishOfCouple}.</li>
     * </ul>
     * В сравнении не учавствуют:
     * <ul type="disc">
     *     <li>{@link #nameOfGroup}, так как в одной паре может учавствовать несколько групп.</li>
     *     <li>{@link #nameOfTeacher}, так как в одной паре может учавствовать несколько преподавателей.</li>
     *     <li>{@link #address}, так как я сомневаюсь в эквиваленте в таблицах Excel.
     *         То есть я предполагаю, что в Excel таблицах адреса могут отличаться:
     *         те, которые default внизу, и те, которые пишутся посреди дня.</li>
     * </ul>
     * @param o Объект, с которым надо сравнивать текущий объект.
     * @return {@code True}, если два объекта совпадают. Иначе - {@code false}.
     */
    @Override
    public boolean equals(Object o) {
        if (this == o) return true;
        if (!(o instanceof CoupleInCalendar)) return false;
        CoupleInCalendar that = (CoupleInCalendar) o;
        return
                dateAndTimeOfCouple.equals(that.dateAndTimeOfCouple)
                        && dateAndTimeFinishOfCouple.equals(that.dateAndTimeFinishOfCouple)
                        && itemTitle.equals(that.itemTitle)
                        && typeOfLesson.equals(that.typeOfLesson)
                        && audience.equals(that.audience);
    }

    /**
     * Высчитывает хэш-код текущего объекта.
     * @return Некотарая числовая маска объекта.
     * @see #equals(Object) Какие поля учавствуют в генерации хэша?
     */
    @Override
    public int hashCode() {
        return dateAndTimeOfCouple.hashCode()
                ^ dateAndTimeFinishOfCouple.hashCode()
                ^ itemTitle.hashCode()
                ^ typeOfLesson.hashCode()
                ^ audience.hashCode();
    }

    /**
     * Получает итератор пар, который проходит по парам, все поля которых
     * одинаковы, кроме даты времени начала и конца.
     *
     * @return an Iterator.
     */
    @Override
    public Iterator<CoupleInCalendar> iterator() {
        return new CouplesIterator(this);
    }

    /**
     * Класс, который реализует переход к следующим календарным парам.
     */
    private class CouplesIterator implements Iterator<CoupleInCalendar> {

        /**
         * Указатель на элемент, который надо будет вернуть.
         */
        private CoupleInCalendar current;

        public CouplesIterator(CoupleInCalendar first) {
            current = first;
        }

        /**
         * Returns {@code true} if the iteration has more elements.
         * (In other words, returns {@code true} if {@link #current} would
         * return an element rather than throwing an exception.)
         *
         * @return {@code true} if the iteration has more elements
         */
        @Override
        public boolean hasNext() {
            return current != null;
        }

        /**
         * Returns the current element in the iteration.
         *
         * @return the current element in the iteration
         * @throws NoSuchElementException if the iteration has no more elements
         */
        @Override
        public CoupleInCalendar next() {
            if(!hasNext())
                throw new NoSuchElementException();
            CoupleInCalendar out = current;
            current = current.next;
            return out;
        }
    }
}
