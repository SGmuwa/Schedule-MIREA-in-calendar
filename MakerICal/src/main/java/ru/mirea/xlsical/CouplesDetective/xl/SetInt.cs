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

namespace ru.mirea.xlsical.CouplesDetective.xl
{
    public class SetInt
    {
        public SetInt(int value) => Value = value;

        public SetInt() => Value = 0;

        public int Value { get; set; }

        public void Add(int value) => Value += value;

        public void Add() => Value++;
    }
}
