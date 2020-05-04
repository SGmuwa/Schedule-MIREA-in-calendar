/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

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

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Класс представляет из себя контейнер полей преподавателя.
    /// </summary>
    public class Teacher
    {
        /// <summary>
        /// Создание экземпляра преподавателя.
        /// </summary>
        /// <param name="fullName">ФИО преподавателя.</param>
        /// <param name="post">Пост преподавателя.</param>
        public Teacher(string fullName, string post)
        {
            this.fullName = fullName != null ? fullName : "";
            this.post = post != null ? post : "";
        }

        public static string[] ConvertNameFromStrToArray(string fullNameStr)
        {
            const int
                    surname = 0, // Фамилия
                    firstName = 1, // Имя
                    patronymic = 2; // Отчество
            string[] fullNameArr = new string[] { "", "", "" };
            int currentFiledName = 0;
            for (int i = 0; i < fullNameStr.Length && currentFiledName <= patronymic; i++)
            {
                if (char.IsUpper(fullNameStr[i]))
                {
                    int indexOfSpace = fullNameStr.IndexOf(' ', i);
                    int indexOfDot = fullNameStr.IndexOf('.', i);
                    int lastIndex = indexOfSpace < indexOfDot ? indexOfSpace : indexOfDot;
                    lastIndex--;
                    fullNameArr[currentFiledName] = fullNameStr.Substring(i, lastIndex - i);
                    i = lastIndex + 2;
                    currentFiledName++;
                }
            }
            return fullNameArr;
        }

        private string surname;
        private string firstName;
        private string patronymic;

        /// <summary>
        /// ФИО преподавателя.
        /// </summary>
        public readonly string fullName;

        /// <summary>
        /// Должность преподавателя.
        /// </summary>
        public readonly string post;

        public override string ToString() => fullName + ' ' + post + '.';
    }
}
