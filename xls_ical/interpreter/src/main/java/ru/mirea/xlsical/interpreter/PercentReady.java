package ru.mirea.xlsical.interpreter;

/**
 * Класс, который служит для управления процентом готовности задачи.
 * Данный класс является потокобезопасным.
 * @since 18.11.2018
 * @version 2018-12-08
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>
 */
public class PercentReady {

    /**
     * Создание нового экземпляра управления процентом готовности.
     */
    public PercentReady(){
        this(null, 1.0f);
    }

    /**
     * Создание нового экземпляра управления процентом готовности.
     * @param subscriber Функция, которую надо вызывать при каждом изменении значения.
     */
    public PercentReady(ICanUsePercentReady subscriber) {
        this(null, 1.0f, subscriber);
    }

    /**
     * Создание потомка, который изменяет общий сумматор.
     * @param whole Ссылка на целую часть.
     * @param coefficient Коэфициент взаимодействя на целую часть. Допустимые значения: от 0 до 1.
     */
    public PercentReady(PercentReady whole, float coefficient) {
        this(whole, coefficient, null);
    }

    /**
     * Создание ребёнка, который изменяет общий сумматор.
     * @param whole Ссылка на целую часть.
     * @param coefficient Коэфициент взаимодействя на целую часть. Допустимые значения: от 0 до 1.
     * @param subscriber Функция, которую надо вызывать при каждом изменении значения.
     */
    public PercentReady(PercentReady whole, float coefficient, ICanUsePercentReady subscriber) {
        this.whole = whole;
        this.subscriber = subscriber;
        if(0.0f <= coefficient && coefficient <= 1.0f) {
            this.coefficient = coefficient;
            if(whole != null) {
                synchronized (whole.sc) {
                    if (whole.sumOfCoefficientInPieces + coefficient > 1.0000001f)
                        throw new IllegalArgumentException("Сумма коэффициентов целой части зашкаливает! Должно быть от 0 до 1 включительно! А получается: " + whole.sumOfCoefficientInPieces + coefficient);
                    else
                        whole.sumOfCoefficientInPieces += coefficient;
                }
            }
        }
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
     * Подписчик на изменения.
     */
    private final ICanUsePercentReady subscriber;
    /**
     * Сумма коэфициентов в частях.
     */
    private float sumOfCoefficientInPieces = 0.0f;

    /**
     * Получает сумму занятых коэффициентов частями.
     * Используйте для того, чтобы вычислить свободное пространство.
     * @return Сумма занятых коэффициентов частями
     */
    public float GetFreeCoefficient() {
        synchronized (sc) {
            return sumOfCoefficientInPieces - Float.MIN_VALUE + 1.0f;
        }
    }

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
        IllegalArgumentException error = null;
        synchronized (sc) { // Блокировка этого состояния.
            if(sumOfCoefficientInPieces != 0.0f)
                throw new IllegalAccessError("Вы потеряли доступ к изменению данного поля. Его можно получить используя его части.");
            if (0.0f <= ready && ready <= 1.0f) {
                if (whole == null)
                    this.ready = ready;
                else {
                    error = this.sendToWhole(ready);
                }
            } else
                error = new IllegalArgumentException("float ready can be only 0.0f ... 1.0f! Argument = " + ready);
        }
        if (error != null)
            throw error;
        if(whole == null)
            if(this.subscriber != null)
                this.subscriber.transferValue(this);
    }

    private IllegalArgumentException sendToWhole(float ready) {
        IllegalArgumentException error = null;
        if(whole != null) {
            synchronized (whole.sc) { // Блокировка целой части.
                // Восстановление значения.
                float wholeWithoutThis = whole.ready - (this.ready * coefficient);
                this.ready = ready;
                float commit = wholeWithoutThis + (this.ready * coefficient);
                if (-0.1f <= commit && commit <= 1.1f) {
                    if (commit > 1.0f)
                        commit = 1.0f;
                    else if (commit < 0.0f)
                        commit = 0.0f;
                    error = whole.sendToWhole(commit);
                } else
                    error = new IllegalArgumentException("I can't send new value: " + commit);
            }
        }
        else { // Это корень! Это и есть сумма всех частей.
            // synchronized sc // уже заблокировано.
            if (0.0f <= ready && ready <= 1.0f)
                this.ready = ready;
            else
                error = new IllegalArgumentException("While i go through wholes: float ready can be only 0.0f ... 1.0f! Argument = " + ready);
        }
        if(error == null)
            if(subscriber != null)
                subscriber.transferValue(this);
        return error;
    }

    @Override
    public String toString() {
        return String.format("%3.1f%%", ready*100f);
    }
}
