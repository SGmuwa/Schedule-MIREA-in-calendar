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
using System.IO;

namespace ru.mirea.xlsical.CouplesDetective
{
    public class StreamWithEvent : Stream, System.IDisposable
    {
        private readonly Stream baseStream;

        /// <summary>
        /// object: this
        /// Exception: null if good. Else — exception.
        /// </summary>
        public event Action<object, Exception> Disposed;

        public StreamWithEvent(Stream baseStream) => this.baseStream = baseStream;

        public new void Dispose()
        {
            try
            {
                base.Dispose();
            }
            catch (Exception e)
            {
                Disposed?.Invoke(this, e);
                throw;
            }
            Disposed?.Invoke(this, null);
        }

        #region abstract class
        public override bool CanRead => baseStream.CanRead;
        public override bool CanSeek => baseStream.CanSeek;
        public override bool CanWrite => baseStream.CanWrite;
        public override long Length => baseStream.Length;
        public override long Position { get => baseStream.Position; set => baseStream.Position = value; }
        public override void Flush() => baseStream.Flush();
        public override int Read(byte[] buffer, int offset, int count) => baseStream.Read(buffer, offset, count);
        public override long Seek(long offset, SeekOrigin origin) => baseStream.Seek(offset, origin);
        public override void SetLength(long value) => baseStream.SetLength(value);
        public override void Write(byte[] buffer, int offset, int count) => baseStream.Write(buffer, offset, count);
        #endregion
    }
}