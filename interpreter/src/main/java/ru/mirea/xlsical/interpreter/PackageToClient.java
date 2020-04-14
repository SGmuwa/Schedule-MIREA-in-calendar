/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)
    George Andreevich Falileev

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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

    @Override
    public String toString() {
        return "PackageToClient{" +
                "CalFile='" + CalFile + '\'' +
                ", Count=" + Count +
                ", Messages='" + Messages + '\'' +
                '}';
    }
}
