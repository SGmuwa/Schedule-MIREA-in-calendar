/*
Файл указывает правило, какие данные будут переданы серверу.
 */

package ru.mirea.xlsical.interpreter;

import java.io.ByteArrayInputStream;
import java.io.IOException;
import java.io.ObjectInputStream;

public class PackageToServer extends Package {

    /**
     * Тут содержатся пути до файлов .xls и .xlsx.
     * @deprecated Теперь excel файлы будут браться из другого места.
     * Сторона Server будет сама контролировать список excel файлов.
     */
    public final String[] excelsFiles;
    /**
     * Тут содержатся критерии запроса.
     */
    public final Seeker queryCriteria;

    /**
    Строит данные отправляемые на сервер.
     @param ctx Уникальный идентификатор сообщения.
     @param excelsFiles Тут содержатся пути до файлов .xls и .xlsx.
     @param queryCriteria Тут содержатся критерии запроса.
     */
    public PackageToServer(Object ctx, String[] excelsFiles, Seeker queryCriteria) {
        super(ctx);
        this.excelsFiles = excelsFiles;
        this.queryCriteria = queryCriteria;
    }



    /**
     * Преобразует входящий массив байтов в текущее хранилище.
     * @param input Массив байтов, который необходимо перевести в текущий класс.
     * @return Представление хранилища в классе PackageToClient. Если ошибка, то null.
     * @throws ClassNotFoundException Данные не истинные.
     */
    public static PackageToServer fromByteArray(byte[] input) throws ClassNotFoundException {
        try {
            ByteArrayInputStream in = new ByteArrayInputStream(input);
            ObjectInputStream inObj = new ObjectInputStream(in);
            PackageToServer out = (PackageToServer) inObj.readObject();
            inObj.close();
            return out;
        } catch(IOException error) {
            return null;
        }
    }
}
