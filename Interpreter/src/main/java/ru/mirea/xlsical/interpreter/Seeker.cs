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

namespace ru.mirea.xlsical.interpreter
{
    /// <summary>
    /// Класс, который представляет из себя искателя.
    /// В этом классе содержатся все поля, которые запрашивает пользователь с интернета.
    /// </summary>
    /// <see cref="Seeker.NameOfSeeker"></see>
    /// <see cref="Seeker.dateStart"></see>
    /// <see cref="Seeker.dateFinish"></see>
    public readonly class Seeker
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
        public DateTimeOffset dateStart { get; }

        /// <summary>
        /// Дата конца составления ICal.
        /// В этот день будет составлено расписание в последний раз.
        /// Указывает на последний день, последнюю секунду, например, 23:59:59.
        /// </summary>
        public DateTimeOffset dateFinish { get; }
        
        /**
         * Первоначальный номер недели. По-умолчанию указывать = 1.
         * @deprecated Не используется. Вместо этого идёт выборка данных.
         */
        public readonly int startWeek { get; }

        /**
         * Создаёт экземпляр запроса.
         * @param nameOfSeeker Имя искателя. Это либо имя преподавателя, либо название группы студента.
         * @param dateStart Дата начала составления расписания. С какого календарного дня надо составлять расписание? Дата указывается по местному времени.
         * @param dateFinish Дата конца составления расписания. До какого календарного дня надо составлять расписание? Дата указывается по местному времени.
         * @param timezoneStart Часовой пояс, где будут пары. Это значение в начале семестра.
         * @param startWeek Первоначальный номер недели. По-умолчанию указывать = 1.
         * @deprecated Поля {@link #defaultAddress}, {@link #seekerType}, {@link #startWeek}, {@link #timezoneStart} уже не используется.
         *             Тип {@code dateStart} и {@code dateFinish} изменены с {@link java.time.LocalDate} на {@link ZonedDateTime}.
         */
        public Seeker(string nameOfSeeker, DateTimeOffset dateStart, DateTimeOffset dateFinish, int startWeek)
        {
            this.nameOfSeeker = nameOfSeeker;
            this.dateStart = ZonedDateTime.of(LocalDateTime.of(dateStart, LocalTime.of(0, 0)), timezoneStart);
            this.dateFinish = ZonedDateTime.of(LocalDateTime.of(dateFinish, LocalTime.of(0, 0)), timezoneStart).plus(1, ChronoUnit.DAYS).minus(1, ChronoUnit.SECONDS);
            this.startWeek = startWeek;
        }

        /**
         * Создаёт экземпляр запроса.
         * @param nameOfSeeker Имя искателя. Это либо имя преподавателя, либо название группы студента.
         * @param dateStart Дата начала составления расписания. С какого календарного дня надо составлять расписание? Дата указывается по местному времени.
         * @param dateFinish Дата конца составления расписания. До какого календарного дня надо составлять расписание? Дата указывается по местному времени.
         * @param timezoneStart Часовой пояс, где будут пары. Это значение в начале семестра.
         */
        public Seeker(String nameOfSeeker, LocalDate dateStart, LocalDate dateFinish, ZoneId timezoneStart)
        {
            this.nameOfSeeker = nameOfSeeker;
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
            this.dateStart = ZonedDateTime.of(LocalDateTime.of(dateStart, LocalTime.of(0, 0)), timezoneStart);
            this.dateFinish = ZonedDateTime.of(LocalDateTime.of(dateFinish, LocalTime.of(0, 0)), timezoneStart).plus(1, ChronoUnit.DAYS).minus(1, ChronoUnit.SECONDS);
            this.startWeek = 1;
        }

        public override bool Equals(Object ex)
        {
            if (this == ex)
                return true;
            if (ex is Seeker e)
            {
                return nameOfSeeker.equals(e.nameOfSeeker) &&
                    dateStart.equals(e.dateStart) &&
                    dateFinish.equals(e.dateFinish);
            }
            return false;
        }

        static Seeker()
        {
            int startWeek = 0;

            /*
             * Для поддержки старых устройств необходим часовой пояс "UTC+03:00" [1],
             * так как не все устройства[2] приняли обновления от 26 октября 2014 [3].
             * Поменять на Europe/Moscow 15 августа 2020 году.
             * Необходимо проверить, поддерживается ли UTC+03:00?
             *
             * [1][3] http://www.consultant.ru/document/cons_doc_LAW_114656/b2707989c276b5a188e63bc41e7bcbcc18723de8/
             * [2] https://dentnt.windowsfaq.ru/?p=1527
             */
            ZoneId zoneId = ZoneId.of("UTC+03:00");


            ZonedDateTime start = ZonedDateTime.now(zoneId);
            // Дольше же восьми лет не может учиться студент?
            ZonedDateTime finish = start.plus(8, ChronoUnit.YEARS);



            Instance = new Seeker(
                    // Штука, которая расчитывает группу по-умолчанию.
                    "ИКБО-04-" + start.minus(6L, ChronoUnit.MONTHS).getLong(ChronoField.YEAR) % 100,
                    start.toLocalDate(),
                    finish.toLocalDate(),
                    zoneId
            );
        }

        /// <summary>
        /// Искатель по-умолчанию.
        /// </summary>
        public static Seeker Instance { get; }
    }
}
