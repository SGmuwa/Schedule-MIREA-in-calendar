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

using ru.mirea.xlsical.interpreter;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Класс-делегад утверждает, что метод умеет обновлять кэш.
    /// Вызывается, когда необходимо обработать новое обновление.
    /// </summary>
    /// <param name="pr">Доступ к управлению передачи данных о статусе задачи.</param>
    /// <exception cref="System.IO.IOException">В процессе работы с файловой системой произошла ошибка.</exception>
    public delegate void UpdateCache(PercentReady pr);
}
