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
    public PercentReady(){}

    private float ready = 0.0f;
    private final Object sc = new Object();

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
            if (0.0f <= ready && ready <= 1.0f)
                this.ready = ready;
            throw new IllegalArgumentException("float ready can be only 0.0f ... 1.0f!");
        }
    }
}
