package ru.mirea.xlsical.interpreter;

import java.io.*;

public abstract class Package implements Serializable {

    public Package(Object ctx) {
        this.ctx = ctx;
    }

    /**
     * Уникальный индентификатор сообщения.
     */
    public final Object ctx;

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
