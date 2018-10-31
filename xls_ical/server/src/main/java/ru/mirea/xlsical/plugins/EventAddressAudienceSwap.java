package ru.mirea.xlsical.plugins;

import ru.mirea.xlsical.CouplesDetective.Couple;

import java.time.ZonedDateTime;

public class EventAddressAudienceSwap extends Couple {
    private EventAddressAudienceSwap(ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple, String nameOfGroup, String nameOfTeacher, String itemTitle, String audience, String address, String typeOfLesson) {
        super(dateAndTimeOfCouple, dateAndTimeFinishOfCouple, nameOfGroup, nameOfTeacher, itemTitle, audience, address, typeOfLesson);
    }

    /**
     * Мненяет местами фактический адрес и номер аудитории и ФИО Преподавателя.
     * Плагин сделан для быстрого просмотра из краткой сводки календаря.
     * @param couple входящая пара которую следует поменять
     * @return Событие с изменённым форматом для быстрого чтения.
     */
    public Couple Swap(Couple couple) {
        return new EventAddressAudienceSwap(couple.DateAndTimeOfCouple, couple.DateAndTimeFinishOfCouple, couple.NameOfGroup, couple.NameOfTeacher,
                couple.ItemTitle, couple.Address,
                couple.Audience + System.lineSeparator() + couple.NameOfTeacher,
                couple.TypeOfLesson);
    }
}
