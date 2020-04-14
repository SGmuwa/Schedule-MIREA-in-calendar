package ru.mirea.xlsical.interpreter;

/**
 * Класс указывает правило, какие данные будут переданы серверу.
 * @author <a href="https://github.com/SGmuwa">[SG]Muwa</a>
 */
public class PackageToServer extends Package {

    /**
     * Тут содержатся критерии запроса.
     */
    public final Seeker queryCriteria;
    /**
     * Содержится процент готовности пакета
     */
    public final PercentReady percentReady;

    /**
     * Строит данные отправляемые на сервер.
     * @param ctx Уникальный идентификатор сообщения.
     * @param queryCriteria Тут содержатся критерии запроса.
     */
    public PackageToServer(Object ctx, Seeker queryCriteria) {
        this(ctx, null, queryCriteria);
    }

    /**
     * Строит данные отправляемые на сервер.
     * @param ctx Уникальный идентификатор
     * @param percentReady Ссылка на класс, куда записывать процент готовности.
     * @param queryCriteria Критерии запроса.
     */
    public PackageToServer(Object ctx, PercentReady percentReady, Seeker queryCriteria) {
        super(ctx);
        this.queryCriteria = queryCriteria;
        if(percentReady != null)
            this.percentReady = percentReady;
        else
            this.percentReady = new PercentReady();
    }
}
