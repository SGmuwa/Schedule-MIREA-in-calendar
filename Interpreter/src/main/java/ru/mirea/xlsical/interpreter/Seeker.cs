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

/*
[SG]Muwa Михаил Павлович Сидоренко. 2018.
Файл представляет из себя хранилище запроса.
Суть запроса: имя искателя, тип искателя (преподаватель, студент), дата начала и конца семестра, адрес по-умолчанию.
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
        /// <param name="nameOfSeeker">Имя искателя. Это либо имя преподавателя, либо название группы студента.</param>
        /// <param name="dateStart">Дата начала составления расписания. С какого календарного дня надо составлять расписание?</param>
        /// <param name="dateFinish">Дата конца составления расписания. До какого календарного дня надо составлять расписание?</param>
        /// <param name="needReset"></param>
        public Seeker(string nameOfSeeker, ZonedDateTime dateStart, ZonedDateTime dateFinish, bool needReset = true)
        {
            this.NameOfSeeker = nameOfSeeker;
            this.DateStart = needReset ? new ZonedDateTime(dateStart.Date.At(LocalTime.MinValue), dateStart.Zone, dateStart.Offset) : dateStart;
            this.DateFinish = needReset ? new ZonedDateTime(dateFinish.Date.PlusDays(1).At(LocalTime.MinValue).Minus(Period.FromSeconds(1)), dateFinish.Zone, dateFinish.Offset) : dateFinish;
        }

        /// <summary>
        /// Создаёт экземпляр запроса.
        /// </summary>
        /// <param name="nameOfSeeker">Имя искателя. Это либо имя преподавателя, либо название группы студента.</param>
        /// <param name="dateStart">Дата начала составления расписания. С какого календарного дня надо составлять расписание? Дата указывается по местному времени <code>timezoneStart</code>.</param>
        /// <param name="dateFinish">Дата конца составления расписания. До какого календарного дня надо составлять расписание? Дата указывается по местному времени <code>timezoneStart</code>.</param>
        /// <param name="timeZoneStart">Часовой пояс, где будут пары.</param>
        public Seeker(string nameOfSeeker, LocalDate dateStart, LocalDate dateFinish, DateTimeZone timeZoneStart)
        : this(nameOfSeeker, timeZoneStart.AtStartOfDay(dateStart), timeZoneStart.AtStartOfDay(dateFinish))
        {
            /*
            Отправлять в конструктор ZonedDateTime не безопасно, так как тогда
            студент или преподаватель могут получить расписание не целиком за день, а
            только части дня. Лучше сокрыть данный функционал, чтобы пользователи
            получали данные в промежуток включая целиком учебные дни.
            Необходимо преобразовать LocalDate и ZoneId в указатели времени,
            чтобы не было проблем с точностью.
            С датой начала нет проблем: указываем на самую первую минуту дня.
            С датой конца проблема: не гарантировано, что в дне строго 60*60*24 секунд.
            Также по описанию данного класса, последний день надо включить.
            Просто прибавить один день и указать до 00:00:00 нельзя, так как при преобразовании
            в LocalDate будет покрываться следующий день.
            Поэтому надо прибавить к dateFinish один день и отнять одну секунду, таким
            образом возложив расчёты по определению "последнего момента времени дня" на
            алгоритмы java.time или их потомков.
             */
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
