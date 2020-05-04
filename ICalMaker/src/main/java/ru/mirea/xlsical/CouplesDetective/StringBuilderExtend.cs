/*
    Schedule MIREA in calendar.
    Copyright (C) 2020
    Dennis Roche
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

using System;
using System.Text;

namespace ru.mirea.xlsical.CouplesDetective
{
    public static class StringBuilderExtend
    {
        /// <summary>
        /// Returns the index of the start of the contents in a <see cref="StringBuilder"/>.
        /// </summary>
        /// <param name="that">The <see cref="StringBuilder"/> in which the search will be performed.</param>
        /// <param name="value">The string to find.</param>
        /// <param name="startIndex">The starting index.</param>
        /// <param name="ignoreCase">If set to <c>true</c> it will ignore case.</param>
        /// <returns>
        /// The zero-based index position of <paramref name="value"/> if that
        /// string is found, or -1 if it is not. If <paramref name="value"/> is
        /// <see cref="string.Empty"/>, the return value is 0.
        /// </returns>
        public static int IndexOf(this StringBuilder that, string value, int startIndex = 0, bool ignoreCase = true)
        {
            if (that == null)
                throw new ArgumentNullException(nameof(that));
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value.Length == 0)
                return 0;

            int index;
            int length = value.Length;
            int maxSearchLength = (that.Length - length) + 1;

            if (ignoreCase)
            {
                for (int i = startIndex; i < maxSearchLength; ++i)
                    if (char.ToLower(that[i]) == char.ToLower(value[0]))
                    {
                        index = 1;
                        while ((index < length) && (char.ToLower(that[i + index]) == char.ToLower(value[index])))
                            ++index;

                        if (index == length)
                            return i;
                    }
                return -1;
            }

            for (int i = startIndex; i < maxSearchLength; ++i)
                if (that[i] == value[0])
                {
                    index = 1;
                    while ((index < length) && (that[i + index] == value[index]))
                        ++index;

                    if (index == length)
                        return i;
                }

            return -1;
        }

        public static string Substring(this StringBuilder that, in int indexStart = 0, in int length = 0)
        {
            if (that == null)
                throw new ArgumentNullException(nameof(that));
            if (indexStart < 0 || that.Length >= indexStart)
                throw new ArgumentOutOfRangeException(nameof(indexStart));
            if (length < 0 || indexStart + length > that.Length)
                throw new ArgumentOutOfRangeException(nameof(length));
            char[] array = new char[length];
            for (int i = indexStart + length - 1; i >= indexStart; --i)
                array[i] = that[i + indexStart];
            return new string(array);
        }
    }
}
