package ru.mirea.xlsical.interpreter;

import java.io.*;

public abstract class Package implements Serializable {

    /**
     * Создание экземпляра пакета
     * @param ctx Контекст задачи.
     * @param percentReady Ссылка на поле float, куда надо отправить % готовности задачи.
     */
    public Package(Object ctx, PercentReady percentReady) {
        this.ctx = ctx;
        this.percentReady = percentReady;
    }

    /**
     * Уникальный индентификатор сообщения.
     */
    public final Object ctx;

    /**
     * Содержится процент готовности пакета
     */
    public final PercentReady percentReady;

    /**
     * Преобразует текущий класс в поток байтов.
     * @return Хранилище данного класса в виде байтов.
     */
    public byte[] toByteArray() {
        try {
            ByteArrayOutputStream out = new ByteArrayOutputStream();
            ObjectOutputStream outObj = new ObjectOutputStream(out);


            // conversion from "yourObject" to byte[]
            outObj.writeObject(this);
            outObj.flush();
            outObj.close();
            return out.toByteArray();
        }
        catch (IOException error){
            return new byte[]{};
        }
    }
}
