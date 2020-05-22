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

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ru.mirea.xlsical.CouplesDetective.xl;

namespace ru.mirea.xlsical.CouplesDetective
{
    public class EnumeratorExcelsFileInfo : IEnumerator<ExcelFileInterface>
    {
        public EnumeratorExcelsFileInfo(IEnumerable<FileInfo> excelFiles, bool needIngoreErrors = true)
        {
            enumeratorOfExcels = excelFiles.GetEnumerator();
            NeedIngoreErrors = needIngoreErrors;
        }

        private readonly IEnumerator<FileInfo> enumeratorOfExcels;
        private IEnumerator<ExcelFileInterface> enumeratorInExcel = null;

        public bool NeedIngoreErrors { get; set; }

        public ExcelFileInterface Current => enumeratorInExcel.Current;

        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (enumeratorOfExcels.MoveNext())
            {
                if (enumeratorInExcel != null)
                {
                    enumeratorInExcel.Current.Dispose();
                    if (enumeratorInExcel.MoveNext())
                        return true;
                    enumeratorInExcel.Dispose();
                }
                try
                {
                    enumeratorInExcel = OpenFile.NewInstances(enumeratorOfExcels.Current).GetEnumerator();
                    if (enumeratorInExcel.MoveNext())
                        return true;
                }
                catch (Exception e)
                {
                    if (NeedIngoreErrors)
                    {
                        enumeratorInExcel = null;
                        Console.WriteLine(e.Message + "\nfile: " + enumeratorOfExcels.Current);
                    }
                    else throw;
                }
            }
            return false;
        }

        public void Reset()
        {
            Dispose();
            enumeratorOfExcels.Reset();
        }

        public void Dispose()
        {
            if (enumeratorInExcel != null)
            {
                enumeratorInExcel.Current.Dispose();
                enumeratorInExcel.Dispose();
                enumeratorInExcel = null;
            }
        }
    }
}
