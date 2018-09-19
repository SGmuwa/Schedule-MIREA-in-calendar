import org.junit.Assert;
import org.junit.Test;
import static org.junit.Assert.*;
import ru.mirea.xlsical.CouplesDetective.Couple;

import java.time.*;
import java.time.temporal.ChronoUnit;
import java.util.List;
import java.util.regex.Pattern;

public class CoupleTest {
    /**
     * Тестирование, если у нас одна пара в один день.
     */
    @Test
    public void startTestOneCouple() {

        LocalDate start = LocalDate.of(2017, Month.DECEMBER, 31);
        LocalDate finish = LocalDate.of(2018, Month.JANUARY, 2);

        LocalTime time1 = LocalTime.of(0, 1, 0);
        LocalTime time2 = LocalTime.of(0, 3, 0); // Гыыы, пара длится 2 минуты

        DayOfWeek day = DayOfWeek.MONDAY;

        ZoneId timezone = ZoneId.systemDefault();

        String nGr = "Группа-01 32";
        String nam = "Математика и инженерия."; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
        String typ = "Лк";
        String tic = "Иванов В.В.";
        String add = "Москва, проспект Вернадского 78, РТУ МИРЭА";
        String aud = "А-1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, true, nGr, nam, typ, tic, aud, add);

        assertNotNull(out);

        /* Количество */            assertEquals(1, out.size());
        ZonedDateTime zonedDateTimeDateTime = ZonedDateTime.of(
                LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 1), LocalTime.of(0, 1, 0)),
                        ZoneId.systemDefault());
        /* Время начала пары*/      assertEquals(zonedDateTimeDateTime, out.get(0).DateAndTimeOfCouple);
        /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), out.get(0).DurationOfCouple);
        /* Название группы */       assertEquals(nGr, out.get(0).NameOfGroup);
        /* Название предмета */     assertEquals(nam, out.get(0).ItemTitle);
        /* Тип пары */              assertEquals(typ, out.get(0).TypeOfLesson);
        /* Имя преподавателя*/      assertEquals(tic, out.get(0).NameOfTeacher);
        /* Адрес кампуса */         assertEquals(add, out.get(0).Address);
        /* Аудитория */             assertEquals(aud, out.get(0).Audience);
    }

    /**
     * Тестирование, если у нас одна пара в один день. Используется иное Timezone.
     */
    @Test
    public void startTestOneCoupleTimezone() {

        LocalDate start = LocalDate.of(2017, Month.DECEMBER, 31);
        LocalDate finish = LocalDate.of(2018, Month.JANUARY, 2);

        LocalTime time1 = LocalTime.of(0, 1, 0);
        LocalTime time2 = LocalTime.of(0, 3, 0); // Гыыы, пара длится 2 минуты

        DayOfWeek day = DayOfWeek.MONDAY;

        ZoneId timezone = ZoneId.of("GMT+3:00"); // GMT+3:00

        String nGr = "Группа-01 32";
        String nam = "Математика и инженерия."; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
        String typ = "Лк";
        String tic = "Иванов В.В.";
        String add = "Москва, проспект Вернадского 78, РТУ МИРЭА";
        String aud = "А-1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, true, nGr, nam, typ, tic, aud, add);

        assertNotNull(out);

        /* Количество */            assertEquals(1, out.size());
        ZonedDateTime zonedDateTime = ZonedDateTime.of(
                LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 1), LocalTime.of(0, 1, 0)),
                timezone
        );
        /* Время начала пары*/      assertEquals(zonedDateTime, out.get(0).DateAndTimeOfCouple);
        /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), out.get(0).DurationOfCouple);
        /* Название группы */       assertEquals(nGr, out.get(0).NameOfGroup);
        /* Название предмета */     assertEquals(nam, out.get(0).ItemTitle);
        /* Тип пары */              assertEquals(typ, out.get(0).TypeOfLesson);
        /* Имя преподавателя*/      assertEquals(tic, out.get(0).NameOfTeacher);
        /* Адрес кампуса */         assertEquals(add, out.get(0).Address);
        /* Аудитория */             assertEquals(aud, out.get(0).Audience);
    }

    /**
     * Тестирование, если у нас одна пара на один конкретный день дней.
     */
    @Test
    public void startTestOneHardCouple() {

        LocalDate start = LocalDate.of(2018, Month.JANUARY, 1);
        LocalDate finish = LocalDate.of(2018, Month.JANUARY, 1);

        LocalTime time1 = LocalTime.of(0, 1, 0);
        LocalTime time2 = LocalTime.of(0, 3, 0); // Гыыы, пара длится 2 минуты

        DayOfWeek day = DayOfWeek.MONDAY;

        ZoneId timezone = ZoneId.of("GMT+00:00"); // GMT+0:00

        String nGr = "Группа-24 32";
        String nam = "Русский jaja номер Нан 1 4 2."; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
        String typ = "Лаб.";
        String tic = "Момов А.А.";
        String add = "Україна, Київ, Центральна 8";
        String aud = "202";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, true, nGr, nam, typ, tic, aud, add);

        assertNotNull(out);

        /* Количество */            assertEquals(1, out.size());
        ZonedDateTime zonedDateTime = ZonedDateTime.of(
                LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 1), LocalTime.of(0, 1, 0)),
                timezone
        );
        /* Время начала пары*/      assertEquals(zonedDateTime, out.get(0).DateAndTimeOfCouple);
        /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), out.get(0).DurationOfCouple);
        /* Название группы */       assertEquals(nGr, out.get(0).NameOfGroup);
        /* Название предмета */     assertEquals(nam, out.get(0).ItemTitle);
        /* Тип пары */              assertEquals(typ, out.get(0).TypeOfLesson);
        /* Имя преподавателя*/      assertEquals(tic, out.get(0).NameOfTeacher);
        /* Адрес кампуса */         assertEquals(add, out.get(0).Address);
        /* Аудитория */             assertEquals(aud, out.get(0).Audience);
    }

    /**
     * Тестирование, если у нас одна пара в расписании в течение 4 недель. Итого за 4 недели у нас 2 пары.
     */
    @Test
    public void startTestOneCoupleTwoDays() {

        LocalDate start = LocalDate.of(2018, Month.JANUARY, 1 /*+ 1*/); // Протестируем вторник.
        LocalDate finish = LocalDate.of(2018, Month.JANUARY, 1 + 7*4 + 1); // 29 + 1 = 30

        LocalTime time1 = LocalTime.of(9, 0, 0);
        LocalTime time2 = LocalTime.of(10, 30, 0);

        DayOfWeek day = DayOfWeek.TUESDAY;

        ZoneId timezone = ZoneId.of("UTC+00:00"); // UTC+0:00

        String nGr = "Группа-,";
        String nam = "Математика и н. значения .pgtju340)(HG(fvhgvh"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
        String typ = "Пр@ктика";
        String tic = "В.В. Иванов";
        String add = "Москва, проспект Вернадского 78, РТУ МИРЭА";
        String aud = "А-1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, true, nGr, nam, typ, tic, aud, add);

        assertNotNull(out);

        /* Количество */            assertEquals(2, out.size());
        /* Время начала пары*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 2), LocalTime.of(9, 0, 0)), timezone), out.get(0).DateAndTimeOfCouple);
        /* Время начала пары*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 2), LocalTime.of(9, 0, 0)), timezone).plus(2, ChronoUnit.WEEKS), out.get(1).DateAndTimeOfCouple);
        for(Couple o : out)
        {
            /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), o.DurationOfCouple);
            /* Название группы */       assertEquals(nGr, o.NameOfGroup);
            /* Название предмета */     assertEquals(nam, o.ItemTitle);
            /* Тип пары */              assertEquals(typ, o.TypeOfLesson);
            /* Имя преподавателя*/      assertEquals(tic, o.NameOfTeacher);
            /* Адрес кампуса */         assertEquals(add, o.Address);
            /* Аудитория */             assertEquals(aud, o.Audience);
        }
    }

    /**
     * Тестирование, если у нас одна пара в расписании в течение 4 месяцев. Итого за 4 месяца у нас полно пар раз в две недели.
     */
    @Test
    public void startTestOneCoupleDuring4Month() {

        LocalDate start = LocalDate.of(2018, Month.JANUARY, 1);
        LocalDate finish = LocalDate.of(2018, Month.APRIL, YearMonth.of(2018, Month.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

        LocalTime time1 = LocalTime.of(10, 40, 0);
        LocalTime time2 = LocalTime.of(12, 10, 0);

        DayOfWeek day = DayOfWeek.WEDNESDAY; // Среда

        ZoneId timezone = ZoneId.of("GMT+0:00"); // GMT+0:00

        String nGr = "АБВГ-01-ГА";
        String nam = ",vrihjegijrw\"woefkweo\21ew_093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н.3ен.ен.45..5н.54.542FPQWQ#@(-)@(#)$oqfk"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
        String typ = "Лабораторная работа.";
        String tic = "ГГГГгггггг. А. а.";
        String add = "ВОдичка";
        String aud = "А-(-1) = А+1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, true, nGr, nam, typ, tic, aud, add);

        assertNotNull(out);

        /* Количество */            assertEquals(9, out.size());
        /* Время начала пары 1*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(0  , ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(0).DateAndTimeOfCouple);
        /* Время начала пары 2*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(2  , ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(1).DateAndTimeOfCouple);
        /* Время начала пары 3*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(2*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(2).DateAndTimeOfCouple);
        /* Время начала пары 4*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(3*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(3).DateAndTimeOfCouple);
        /* Время начала пары 5*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(4*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(4).DateAndTimeOfCouple);
        /* Время начала пары 6*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(5*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(5).DateAndTimeOfCouple);
        /* Время начала пары 7*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(6*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(6).DateAndTimeOfCouple);
        /* Время начала пары 8*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(7*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(7).DateAndTimeOfCouple);
        /* Время начала пары 9*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 3).plus(8*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(8).DateAndTimeOfCouple);
        for(Couple o : out)
        {
            /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), o.DurationOfCouple);
            /* Название группы */       assertEquals(nGr, o.NameOfGroup);
            /* Название предмета */     assertEquals(nam, o.ItemTitle);
            /* Тип пары */              assertEquals(typ, o.TypeOfLesson);
            /* Имя преподавателя*/      assertEquals(tic, o.NameOfTeacher);
            /* Адрес кампуса */         assertEquals(add, o.Address);
            /* Аудитория */             assertEquals(aud, o.Audience);
        }
    }

    /**
     * Тестирование одной пары на четыре месяца по чётным неделям.
     */
    @Test
    public void startTestOneCoupleDuring4MonthInEvenWeek() {

        LocalDate start = LocalDate.of(2018, Month.JANUARY, 1);
        LocalDate finish = LocalDate.of(2018, Month.APRIL, YearMonth.of(2018, Month.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

        LocalTime time1 = LocalTime.of(10, 40, 0);
        LocalTime time2 = LocalTime.of(12, 10, 0);

        DayOfWeek day = DayOfWeek.THURSDAY; // Четверг

        ZoneId timezone = ZoneId.of("GMT+00:00"); // GMT+0:00

        String nGr = "АБВГ-01-ГА";
        String nam = ",vrihjegijrw\"woefkweo\21ew_093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н.3ен.ен.45..5н.54.542FPQWQ#@(-)@(#)$oqfk"; // http://xpoint.ru/forums/internet/standards/thread/29138.xhtml
        String typ = "Лабораторная работа.";
        String tic = "ГГГГгггггг. А. а.";
        String add = "ВОдичка";
        String aud = "А-(-1) = А+1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, false, nGr, nam, typ, tic, aud, add);

        /* Количество */            assertEquals(8, out.size());

        assertNotNull(out);

        /* Время начала пары 1*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(0  , ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(0).DateAndTimeOfCouple);
        /* Время начала пары 2*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2  , ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(1).DateAndTimeOfCouple);
        /* Время начала пары 3*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2*2, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(2).DateAndTimeOfCouple);
        /* Время начала пары 4*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2*3, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(3).DateAndTimeOfCouple);
        /* Время начала пары 5*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2*4, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(4).DateAndTimeOfCouple);
        /* Время начала пары 6*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2*5, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(5).DateAndTimeOfCouple);
        /* Время начала пары 7*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2*6, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(6).DateAndTimeOfCouple);
        /* Время начала пары 8*/      assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY, 11).plus(2*7, ChronoUnit.WEEKS), LocalTime.of(10, 40, 0)), timezone), out.get(7).DateAndTimeOfCouple);
        for(Couple o : out)
        {
            /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), o.DurationOfCouple);
            /* Название группы */       assertEquals(nGr, o.NameOfGroup);
            /* Название предмета */     assertEquals(nam, o.ItemTitle);
            /* Тип пары */              assertEquals(typ, o.TypeOfLesson);
            /* Имя преподавателя*/      assertEquals(tic, o.NameOfTeacher);
            /* Адрес кампуса */         assertEquals(add, o.Address);
            /* Аудитория */             assertEquals(aud, o.Audience);
        }
    }

    /**
     * Данный тест проверяет, правильно ли программа отвечает на вопрос,
     * есть ли в записи предмета информации об исключениях датах.
     * А именно на каких неделях есть пары, или на каких недлях пар нет.
     */
    @Test
    public void startTestRex() {

        /*
        Beginner
         */

        assertTrue(
                Pattern.compile("^[a-z0-9_-]{3,15}$")
                        .matcher("vovan").matches()
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

        // --------


        assertTrue(Couple.isStringHaveWeek("1 н. 1"));
        assertTrue(Couple.isStringHaveWeek("1 н. 1\n"));
        assertTrue(Couple.isStringHaveWeek("кр 5 н Логика\n"));
        assertTrue(Couple.isStringHaveWeek("кр. 5 н. Логика\n"));
        assertFalse(Couple.isStringHaveWeek("Внешний и внутренний PR\n"));
        assertFalse(Couple.isStringHaveWeek("Дискретная математика\n"));
        assertTrue(Couple.isStringHaveWeek("11,13,15,17 н. Правоведение\n"));
        assertTrue(Couple.isStringHaveWeek("11,13,15,17 н Правоведение\n"));

        assertTrue(Couple.isStringHaveWeekException("кр 5 н Логика\n"));
        assertTrue(Couple.isStringHaveWeekException("кр. 5 н. Логика\n"));
        assertFalse(Couple.isStringHaveWeekException("Внешний и внутренний PR\n"));
        assertFalse(Couple.isStringHaveWeekException("Дискретная математика\n"));
        assertFalse(Couple.isStringHaveWeekException("11,13,15,17 н. Правоведение\n"));

        assertFalse(Couple.isStringHaveWeek(",vrihjegijrw\"woefkweo\21ew_093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н.3ен.ен.45..5н.54.542FPQWQ#@(-)@(#)$oqfk"));
        assertFalse(Couple.isStringHaveWeekException(",vrihjegijrw\"woefkweo\21ew_093i2-FFOKEOKOкуцпцшокш342хгйе9з3кшйз3сь4мш9рХШАООХЕ3пп4хзр54.епз35щлр344щее.3уе4.н.3ен.ен.45..5н.54.542FPQWQ#@(-)@(#)$oqfk"));

    }

    /**
     * Тестирование одной пары на четыре месяца по чётным неделям, а именно на 2, 4 и 8.
     */
    @Test
    public void startTestOneCoupleDuring4MonthInSomeWeek() {

        LocalDate start = LocalDate.of(2018, Month.JANUARY, 1);
        LocalDate finish = LocalDate.of(2018, Month.APRIL, YearMonth.of(2018, Month.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

        LocalTime time1 = LocalTime.of(10, 40, 0);
        LocalTime time2 = LocalTime.of(12, 10, 0);

        DayOfWeek day = DayOfWeek.THURSDAY; // Четверг

        ZoneId timezone = ZoneId.of("Europe/Moscow"); // Moscow

        // Имя группы.
        String nGr = "АБВГ-01-ГА";
        // Название предмета.
        String nam = "Игрообразование 2, 4 и 8 н.";
        // Тип пары.
        String typ = "Лабораторная работа.";
        // Учитель.
        String tic = "ГГГГгггггг. А. а.";
        // Адрес филиала.
        String add = "ВОдичка";
        // Аудитория.
        String aud = "А-(-1) = А+1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, false, nGr, nam, typ, tic, aud, add);

        assertNotNull(out);

        /* Количество */            assertEquals(3, out.size());
        /* Время начала пары 1*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY , 11), LocalTime.of(10, 40, 0)), timezone), out.get(0).DateAndTimeOfCouple);
        /* Время начала пары 2*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.JANUARY , 25), LocalTime.of(10, 40, 0)), timezone), out.get(1).DateAndTimeOfCouple);
        /* Время начала пары 4*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.FEBRUARY, 22), LocalTime.of(10, 40, 0)), timezone), out.get(2).DateAndTimeOfCouple);
        for(Couple o : out)
        {
            /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), o.DurationOfCouple);
            /* Название группы */       assertEquals(nGr, o.NameOfGroup);
            /* Название предмета */     assertEquals(nam, o.ItemTitle);
            /* Тип пары */              assertEquals(typ, o.TypeOfLesson);
            /* Имя преподавателя*/      assertEquals(tic, o.NameOfTeacher);
            /* Адрес кампуса */         assertEquals(add, o.Address);
            /* Аудитория */             assertEquals(aud, o.Audience);
        }
    }



    /**
     * Тестирование одной пары на четыре месяца по чётным неделям, а именно те, что не являются 2, 4 и 8 неделями.
     */
    @Test
    public void startTestOneCoupleDuring4MonthInSomeExceptionWeek() {

        LocalDate start = LocalDate.of(2018, Month.JANUARY, 1);
        LocalDate finish = LocalDate.of(2018, Month.APRIL, YearMonth.of(2018, Month.APRIL).lengthOfMonth()); // Последний день апреля 2018 года.

        LocalTime time1 = LocalTime.of(10, 40, 0);
        LocalTime time2 = LocalTime.of(12, 10, 0);

        DayOfWeek day = DayOfWeek.THURSDAY; // Четверг

        ZoneId timezone = ZoneId.of("GMT+00:00"); // GMT+0:00

        // Имя группы.
        String nGr = "АБВГ-01-ГА";
        // Название предмета.
        String nam = "Игрообразование кр. 2, 4 и 8 н.";
        // Тип пары.
        String typ = "Лабораторная работа.";
        // Учитель.
        String tic = "ГГГГгггггг. А. а.";
        // Адрес филиала.
        String add = "ВОдичка";
        // Аудитория.
        String aud = "А-(-1) = А+1";

        List<Couple> out = Couple.GetCouplesByPeriod(start, finish, time1, time2, timezone, day, false, nGr, nam, typ, tic, aud, add);

        /* Количество */            assertEquals(5, out.size());
        /* Время начала пары 3*/
        assertEquals(
                ZonedDateTime.of(
                        LocalDateTime.of(
                                LocalDate.of(
                                        2018, Month.FEBRUARY, 8
                                ),
                                LocalTime.of(
                                        10, 40, 0
                                )
                        ),
                        timezone
                ),
                out.get(0).DateAndTimeOfCouple
        );
        /* Время начала пары 5*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.MARCH, 8 ), LocalTime.of(10, 40, 0)), timezone), out.get(1).DateAndTimeOfCouple);
        /* Время начала пары 6*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.MARCH, 22), LocalTime.of(10, 40, 0)), timezone), out.get(2).DateAndTimeOfCouple);
        /* Время начала пары 7*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.APRIL, 5 ), LocalTime.of(10, 40, 0)), timezone), out.get(3).DateAndTimeOfCouple);
        /* Время начала пары 8*/    assertEquals(ZonedDateTime.of(LocalDateTime.of(LocalDate.of(2018, Month.APRIL, 19), LocalTime.of(10, 40, 0)), timezone), out.get(4).DateAndTimeOfCouple);
        for(Couple o : out)
        {
            /* Продолжительность пары*/ assertEquals(Duration.between(time1, time2), o.DurationOfCouple);
            /* Название группы */       assertEquals(nGr, o.NameOfGroup);
            /* Название предмета */     assertEquals(nam, o.ItemTitle);
            /* Тип пары */              assertEquals(typ, o.TypeOfLesson);
            /* Имя преподавателя*/      assertEquals(tic, o.NameOfTeacher);
            /* Адрес кампуса */         assertEquals(add, o.Address);
            /* Аудитория */             assertEquals(aud, o.Audience);
        }
    }
}
