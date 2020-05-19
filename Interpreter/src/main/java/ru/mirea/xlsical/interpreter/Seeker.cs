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

using NodaTime;

namespace ru.mirea.xlsical.interpreter
{
    /// <summary>
    /// Класс, который представляет из себя искателя.
    /// В этом классе содержатся все поля, которые запрашивает пользователь с интернета.
    /// </summary>
    /// <see cref="Seeker.NameOfSeeker"/>
    /// <see cref="Seeker.DateStart"/>
    /// <see cref="Seeker.DateFinish"/>
    public class Seeker
    {
        /// <summary>
        /// Имя искателя.
        /// Это может быть как имя преподавателя, так и имя группы.
        /// </summary>
        public string NameOfSeeker { get; }

        /// <summary>
        /// Начало составления ICal расписания.
        /// В этот день уже будет составляться расписание.
        /// Указывает на первый день 00:00:00.
        /// </summary>
        public ZonedDateTime DateStart { get; }

        /// <summary>
        /// Дата конца составления ICal.
        /// В этот день будет составлено расписание в последний раз.
        /// Указывает на последний день, последнюю секунду, например, 23:59:59.
        /// </summary>
        public ZonedDateTime DateFinish { get; }

        /// <summary>
        /// Создаёт экземпляр запроса.
        /// </summary>
        /// <param name="nameOfSeeker">
        /// Имя искателя. Это либо имя преподавателя, либо название группы студента.
        /// </param>
        /// <param name="dateStart">
        /// Дата начала составления расписания.
        /// С какого момента времени включительно включительно надо составлять расписание?
        /// </param>
        /// <param name="dateFinish">
        /// Дата конца составления расписания.
        /// До какого момента времени включительно надо составлять расписание?
        /// </param>
        public Seeker(string nameOfSeeker, ZonedDateTime dateStart, ZonedDateTime dateFinish)
            => (NameOfSeeker, DateStart, DateFinish) = (nameOfSeeker, dateStart, dateFinish);

        /// <summary>
        /// Создаёт экземпляр запроса.
        /// </summary>
        /// <param name="nameOfSeeker">
        /// Имя искателя. Это либо имя преподавателя, либо название группы студента.
        /// </param>
        /// <param name="dateStart">
        /// Дата начала составления расписания.
        /// С какого дня включительно включительно надо составлять расписание?
        /// </param>
        /// <param name="dateFinish">
        /// Дата конца составления расписания.
        /// До какого дня включительно надо составлять расписание?
        /// </param>
        /// <param name="timeZone">
        /// Часовой пояс искателя.
        /// </param>
        /// <remarks>
        /// Допустим, искатель живёт в часовом поясне МСК+24.
        /// Искатель задаётся вопросом, какие у него пары с понедельника по пятницу?
        /// У него в календаре отображаются занятия с понедельника по пятницу.
        /// Он смотрит, что в понедельник у него пар нет. Со вторника по пятницу есть.
        /// Но на сервере хранятся пары с понедельника по пятницу. Но из-за смещения
        /// часового пояса, у него все пары сдвинуты на день, и это нормально, так как искатель живёт
        /// по своему часовому поясу.
        /// У него, оказывается, есть пары ещё и в субботу в его часовом поясе. Чтобы их получить, ему надо
        /// запросить пары на субботу включительно.
        /// В то время, пока студент думает, что у него суббота, университет думает, что в стенах университета пятница.
        /// </remarks>
        public Seeker(string nameOfSeeker, LocalDate dateStart, LocalDate dateFinish, DateTimeZone timeZone)
        {
            NameOfSeeker = nameOfSeeker;
            DateStart = timeZone.AtStartOfDay(dateStart);
            DateFinish = timeZone.AtStartOfDay(dateFinish.PlusDays(1)).PlusSeconds(-1);
        }

        public override bool Equals(object ex)
        {
            if (this == ex)
                return true;
            if (ex is Seeker e)
            {
                return NameOfSeeker.Equals(e.NameOfSeeker) &&
                    DateStart.Equals(e.DateStart) &&
                    DateFinish.Equals(e.DateFinish);
            }
            return false;
        }

        public override int GetHashCode()
        => System.HashCode.Combine(NameOfSeeker, DateStart, DateFinish);
    }
}
