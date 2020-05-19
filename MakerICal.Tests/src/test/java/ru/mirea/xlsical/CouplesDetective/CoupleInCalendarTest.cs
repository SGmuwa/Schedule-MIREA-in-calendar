/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)
    Marina Romanovna Kuzmina

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
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;
using Xunit;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Тестирование арифметических операций над учебными парами, тестирование подсчёта.
    /// </summary>
    public class CoupleInCalendarTest
    {
        /// <summary>
        /// Тестирование, если у нас одна пара в один день.
        /// </summary>
        [Fact]
        public void startTestOneCouple()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+03:00"]; // GMT+3:00

            ZonedDateTime startSemester = timezone.AtStartOfDay(new LocalDate(2017, (int)IsoMonth.December, 31));
            ZonedDateTime finishSemester = timezone.AtStartOfDay(new LocalDate(2018, (int)IsoMonth.January, 2));

            LocalTime time1 = new LocalTime(0, 1, 0);
            LocalTime time2 = new LocalTime(0, 3, 0); // Пара длится 2 минуты

            IsoDayOfWeek day = IsoDayOfWeek.Monday;

            string nGr = "Группа-01 32";
            string nam = "Математика и инженерия."; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Лк";
            string tic = "Иванов В.В.";
            string add = "Москва, проспект Вернадского 78, РТУ МИРЭА";
            string aud = "А-1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(startSemester, finishSemester, 1, time1, time2, timezone, day, false, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            /* Количество */
            Assert.Equal(1, @out.Count);
            ZonedDateTime zonedDateTimeDateTime1 = DateTimeZone.Utc.AtStrictly(new LocalDate(2018, (int)IsoMonth.January, 1).At(time1));
            ZonedDateTime zonedDateTimeDateTime2 = DateTimeZone.Utc.AtStrictly(new LocalDate(2018, (int)IsoMonth.January, 1).At(time2));
            /* Время начала пары */
            Assert.Equal(zonedDateTimeDateTime1, @out[0].DateAndTimeOfCouple);
            /* Время конца пары */
            Assert.Equal(zonedDateTimeDateTime2, @out[0].DateAndTimeFinishOfCouple);
            /* Название группы */
            Assert.Equal(nGr, @out[0].NameOfGroup);
            /* Название предмета */
            Assert.Equal(nam, @out[0].ItemTitle);
            /* Тип пары */
            Assert.Equal(typ, @out[0].TypeOfLesson);
            /* Имя преподавателя*/
            Assert.Equal(tic, @out[0].NameOfTeacher);
            /* Адрес кампуса */
            Assert.Equal(add, @out[0].Address);
            /* Аудитория */
            Assert.Equal(aud, @out[0].Audience);
        }

        /// <summary>
        /// Тестирование, если у нас одна пара в один день. Используется иное Timezone.
        /// </summary>
        [Fact]
        public void startTestOneCoupleTimezone()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+03:00"]; // GMT+3:00

            ZonedDateTime start = new LocalDate(2017, (int)IsoMonth.December, 31).AtStartOfDayInZone(timezone);
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.January, 2).AtStartOfDayInZone(timezone);

            LocalTime time1 = new LocalTime(0, 1, 0);
            LocalTime time2 = new LocalTime(0, 3, 0); // Пара длится 2 минуты

            IsoDayOfWeek day = IsoDayOfWeek.Monday;


            string nGr = "Группа-01 32";
            string nam = "Математика и инженерия."; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Лк";
            string tic = "Иванов В.В.";
            string add = "Москва, проспект Вернадского 78, РТУ МИРЭА";
            string aud = "А-1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, false, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            /* Количество */
            Assert.Equal(1, @out.Count);
            ZonedDateTime zonedDateTime = timezone.AtStrictly(new LocalDate(2018, (int)IsoMonth.January, 1).At(new LocalTime(0, 1, 0)));
            ZonedDateTime zonedDateTimeDateTime2 = timezone.AtStrictly(new LocalDate(2018, (int)IsoMonth.January, 1).At(time2));
            /* Время начала пары */
            Assert.Equal(zonedDateTime, @out[0].DateAndTimeOfCouple);
            /* Время конца пары */
            Assert.Equal(zonedDateTimeDateTime2, @out[0].DateAndTimeFinishOfCouple);
            /* Название группы */
            Assert.Equal(nGr, @out[0].NameOfGroup);
            /* Название предмета */
            Assert.Equal(nam, @out[0].ItemTitle);
            /* Тип пары */
            Assert.Equal(typ, @out[0].TypeOfLesson);
            /* Имя преподавателя*/
            Assert.Equal(tic, @out[0].NameOfTeacher);
            /* Адрес кампуса */
            Assert.Equal(add, @out[0].Address);
            /* Аудитория */
            Assert.Equal(aud, @out[0].Audience);
        }

        /// <summary>
        /// Тестирование, если у нас одна пара на один конкретный день дней.
        /// </summary>
        [Fact]
        public void startTestOneHardCouple()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+00:00"]; // GMT+0:00

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);

            LocalTime time1 = new LocalTime(0, 1, 0);
            LocalTime time2 = new LocalTime(0, 3, 0); // Пара длится 2 минуты

            IsoDayOfWeek day = IsoDayOfWeek.Monday;

            string nGr = "Группа-24 32";
            string nam = "Русский jaj номер Нан 1 4 2."; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Лаб.";
            string tic = "Момов А.А.";
            string add = "Україна, Київ, Центральна 8";
            string aud = "202";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, true, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            /* Количество */
            Assert.Equal(1, @out.Count);
            ZonedDateTime zonedDateTime = timezone.AtStrictly(new LocalDate(2018, (int)IsoMonth.January, 1).At(new LocalTime(0, 1, 0)));
            ZonedDateTime zonedDateTimeDateTime2 = timezone.AtStrictly(new LocalDate(2018, (int)IsoMonth.January, 1).At(time2));
            /* Время начала пары */
            Assert.Equal(zonedDateTime, @out[0].DateAndTimeOfCouple);
            /* Время конца пары */
            Assert.Equal(zonedDateTimeDateTime2, @out[0].DateAndTimeFinishOfCouple);
            /* Название группы */
            Assert.Equal(nGr, @out[0].NameOfGroup);
            /* Название предмета */
            Assert.Equal(nam, @out[0].ItemTitle);
            /* Тип пары */
            Assert.Equal(typ, @out[0].TypeOfLesson);
            /* Имя преподавателя*/
            Assert.Equal(tic, @out[0].NameOfTeacher);
            /* Адрес кампуса */
            Assert.Equal(add, @out[0].Address);
            /* Аудитория */
            Assert.Equal(aud, @out[0].Audience);
        }

        /**
         * Тестирование, если у нас одна пара в расписании в течение 4 недель. Итого за 4 недели у нас 2 пары.
         */
        [Fact]
        public void startTestOneCoupleTwoDays()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["UTC+00:00"]; // UTC+0:00

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1 /*+ 1*/).AtStartOfDayInZone(timezone); // Протестируем вторник.
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.January, 1 + 7 * 4 + 1).AtStartOfDayInZone(timezone); // 29 + 1 = 30

            LocalTime time1 = new LocalTime(9, 0, 0);
            LocalTime time2 = new LocalTime(10, 30, 0);

            IsoDayOfWeek day = IsoDayOfWeek.Tuesday;

            string nGr = "Группа-,";
            string nam = "Математика и н. значения .pgt ju340)(HG(fvh gvh"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Пр@кти ка";
            string tic = "В.В. Иванов";
            string add = "Москва, проспект Вернадского 78, РТУ МИРЭА";
            string aud = "А-1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, true, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            /* Количество */
            Assert.Equal(3, @out.Count);
            /* Время начала пары*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.January, 2).At(new LocalTime(9, 0, 0)).InZoneStrictly(timezone), @out[0].DateAndTimeOfCouple);
            /* Время начала пары*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.January, 2).At(new LocalTime(9, 0, 0)).InZoneStrictly(timezone).PlusWeeks(2), @out[1].DateAndTimeOfCouple);
            /* Время начала пары*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.January, 2).At(new LocalTime(9, 0, 0)).InZoneStrictly(timezone).PlusWeeks(2 * 2), @out[2].DateAndTimeOfCouple);

            /* Время конца пары */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.January, 2).At(time2).InZoneStrictly(timezone), @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.January, 2).At(time2).InZoneStrictly(timezone).PlusWeeks(2), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.January, 2).At(time2).InZoneStrictly(timezone).PlusWeeks(2 * 2), @out[2].DateAndTimeFinishOfCouple);
            foreach (CoupleInCalendar o in @out)
            {
                /* Название группы */
                Assert.Equal(nGr, o.NameOfGroup);
                /* Название предмета */
                Assert.Equal(nam, o.ItemTitle);
                /* Тип пары */
                Assert.Equal(typ, o.TypeOfLesson);
                /* Имя преподавателя*/
                Assert.Equal(tic, o.NameOfTeacher);
                /* Адрес кампуса */
                Assert.Equal(add, o.Address);
                /* Аудитория */
                Assert.Equal(aud, o.Audience);
            }
        }

        /// <summary>
        /// Тестирование, если у нас одна пара в расписании в течение 4 месяцев. Итого за 4 месяца у нас полно пар раз в две недели.
        /// </summary>
        [Fact]
        public void StartTestOneCoupleDuring4Month()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+00:00"]; // GMT+0:00

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.April, start.Calendar.GetDaysInMonth(2018, (int)IsoMonth.April)).AtStartOfDayInZone(timezone); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.Wednesday; // Среда

            string nGr = "АБВГ-01-ГА";
            string nam = ",vrihjegijrw093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н3ен.е45..5н54.542FPQWQ#@(-)@(#)$oqfk"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Лабораторная работа.";
            string tic = "ГГГГгггггг. А. а.";
            string add = "ВОдичка";
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, true, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            ZonedDateTime firstGood = new LocalDate(2018, (int)IsoMonth.January, 3).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone);

            /* Количество */
            Assert.Equal(9, @out.Count);
            /* Время начала пары 1*/
            Assert.Equal(firstGood, @out[0].DateAndTimeOfCouple);
            /* Время начала пары 2*/
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 3*/
            Assert.Equal(firstGood.PlusWeeks(2 * 2), @out[2].DateAndTimeOfCouple);
            /* Время начала пары 4*/
            Assert.Equal(firstGood.PlusWeeks(3 * 2), @out[3].DateAndTimeOfCouple);
            /* Время начала пары 5*/
            Assert.Equal(firstGood.PlusWeeks(4 * 2), @out[4].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(firstGood.PlusWeeks(5 * 2), @out[5].DateAndTimeOfCouple);
            /* Время начала пары 7*/
            Assert.Equal(firstGood.PlusWeeks(6 * 2), @out[6].DateAndTimeOfCouple);
            /* Время начала пары 8*/
            Assert.Equal(firstGood.PlusWeeks(7 * 2), @out[7].DateAndTimeOfCouple);
            /* Время начала пары 9*/
            Assert.Equal(firstGood.PlusWeeks(8 * 2), @out[8].DateAndTimeOfCouple);

            firstGood = new LocalDate(2018, (int)IsoMonth.January, 3).At(time2).InZoneStrictly(timezone);

            /* Время конца пары 1 */
            Assert.Equal(firstGood, @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 2 */
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары 3 */
            Assert.Equal(firstGood.PlusWeeks(2 * 2), @out[2].DateAndTimeFinishOfCouple);
            /* Время конца пары 4 */
            Assert.Equal(firstGood.PlusWeeks(3 * 2), @out[3].DateAndTimeFinishOfCouple);
            /* Время конца пары 5 */
            Assert.Equal(firstGood.PlusWeeks(4 * 2), @out[4].DateAndTimeFinishOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(firstGood.PlusWeeks(5 * 2), @out[5].DateAndTimeFinishOfCouple);
            /* Время конца пары 7 */
            Assert.Equal(firstGood.PlusWeeks(6 * 2), @out[6].DateAndTimeFinishOfCouple);
            /* Время конца пары 8 */
            Assert.Equal(firstGood.PlusWeeks(7 * 2), @out[7].DateAndTimeFinishOfCouple);
            /* Время конца пары 9 */
            Assert.Equal(firstGood.PlusWeeks(8 * 2), @out[8].DateAndTimeFinishOfCouple);


            foreach (CoupleInCalendar o in @out)
            {
                /* Название группы */
                Assert.Equal(nGr, o.NameOfGroup);
                /* Название предмета */
                Assert.Equal(nam, o.ItemTitle);
                /* Тип пары */
                Assert.Equal(typ, o.TypeOfLesson);
                /* Имя преподавателя*/
                Assert.Equal(tic, o.NameOfTeacher);
                /* Адрес кампуса */
                Assert.Equal(add, o.Address);
                /* Аудитория */
                Assert.Equal(aud, o.Audience);
            }
        }

        /**
         * Тестирование одной пары на четыре месяца по чётным неделям.
         */
        [Fact]
        public void startTestOneCoupleDuring4MonthInEvenWeek()
        {

            LocalDate start = new LocalDate(2018, (int)IsoMonth.January, 1);
            LocalDate finish = new LocalDate(2018, (int)IsoMonth.APRIL, YearMonth.of(2018, (int)IsoMonth.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.THURSDAY; // Четверг

            DateTimeZone timezone = DateTimeZone.of("GMT+00:00"); // GMT+0:00

            string nGr = "АБВГ-01-ГА";
            string nam = ",vrihjegijrw093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н3ен.е45..5н54.542FPQWQ#@(-)@(#)$oqfk"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Лабораторная работа.";
            string tic = "ГГГГгггггг. А. а.";
            string add = "ВОдичка";
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, timezone, 1, time1, time2, day, false, nam, typ, nGr, tic, aud, add);

            /* Количество */
            Assert.Equal(8, @out.Count);

            Assert.NotNull(@out);

            /* Время начала пары 1*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(0, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[0].DateAndTimeOfCouple);
            /* Время начала пары 2*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 3*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 2, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[2].DateAndTimeOfCouple);
            /* Время начала пары 4*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 3, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[3].DateAndTimeOfCouple);
            /* Время начала пары 5*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 4, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[4].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 5, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[5].DateAndTimeOfCouple);
            /* Время начала пары 7*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 6, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[6].DateAndTimeOfCouple);
            /* Время начала пары 8*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 7, ChronoUnit.WEEKS), new LocalTime(10, 40, 0)), timezone), @out[7].DateAndTimeOfCouple);

            /* Время конца пары 1 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(0, ChronoUnit.WEEKS), time2), timezone), @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 2 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2, ChronoUnit.WEEKS), time2), timezone), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары 3 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 2, ChronoUnit.WEEKS), time2), timezone), @out[2].DateAndTimeFinishOfCouple);
            /* Время конца пары 4 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 3, ChronoUnit.WEEKS), time2), timezone), @out[3].DateAndTimeFinishOfCouple);
            /* Время конца пары 5 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 4, ChronoUnit.WEEKS), time2), timezone), @out[4].DateAndTimeFinishOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 5, ChronoUnit.WEEKS), time2), timezone), @out[5].DateAndTimeFinishOfCouple);
            /* Время конца пары 7 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 6, ChronoUnit.WEEKS), time2), timezone), @out[6].DateAndTimeFinishOfCouple);
            /* Время конца пары 8 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11).plus(2 * 7, ChronoUnit.WEEKS), time2), timezone), @out[7].DateAndTimeFinishOfCouple);
            for (CoupleInCalendar o : @out)
            {
                /* Название группы */
                Assert.Equal(nGr, o.NameOfGroup);
                /* Название предмета */
                Assert.Equal(nam, o.ItemTitle);
                /* Тип пары */
                Assert.Equal(typ, o.TypeOfLesson);
                /* Имя преподавателя*/
                Assert.Equal(tic, o.NameOfTeacher);
                /* Адрес кампуса */
                Assert.Equal(add, o.Address);
                /* Аудитория */
                Assert.Equal(aud, o.Audience);
            }
        }

        /**
         * Данный тест проверяет, правильно ли программа отвечает на вопрос,
         * есть ли в записи предмета информации об исключениях датах.
         * А именно на каких неделях есть пары, или на каких недлях пар нет.
         */
        [Fact]
        public void startTestRex()
        {

            /*
            Beginner
             */

            assertTrue(
                    Pattern.compile("^[a-z0-9_-]{3,15}$") // Сюда пишется регулярное выражение
                            .matcher("vovan") // Здесь пишем проверяемый текст, который надо сравнить с регулярным выражением.
                            .matches() // Отправляем команду "сравнить". True, если текст совпадает с регулярным выражением.
            );

            assertFalse(
                    Pattern.compile("^[a-z0-9_-]{3,15}$")
                            .matcher("_@BEST").matches()
            );

            // --------

            assertTrue(
                    Pattern.compile("^.+w\\.?.+$")
                            .matcher("1 w. 1").matches()
            );

            assertTrue(
                    Pattern.compile("^.+н\\.?.+$")
                            .matcher("1 н. 1").matches()
            );

            assertTrue(
                    Pattern.compile(".+н\\.?.+")
                            .matcher("1 н. 1").matches()
            );

            assertFalse(
                    Pattern.compile("н\\.?")
                            .matcher("1 н. 1").matches()
            );

            assertTrue(
                    Pattern.compile("(^.+\\s[нН]\\.?.+$)")
                            .matcher("1 н. 1").matches()
            );

            assertFalse(
                    Pattern.compile("(^.+\\s[нН]\\.?.+$)|()")
                            .matcher("1 н. 1\n").matches()
            );

            // -------- ^J


            assertArrayEquals(new Integer[] { 1 }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("1 н. 1").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("кр 5 н Логика").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("кр. 5 н. Логика").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("Внешний и внутренний PR").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("Дискретная математика").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { 11, 13, 15, 17 }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("11,13,15,17 н. Правоведение").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { 11, 13, 15, 17 }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("11,13,15,17 н Правоведение").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { }, DetectiveSemester.SetterCouplesInCalendar.getAllOnlyWeeks("История Неполита").toArray(new Integer[0]));

            assertArrayEquals(new Integer[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.getAllExceptionWeeks("кр 5 н Логика").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.getAllExceptionWeeks("кр. 5 н. Логика").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { }, DetectiveSemester.SetterCouplesInCalendar.getAllExceptionWeeks("Внешний и внутренний PR").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { }, DetectiveSemester.SetterCouplesInCalendar.getAllExceptionWeeks("Дискретная математика").toArray(new Integer[0]));
            assertArrayEquals(new Integer[] { }, DetectiveSemester.SetterCouplesInCalendar.getAllExceptionWeeks("11,13,15,17 н. Правоведение").toArray(new Integer[0]));


        }

        /**
         * Тестирование одной пары на четыре месяца по чётным неделям, а именно на 2, 4 и 8.
         */
        [Fact]
        public void startTestOneCoupleDuring4MonthInSomeWeek()
        {

            LocalDate start = new LocalDate(2018, (int)IsoMonth.January, 1);
            LocalDate finish = new LocalDate(2018, (int)IsoMonth.APRIL, YearMonth.of(2018, (int)IsoMonth.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.THURSDAY; // Четверг

            DateTimeZone timezone = DateTimeZone.of("Europe/Moscow"); // Moscow

            // Имя группы.
            string nGr = "АБВГ-01-ГА";
            // Название предмета.
            string nam = "Игрообразование 2, 4 и 8 н.";
            // Тип пары.
            string typ = "Лабораторная работа.";
            // Учитель.
            string tic = "ГГГГгггггг. А. а.";
            // Адрес филиала.
            string add = "ВОдичка";
            // Аудитория.
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, timezone, 1, time1, time2, day, false, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            /* Количество */
            Assert.Equal(3, @out.Count);
            /* Время начала пары 1*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11), new LocalTime(10, 40, 0)), timezone), @out[0].DateAndTimeOfCouple);
            /* Время начала пары 2*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 25), new LocalTime(10, 40, 0)), timezone), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 4*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.FEBRUARY, 22), new LocalTime(10, 40, 0)), timezone), @out[2].DateAndTimeOfCouple);

            /* Время конца пары 1*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 11), time2), timezone), @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 2*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.January, 25), time2), timezone), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары 4*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.FEBRUARY, 22), time2), timezone), @out[2].DateAndTimeFinishOfCouple);

            for (CoupleInCalendar o : @out)
            {
                /* Название группы */
                Assert.Equal(nGr, o.NameOfGroup);
                /* Название предмета */
                Assert.Equal(nam, o.ItemTitle);
                /* Тип пары */
                Assert.Equal(typ, o.TypeOfLesson);
                /* Имя преподавателя*/
                Assert.Equal(tic, o.NameOfTeacher);
                /* Адрес кампуса */
                Assert.Equal(add, o.Address);
                /* Аудитория */
                Assert.Equal(aud, o.Audience);
            }
        }



        /**
         * Тестирование одной пары на четыре месяца по чётным неделям, а именно те, что не являются 2, 4 и 8 неделями.
         */
        [Fact]
        public void startTestOneCoupleDuring4MonthInSomeExceptionWeek()
        {

            LocalDate start = new LocalDate(2018, (int)IsoMonth.January, 1);
            LocalDate finish = new LocalDate(2018, (int)IsoMonth.APRIL, YearMonth.of(2018, (int)IsoMonth.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.THURSDAY; // Четверг

            DateTimeZone timezone = DateTimeZone.of("GMT+00:00"); // GMT+0:00

            // Имя группы.
            string nGr = "АБВГ-01-ГА";
            // Название предмета.
            string nam = "Игрообразование кр. 2, 4 и 8 н.";
            // Тип пары.
            string typ = "Лабораторная работа.";
            // Учитель.
            string tic = "ГГГГгггггг. А. а.";
            // Адрес филиала.
            string add = "ВОдичка";
            // Аудитория.
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, timezone, 1, time1, time2, day, false, nam, typ, nGr, tic, aud, add);

            /* Количество */
            Assert.Equal(5, @out.Count);
            /* Время начала пары 3*/
            Assert.Equal(
                    ZonedDateTime.of(
                            LocalDateTime.of(
                                    new LocalDate(
                                            2018, (int)IsoMonth.FEBRUARY, 8
                                    ),
                                    new LocalTime(
                                            10, 40, 0
                                    )
                            ),
                            timezone
                    ),
                    @out[0].DateAndTimeOfCouple
            );
            /* Время начала пары 5*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.MARCH, 8), new LocalTime(10, 40, 0)), timezone), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.MARCH, 22), new LocalTime(10, 40, 0)), timezone), @out[2].DateAndTimeOfCouple);
            /* Время начала пары 7*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.APRIL, 5), new LocalTime(10, 40, 0)), timezone), @out[3].DateAndTimeOfCouple);
            /* Время начала пары 8*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.APRIL, 19), new LocalTime(10, 40, 0)), timezone), @out[4].DateAndTimeOfCouple);

            /* Время конца пары 5 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.MARCH, 8), time2), timezone), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.MARCH, 22), time2), timezone), @out[2].DateAndTimeFinishOfCouple);
            /* Время конца пары 7 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.APRIL, 5), time2), timezone), @out[3].DateAndTimeFinishOfCouple);
            /* Время конца пары 8 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.APRIL, 19), time2), timezone), @out[4].DateAndTimeFinishOfCouple);
            for (CoupleInCalendar o : @out)
            {
                /* Название группы */
                Assert.Equal(nGr, o.NameOfGroup);
                /* Название предмета */
                Assert.Equal(nam, o.ItemTitle);
                /* Тип пары */
                Assert.Equal(typ, o.TypeOfLesson);
                /* Имя преподавателя*/
                Assert.Equal(tic, o.NameOfTeacher);
                /* Адрес кампуса */
                Assert.Equal(add, o.Address);
                /* Аудитория */
                Assert.Equal(aud, o.Audience);
            }
        }

        [Fact]
        public void startTestStartWith5Week()
        {
            LocalDate start = new LocalDate(2018, (int)IsoMonth.January, 1);
            LocalDate finish = new LocalDate(2018, (int)IsoMonth.FEBRUARY, YearMonth.of(2018, (int)IsoMonth.FEBRUARY).lengthOfMonth()); // Последний день января 2018 года.

            LocalTime timeStartCouple = new LocalTime(10, 40, 0);
            LocalTime timeFinishCouple = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.THURSDAY; // Четверг

            DateTimeZone timezone = DateTimeZone.of("GMT+04:00"); // GMT+4:00

            // Имя группы.
            string nGr = "АБВГ-01-ГА";
            // Название предмета.
            string nam = "с 5 недели Игрообразование";
            // Тип пары.
            string typ = "Лабораторная работа.";
            // Учитель.
            string tic = "ГГГГгггггг. А. а.";
            // Адрес филиала.
            string add = "ВОдичка";
            // Аудитория.
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, timezone, 1, timeStartCouple, timeFinishCouple, day, true, nam, typ, nGr, tic, aud, add);

            /* Количество */
            Assert.Equal(2, @out.Count);
            /* Время начала пары*/
            Assert.Equal(
                    ZonedDateTime.of(
                            LocalDateTime.of(
                                    new LocalDate(
                                            2018, (int)IsoMonth.FEBRUARY, 1
                                    ),
                                    new LocalTime(
                                            10, 40, 0
                                    )
                            ),
                            timezone
                    ),
                    @out[0].DateAndTimeOfCouple
            );
            /* Время начала пары 5*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.FEBRUARY, 1), new LocalTime(10, 40, 0)), timezone), @out[0].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.FEBRUARY, 15), new LocalTime(10, 40, 0)), timezone), @out[1].DateAndTimeOfCouple);

            /* Время конца пары 5 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.FEBRUARY, 1), timeFinishCouple), timezone), @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(ZonedDateTime.of(LocalDateTime.of(new LocalDate(2018, (int)IsoMonth.FEBRUARY, 15), timeFinishCouple), timezone), @out[1].DateAndTimeFinishOfCouple);
            for (CoupleInCalendar o : @out)
            {
                /* Название группы */
                Assert.Equal(nGr, o.NameOfGroup);
                /* Название предмета */
                Assert.Equal(nam, o.ItemTitle);
                /* Тип пары */
                Assert.Equal(typ, o.TypeOfLesson);
                /* Имя преподавателя*/
                Assert.Equal(tic, o.NameOfTeacher);
                /* Адрес кампуса */
                Assert.Equal(add, o.Address);
                /* Аудитория */
                Assert.Equal(aud, o.Audience);
            }
        }
    }
}
