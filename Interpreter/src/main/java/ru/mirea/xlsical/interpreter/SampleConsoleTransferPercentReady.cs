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

namespace ru.mirea.xlsical.interpreter
{
    public class SampleConsoleTransferPercentReady
    {
        public SampleConsoleTransferPercentReady(string message = "", bool needNewLine = true, bool autoFlush = false)
        {
            this.Message = message;
            this.NeedNewLine = needNewLine;
            this.AutoFlush = autoFlush;
        }

        private string oldValue;

        private string message;

        /// <summary>
        /// True, если нужно при печати новая строка.
        /// </summary>
        public bool NeedNewLine { get; set; }

        /// <summary>
        /// Сообщение перед выводом процента загрузки.
        /// </summary>
        public string Message
        {
            get => message;
            set
            {
                oldValue = null;
                message = value;
            }
        }

        public bool AutoFlush { get; set; }

        /// <summary>
        /// Вызывается всегда, когда используется setValue.
        /// </summary>
        /// <param name="pr">Объект, который был изменён.</param>
        public void TransferValue(PercentReady pr)
        {
            string newValue = pr.ToString();
            if (!newValue.Equals(oldValue))
            {
                oldValue = newValue;
                Console.Write(Message + newValue + (NeedNewLine ? Environment.NewLine : String.Empty));
                if (AutoFlush)
                    Console.Out.Flush();
            }
        }
    }
}
