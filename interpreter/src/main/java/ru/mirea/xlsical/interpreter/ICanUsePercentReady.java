package ru.mirea.xlsical.interpreter;

/**
 * Интерфейс говорит, что объект умеет принимать изменения percentReady
 */
public interface ICanUsePercentReady {
    /**
     * Вызывается всегда, когда используется setValue.
     * @param percentReady Объект, который был изменён.
     */
    void transferValue(PercentReady percentReady);
}
