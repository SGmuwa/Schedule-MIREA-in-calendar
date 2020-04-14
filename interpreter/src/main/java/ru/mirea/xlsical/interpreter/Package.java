package ru.mirea.xlsical.interpreter;

public abstract class Package {

    /**
     * Создание экземпляра пакета
     * @param ctx Контекст задачи.
     */
    public Package(Object ctx) {
        this.ctx = ctx;
    }

    /**
     * Уникальный индентификатор сообщения.
     */
    public final Object ctx;
}
