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
using System.Text.RegularExpressions;
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

        /// <summary>
        /// Тестирование одной пары на четыре месяца по чётным неделям.
        /// </summary>
        [Fact]
        public void StartTestOneCoupleDuring4MonthInEvenWeek()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+00:00"]; // GMT+0:00

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.April, start.Calendar.GetDaysInMonth(2018, (int)IsoMonth.April)).AtStartOfDayInZone(timezone); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.Thursday; // Четверг

            string nGr = "АБВГ-01-ГА";
            string nam = ",vrihjegijrw093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н3ен.е45..5н54.542FPQWQ#@(-)@(#)$oqfk"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
            string typ = "Лабораторная работа.";
            string tic = "ГГГГгггггг. А. а.";
            string add = "ВОдичка";
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, false, nam, typ, nGr, tic, aud, add);

            /* Количество */
            Assert.Equal(8, @out.Count);

            Assert.NotNull(@out);

            ZonedDateTime firstGood = new LocalDate(2018, (int)IsoMonth.January, 11).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone);

            /* Время начала пары 1*/
            Assert.Equal(firstGood, @out[0].DateAndTimeOfCouple);
            /* Время начала пары 2*/
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 3*/
            Assert.Equal(firstGood.PlusWeeks(2 * 2), @out[2].DateAndTimeOfCouple);
            /* Время начала пары 4*/
            Assert.Equal(firstGood.PlusWeeks(2 * 3), @out[3].DateAndTimeOfCouple);
            /* Время начала пары 5*/
            Assert.Equal(firstGood.PlusWeeks(2 * 4), @out[4].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(firstGood.PlusWeeks(2 * 5), @out[5].DateAndTimeOfCouple);
            /* Время начала пары 7*/
            Assert.Equal(firstGood.PlusWeeks(2 * 6), @out[6].DateAndTimeOfCouple);
            /* Время начала пары 8*/
            Assert.Equal(firstGood.PlusWeeks(2 * 7), @out[7].DateAndTimeOfCouple);

            firstGood = new LocalDate(2018, (int)IsoMonth.January, 11).At(time2).InZoneStrictly(timezone);

            /* Время конца пары 1 */
            Assert.Equal(firstGood, @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 2 */
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары 3 */
            Assert.Equal(firstGood.PlusWeeks(2 * 2), @out[2].DateAndTimeFinishOfCouple);
            /* Время конца пары 4 */
            Assert.Equal(firstGood.PlusWeeks(2 * 3), @out[3].DateAndTimeFinishOfCouple);
            /* Время конца пары 5 */
            Assert.Equal(firstGood.PlusWeeks(2 * 4), @out[4].DateAndTimeFinishOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(firstGood.PlusWeeks(2 * 5), @out[5].DateAndTimeFinishOfCouple);
            /* Время конца пары 7 */
            Assert.Equal(firstGood.PlusWeeks(2 * 6), @out[6].DateAndTimeFinishOfCouple);
            /* Время конца пары 8 */
            Assert.Equal(firstGood.PlusWeeks(2 * 7), @out[7].DateAndTimeFinishOfCouple);
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
        /// Данный тест проверяет, правильно ли программа отвечает на вопрос,
        /// есть ли в записи предмета информации об исключениях датах.
        /// А именно на каких неделях есть пары, или на каких неделях пар нет.
        /// </summary>
        [Fact]
        public void StartTestRex()
        {
            Assert.Matches("^[a-z0-9_-]{3,15}$", "volume");
            Assert.DoesNotMatch("^[a-z0-9_-]{3,15}$", "_@BEST");
            Assert.Matches(@"^.+w\.?.+$", "1 w. 1");
            Assert.Matches(@"^.+н\.?.+$", "1 н. 1");
            Assert.Matches(@".+н\.?.+", "1 н. 1");
            Assert.DoesNotMatch(@"н\.?", "1 н. 1");
            Assert.Matches(@"(^.+\s[нН]\.?.+$)", "1 н. 1");
            Assert.DoesNotMatch(@"(^.+\s[нН]\.?.+$)|()", "1 н. 1\n");

            Assert.Equal(new int[] { 1 }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("1 н. 1"));
            Assert.Equal(new int[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("кр 5 н Логика"));
            Assert.Equal(new int[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("кр. 5 н. Логика"));
            Assert.Equal(new int[] { }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("Внешний и внутренний PR"));
            Assert.Equal(new int[] { }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("Дискретная математика"));
            Assert.Equal(new int[] { 11, 13, 15, 17 }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("11,13,15,17 н. Правоведение"));
            Assert.Equal(new int[] { 11, 13, 15, 17 }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("11,13,15,17 н Правоведение"));
            Assert.Equal(new int[] { }, DetectiveSemester.SetterCouplesInCalendar.GetAllOnlyWeeks("История Неолита"));

            Assert.Equal(new int[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.GetAllExceptionWeeks("кр 5 н Логика"));
            Assert.Equal(new int[] { 5 }, DetectiveSemester.SetterCouplesInCalendar.GetAllExceptionWeeks("кр. 5 н. Логика"));
            Assert.Equal(new int[] { }, DetectiveSemester.SetterCouplesInCalendar.GetAllExceptionWeeks("Внешний и внутренний PR"));
            Assert.Equal(new int[] { }, DetectiveSemester.SetterCouplesInCalendar.GetAllExceptionWeeks("Дискретная математика"));
            Assert.Equal(new int[] { }, DetectiveSemester.SetterCouplesInCalendar.GetAllExceptionWeeks("11,13,15,17 н. Правоведение"));
        }

        /// <summary>
        /// Тестирование одной пары на четыре месяца по чётным неделям, а именно на 2, 4 и 8.
        /// </summary>
        [Fact]
        public void StartTestOneCoupleDuring4MonthInSomeWeek()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["Europe/Moscow"]; // Moscow

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.April, start.Calendar.GetDaysInMonth(2018, (int)IsoMonth.April)).AtStartOfDayInZone(timezone); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.Thursday; // Четверг

            // Имя группы.
            string nGr = "АБВГ-01-ГА";
            // Название предмета.
            string nam = "Игровое образование 2, 4 и 8 н.";
            // Тип пары.
            string typ = "Лабораторная работа.";
            // Учитель.
            string tic = "ГГГГгггггг. А. а.";
            // Адрес филиала.
            string add = "ВОдичка";
            // Аудитория.
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, false, nam, typ, nGr, tic, aud, add);

            Assert.NotNull(@out);

            ZonedDateTime firstGood = new LocalDate(2018, (int)IsoMonth.January, 11).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone);

            /* Количество */
            Assert.Equal(3, @out.Count);
            /* Время начала пары 1*/
            Assert.Equal(firstGood, @out[0].DateAndTimeOfCouple);
            /* Время начала пары 2*/
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 4*/
            Assert.Equal(firstGood.PlusWeeks(2 * 3), @out[2].DateAndTimeOfCouple);

            firstGood = new LocalDate(2018, (int)IsoMonth.January, 11).At(time2).InZoneStrictly(timezone);

            /* Время конца пары 1*/
            Assert.Equal(firstGood, @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 2*/
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeFinishOfCouple);
            /* Время конца пары 4*/
            Assert.Equal(firstGood.PlusWeeks(2 * 3), @out[2].DateAndTimeFinishOfCouple);

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
        /// Тестирование одной пары на четыре месяца по чётным неделям, а именно те, что не являются 2, 4 и 8 неделями.
        /// </summary>
        [Fact]
        public void StartTestOneCoupleDuring4MonthInSomeExceptionWeek()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+00:00"]; // GMT+0:00

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.April, start.Calendar.GetDaysInMonth(2018, (int)IsoMonth.April)).AtStartOfDayInZone(timezone); // Последний день апреля 2018 года.

            LocalTime time1 = new LocalTime(10, 40, 0);
            LocalTime time2 = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.Thursday; // Четверг

            // Имя группы.
            string nGr = "АБВГ-01-ГА";
            // Название предмета.
            string nam = "Игровое образование кр. 2, 4 и 8 н.";
            // Тип пары.
            string typ = "Лабораторная работа.";
            // Учитель.
            string tic = "ГГГГгггггг. А. а.";
            // Адрес филиала.
            string add = "ВОдичка";
            // Аудитория.
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, time1, time2, timezone, day, false, nam, typ, nGr, tic, aud, add);

            /* Количество */
            Assert.Equal(5, @out.Count);
            /* Время начала пары 3*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.February, 8).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone), @out[0].DateAndTimeOfCouple);
            /* Время начала пары 5*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.March, 8).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone), @out[1].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.March, 22).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone), @out[2].DateAndTimeOfCouple);
            /* Время начала пары 7*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.April, 5).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone), @out[3].DateAndTimeOfCouple);
            /* Время начала пары 8*/
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.April, 19).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone), @out[4].DateAndTimeOfCouple);

