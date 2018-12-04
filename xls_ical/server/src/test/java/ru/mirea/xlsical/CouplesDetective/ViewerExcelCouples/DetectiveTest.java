package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import org.junit.Test;

import java.time.*;
import java.time.temporal.ChronoUnit;

import static org.junit.Assert.*;
import static ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective.addBusinessDaysToDate;
import static ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.Detective.getCounts6Days;

public class DetectiveTest {


    /**
     * Функция разработана для математического вычисления,
     * как надо реализовать расстановку начала и конца семестра.
     */
    @Test
    public void calculateDays() {

        assertEquals(2,
                getCounts6Days(ZonedDateTime.of(
                        LocalDate.of(2018, 12, 3),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                        ),
                        ZonedDateTime.of(
                                LocalDate.of(2018, 12, 4),
                                LocalTime.of(0, 0, 0),
                                ZoneId.systemDefault()
                        )
                )
        );

        assertEquals(1,
                getCounts6Days(ZonedDateTime.of(
                        LocalDate.of(2018, 12, 1),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                        ),
                        ZonedDateTime.of(
                                LocalDate.of(2018, 12, 2),
                                LocalTime.of(0, 0, 0),
                                ZoneId.systemDefault()
                        )
                )
        );

        assertEquals(1,
                getCounts6Days(ZonedDateTime.of(
                        LocalDate.of(2018, 12, 2),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                        ),
                        ZonedDateTime.of(
                                LocalDate.of(2018, 12, 3),
                                LocalTime.of(0, 0, 0),
                                ZoneId.systemDefault()
                        )
                )
        );

        assertEquals(6,
                getCounts6Days(ZonedDateTime.of(
                        LocalDate.of(2000, 1, 1),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                        ),
                        ZonedDateTime.of(
                                LocalDate.of(2000, 1, 7),
                                LocalTime.of(0, 0, 0),
                                ZoneId.systemDefault()
                        )
                )
        );

        System.out.print(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2015, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2015, 2, 8),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )) + ", ");

        System.out.print(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2017, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2017, 2, 5),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )) + ", ");
        System.out.print(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2018, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2018, 2, 8),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )) + ", ");
        System.out.println(getCounts6Days(ZonedDateTime.of(
                LocalDate.of(2019, 1, 1),
                LocalTime.of(0, 0, 0),
                ZoneId.systemDefault()
                ),
                ZonedDateTime.of(
                        LocalDate.of(2019, 2, 10),
                        LocalTime.of(0, 0, 0),
                        ZoneId.systemDefault()
                )) + "."
        );
        assertEquals(ZonedDateTime.of(
                LocalDate.of(2018, 12, 4),
                LocalTime.NOON,
                ZoneId.systemDefault()
                ),
                addBusinessDaysToDate(
                        ZonedDateTime.of(
                                LocalDate.of(2018, 12, 2),
                                LocalTime.NOON,
                                ZoneId.systemDefault()
                        ),
                        1
                )
        );
        {
            ZonedDateTime a = ZonedDateTime.of(
                    LocalDate.of(2019, 1, 1),
                    LocalTime.of(0, 0, 0),
                    ZoneId.systemDefault()
            );
            ZonedDateTime b = ZonedDateTime.of(
                    LocalDate.of(2019, 2, 10),
                    LocalTime.of(0, 0, 0),
                    ZoneId.systemDefault()
            );
            assertEquals(getCounts6Days(a, b), getCounts6Days(a, addBusinessDaysToDate(a, getCounts6Days(a, b) - 1)));
        }
    }

}