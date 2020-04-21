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

using System;

namespace ru.mirea.xlsical.CouplesDetective
{
    public static class RandomExtend
    {
        /// <summary>
        /// Генерирует случайное 64-битное знаковое число в пределах от <paramref name="minValue"/>
        /// включительно до <paramref name="maxValue"/> не включительно.
        /// </summary>
        /// <param name="that">Генератор случайных чисел.</param>
        /// <param name="minValue">Минимальное допустимое генерируемое число.</param>
        /// <param name="maxValue">Максимальное недопустимое число.</param>
        /// <returns>Случайное число.</returns>
        /// <remarks>
        /// Исключение: если <paramref name="minValue"/> равен <see cref="long.MinValue"/>
        /// и <paramref name="maxValue"/> равен <see cref="long.MaxValue"/>,
        /// то генерируется случайное число от <paramref name="minValue"/> включительно до
        /// <paramref name="maxValue"/> включительно.
        /// </remarks>
        public static long NextLong(this Random that, long minValue = long.MinValue, long maxValue = long.MaxValue)
        {
            byte[] entropyArray = new byte[sizeof(long)];
            that.NextBytes(entropyArray);
            long entropy = BitConverter.ToInt64(entropyArray);
            return minValue == long.MinValue && maxValue == long.MaxValue
                ? entropy
                : (entropy % (maxValue - minValue)) + minValue;
        }

        /// <summary>
        /// Генерирует случайное 64-битное беззнаковое число в пределах от <paramref name="minValue"/>
        /// включительно до <paramref name="maxValue"/> не включительно.
        /// </summary>
        /// <param name="that">Генератор случайных чисел.</param>
        /// <param name="minValue">Минимальное допустимое генерируемое число.</param>
        /// <param name="maxValue">Максимальное недопустимое число.</param>
        /// <returns>Случайное число.</returns>
        /// <remarks>
        /// Исключение: если <paramref name="minValue"/> равен <see cref="ulong.MinValue"/>
        /// и <paramref name="maxValue"/> равен <see cref="ulong.MaxValue"/>,
        /// то генерируется случайное число от <paramref name="minValue"/> включительно до
        /// <paramref name="maxValue"/> включительно.
        /// </remarks>
        public static ulong NextULong(this Random that, ulong minValue = ulong.MinValue, ulong maxValue = ulong.MaxValue)
        {
            byte[] entropyArray = new byte[sizeof(ulong)];
            that.NextBytes(entropyArray);
            ulong entropy = BitConverter.ToUInt64(entropyArray);
            return minValue == ulong.MinValue && maxValue == ulong.MaxValue
                ? entropy
                : (entropy % (maxValue - minValue)) + minValue;
        }
    }
}
