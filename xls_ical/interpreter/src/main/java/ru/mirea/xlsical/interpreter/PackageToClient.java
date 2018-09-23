/*
Файл указывает правило, какие данные будут переданы клиенту.
 */

package ru.mirea.xlsical.interpreter;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.ObjectInputStream;

public class PackageToClient extends Package {

    /**
     * Тут содержатся путь до файла "*.iCal".
     */
    public final String CalFile;

    /**
     * Тут содержится количество созданных мероприятий.
     */
    public final int Count;

    /**
     * Тут содержится сообщение от сервера. Например: "Найдено несколько преподавателей с этим именем: Иванов И.И. и Иванов И.А."
     */
    public final String Messages;

    /**
    Строит данные отправляемые на клиент.
     @param ctx Уникальный идентификатор сообщения.
     @param CalFile Тут содержатся файл .iCal.
     @param Count Тут содержится количество созданных мероприятий.
     @param Messages Сообщение от обработчика пользователю клиента.
     */
    public PackageToClient(Object ctx, String CalFile, int Count, String Messages) {
        super(ctx);
        this.CalFile = CalFile;
        this.Count = Count;
        this.Messages = Messages;
    }



    /**
     * Преобразует входящий массив байтов в текущее хранилище.
     * @param input Массив байтов, который необходимо перевести в текущий класс.
     * @return Представление хранилища в классе PackageToClient. Если ошибка, то null.
     * @throws ClassNotFoundException Данные не истинные.
     */
    public static PackageToClient fromByteArray(byte[] input) throws ClassNotFoundException {
        try {
            ByteArrayInputStream in = new ByteArrayInputStream(input);
            ObjectInputStream inObj = new ObjectInputStream(in);
            PackageToClient out = (PackageToClient) inObj.readObject();
            inObj.close();
            return out;
        } catch(IOException error) {
            return null;
        }
    }
}
