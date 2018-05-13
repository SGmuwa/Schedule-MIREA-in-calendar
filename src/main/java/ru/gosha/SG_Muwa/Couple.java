import java.util.Calendar;
import java.util.Date;
import java.util.LinkedList;

public class Couple {
    // Дата и время пары.
    public Date DateAndTimeOfCouple;
    // Имя преподавателя.
    public String NameOfTeacher;
    // Название пары.
    public String ItemTitle;
    // Номер аудитории.
    public String Audience;
    // Адрес корпуса.
    public String Address;
    // Тип занятия (лекция, практика, лабораторная работа)
    public String TypeOfLesson;

    /**
     * Получает на входе данные про две строки. Принимает решение, в какие дни будут пары.
     * @param Start Дата и время начала сессии. Расписание будет составлено с этого дня и времени.
     * @param Finish Дата и время окончания сессии. Расписание будет составлено до этого дня и времени.
     * @param DayOfTheWeek Рассматриваемый день недели.
     * @param NameOfTeacher Первая строка данных преподавателя.
     * @param ItemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
     * @param Audience Первая строка аудитории.
     * @param Address Адрес корпуса.
     * @param TypeOfLesson Первая строка типа занятия.
     * @param NumberOfCoupleDouble Какая это строка записи занятия?
     * @param TimeOfCouples Расписание звонков занятий в минутах.
     * @return Возвращает, в какие дни будут пары.
     */
    public static Iterable<Couple> GetCouplesByPeriod(Date Start, Date Finish, byte DayOfTheWeek, String NameOfTeacher, String ItemTitle, String Audience, String Address, String TypeOfLesson, int NumberOfCoupleDouble, int[] TimeOfCouples) {
        ///.get(Calendar.DAY_OF_WEEK);

    }
}
