package ru.mirea.xlsical.interpreter;

/**
 * Класс, который служит для управления процентом готовности задачи.
 * @since 18.11.2018
 * @version 18.11.2018
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 */
public class PercentReady {

    /**
     * Создание нового экземпляра управления процентом готовности.
     */
    public PercentReady(){
        whole = null;
        coefficient = 1.0f;
    }

    /**
     * Создание ребёнка, который изменяет общий сумматор.
     * @param whole Ссылка на целую часть.
     * @param coefficient Коэфициент взаимодействя на целую часть. Допустимые значения: от 0 до 1.
     */
    public PercentReady(PercentReady whole, float coefficient) {
        this.whole = whole;
        if(0.0f <= coefficient && coefficient <= 1.0f)
            this.coefficient = coefficient;
        else
            throw new IllegalArgumentException("Коэффициент должен быть от 0 до 1 включительно!");
    }

    /**
     * На сколько готов текущий загрузчик?
     */
    private float ready = 0.0f;
    /**
     * Объект синхронизации
     */
    private final Object sc = new Object();
    /**
     * Ссылка на родителя
     */
    private final PercentReady whole;
    /**
     * Коэффициент.
     */
    public final float coefficient;

    /**
     * Возвращает процент готовности от 0 до 1.
     * @return Процент готовности от 0 до 1.
     */
    public float getReady() {
        synchronized (sc) {
            return ready;
        }
    }

    /**
     * Устанавливает процент готовности задачи от 0 до 1.
     * @param ready Процент готовности от 0 до 1.
     */
    public void setReady(float ready) {
        synchronized (sc) {
            if (whole == null)
                if (0.0f <= ready && ready <= 1.0f)
                    this.ready = ready;
                else
                    throw new IllegalArgumentException("float ready can be only 0.0f ... 1.0f!");
            else {
                synchronized (whole.sc) {

                }
            }
        }
    }
}
