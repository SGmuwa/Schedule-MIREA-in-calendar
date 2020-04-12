package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.interpreter.PercentReady;

import java.io.IOException;

/**
 * Интерфейс утверждает, что объект умеет обновлять.
 */
public interface ICacheUpdater {
    /**
     * Вызывается, когда необходимо обработать новое обновление.
     * @throws IOException В процессе работы с файловой системой произошла ошибка.
     */
    void update(PercentReady pr) throws IOException;
}
