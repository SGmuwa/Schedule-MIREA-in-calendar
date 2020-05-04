/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  
    Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

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

using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ru.mirea.xlsical.CouplesDetective
{
    public static class SaverLoaderClass
    {
        /// <summary>
        /// Экземпляр реализации записи объектов в файлы.
        /// </summary>
        private static readonly BinaryFormatter binaryFormatter = new BinaryFormatter();

        /// <summary>
        /// Записывает входной класс в бинарный файл.
        /// <para>Все члены класса должны поддерживать атрибут [<see cref="System.SerializableAttribute"/>].</para>
        /// </summary>
        /// <typeparam name="T">Тип, который используется для записи. Должен поддерживать атрибут [<see cref="System.SerializableAttribute"/>].</typeparam>
        /// <param name="fileInfo">Путь до файла, в который надо записать объект <paramref name="objectToWrite"/>.</param>
        /// <param name="objectToWrite">Объект, который надо записать в файл.</param>
        /// <param name="append"><c>true</c> — добавит в текущий файл объект.
        /// <c>false</c> — перезапишет объектом файл. По-умолчанию <c>false</c>.</param>
        public static void WriteToBinaryFile<T>(FileInfo fileInfo, T objectToWrite, bool append = false)
        {
            using Stream stream = File.Open(fileInfo.FullName, append ? FileMode.Append : FileMode.Create);
            binaryFormatter.Serialize(stream, objectToWrite);
        }

        /// <summary>
        /// Считывает объект типа <typeparamref name="T"/> из файла по пути <paramref name="fileInfo"/>.
        /// </summary>
        /// <typeparam name="T">Тип, который используется для записи.</typeparam>
        /// <param name="fileInfo">Местоположение файла, с которого надо произвести чтение.</param>
        /// <returns>Возвращает новый экземпляр, полученный из файла.</returns>
        public static T ReadFromBinaryFile<T>(FileInfo fileInfo)
        {
            using Stream stream = File.Open(fileInfo.FullName, FileMode.Open);
            return (T)binaryFormatter.Deserialize(stream);
        }
    }
}