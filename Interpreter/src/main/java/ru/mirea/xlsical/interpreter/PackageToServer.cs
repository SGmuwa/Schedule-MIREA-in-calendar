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

namespace ru.mirea.xlsical.interpreter
{
    /// <summary>
    /// Класс указывает правило, какие данные будут переданы серверу.
    /// </summary>
    public class PackageToServer : Package
    {
        /// <summary>
        /// Критерии запроса.
        /// </summary>
        public readonly Seeker queryCriteria;

        /// <summary>
        /// Процент готовности пакета.
        /// </summary>
        public readonly PercentReady percentReady;

        /// <summary>
        /// Строит данные отправляемые на сервер.
        /// </summary>
        /// <param name="ctx">Уникальный идентификатор или контекст сообщения.</param>
        /// <param name="queryCriteria">Критерии запроса.</param>
        /// <param name="percentReady">Ссылка на класс, куда записывать процент готовности.</param>
        public PackageToServer(object ctx, Seeker queryCriteria, PercentReady percentReady = null)
        : base(ctx)
        {
            this.queryCriteria = queryCriteria;
            this.percentReady = percentReady == null ? new PercentReady() : percentReady;
        }
    }
}
