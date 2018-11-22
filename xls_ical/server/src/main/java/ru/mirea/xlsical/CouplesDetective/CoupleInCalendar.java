package ru.mirea.xlsical.CouplesDetective;

import java.time.*;
import java.util.Objects;


/**
 * Структура данных, которая представляет учебную пару в определённый день и время.
 * Сокращённо: "Календарная пара".
 * Время начала и конца пары, название группы и имя преподавателя,
 * название предмета, аудитория, адрес, тип пары.
 */
public class CoupleInCalendar extends Couple {

    public CoupleInCalendar(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, ZonedDateTime dateAndTimeOfCouple, ZonedDateTime dateAndTimeFinishOfCouple) {
        super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
        this.dateAndTimeOfCouple = dateAndTimeOfCouple;
        this.dateAndTimeFinishOfCouple = dateAndTimeFinishOfCouple;
    }

    /**
     * Дата и время пары.
     */
    public final ZonedDateTime dateAndTimeOfCouple;
    /**
     * Количество времени, сколько длится пара.
     */
    public final ZonedDateTime dateAndTimeFinishOfCouple;

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
}
