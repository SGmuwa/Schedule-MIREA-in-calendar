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
