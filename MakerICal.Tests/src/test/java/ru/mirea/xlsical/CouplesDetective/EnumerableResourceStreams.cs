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

using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ru.mirea.xlsical.CouplesDetective
{
    internal class EnumerableResourceStreams : IEnumerable<Stream>
    {
        private IEnumerable<string> streamsNames;

        private static Assembly assembly = Assembly.GetExecutingAssembly();

        public EnumerableResourceStreams(IEnumerable<string> streamsNames)
            => this.streamsNames = streamsNames;

        public IEnumerator<Stream> GetEnumerator()
            => (from a in streamsNames select assembly.GetManifestResourceStream(a)).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