#warning Отсутсвует проверка начала пары 3.
            /* Время конца пары 5 */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.March, 8).At(time2).InZoneStrictly(timezone), @out[1].DateAndTimeOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.March, 22).At(time2).InZoneStrictly(timezone), @out[2].DateAndTimeOfCouple);
            /* Время конца пары 7 */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.April, 5).At(time2).InZoneStrictly(timezone), @out[3].DateAndTimeOfCouple);
            /* Время конца пары 8 */
            Assert.Equal(new LocalDate(2018, (int)IsoMonth.April, 19).At(time2).InZoneStrictly(timezone), @out[4].DateAndTimeOfCouple);
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

        [Fact]
        public void StartTestStartWith5Week()
        {
            DateTimeZone timezone = DateTimeZoneProviders.Tzdb["GMT+04:00"]; // GMT+4:00

            ZonedDateTime start = new LocalDate(2018, (int)IsoMonth.January, 1).AtStartOfDayInZone(timezone);
#warning Так последний день января или февраля?
            ZonedDateTime finish = new LocalDate(2018, (int)IsoMonth.February, start.Calendar.GetDaysInMonth(2018, (int)IsoMonth.February)).AtStartOfDayInZone(timezone); // Последний день января 2018 года.

            LocalTime timeStartCouple = new LocalTime(10, 40, 0);
            LocalTime timeFinishCouple = new LocalTime(12, 10, 0);

            IsoDayOfWeek day = IsoDayOfWeek.Thursday; // Четверг

            // Имя группы.
            string nGr = "АБВГ-01-ГА";
            // Название предмета.
            string nam = "с 5 недели Игровое образование";
            // Тип пары.
            string typ = "Лабораторная работа.";
            // Учитель.
            string tic = "ГГГГгггггг. А. а.";
            // Адрес филиала.
            string add = "ВОдичка";
            // Аудитория.
            string aud = "А-(-1) = А+1";

            List<CoupleInCalendar> @out = DetectiveSemester.SetterCouplesInCalendar.getCouplesByPeriod(start, finish, 1, timeStartCouple, timeFinishCouple, timezone, day, true, nam, typ, nGr, tic, aud, add);

            ZonedDateTime firstGood = new LocalDate(2018, (int)IsoMonth.February, 1).At(new LocalTime(10, 40, 0)).InZoneStrictly(timezone);

            /* Количество */
            Assert.Equal(2, @out.Count);
            /* Время начала пары 5*/
            Assert.Equal(firstGood, @out[0].DateAndTimeOfCouple);
            /* Время начала пары 6*/
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeOfCouple);

            firstGood = new LocalDate(2018, (int)IsoMonth.February, 1).At(timeFinishCouple).InZoneStrictly(timezone);

            /* Время конца пары 5 */
            Assert.Equal(firstGood, @out[0].DateAndTimeFinishOfCouple);
            /* Время конца пары 6 */
            Assert.Equal(firstGood.PlusWeeks(2), @out[1].DateAndTimeFinishOfCouple);
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
    }
}
