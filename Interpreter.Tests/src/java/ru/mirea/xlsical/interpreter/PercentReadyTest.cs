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

using Xunit;
using System;
using System.Threading;

namespace ru.mirea.xlsical.interpreter
{
    public class PercentReadyTest
    {
        [Fact]
        public void StartSimplePercent()
        {
            PercentReady pr = new PercentReady();
            Assert.Equal(0.0, pr.Ready, 10);
            pr.Ready = 0.5f;
            Assert.Equal(0.5f, pr.Ready, 10);
            Assert.Throws<ArgumentException>(() => pr.Ready = 2f);
            Assert.Equal(0.5, pr.Ready, 10);
            pr.Ready = 1.0f;
            Assert.Equal(1.0, pr.Ready, 10);
        }

        [Fact]
        public void StartWholePercent()
        {
            PercentReady whole = new PercentReady(subscribers: new SampleConsoleTransferPercentReady("   \r", false, true).TransferValue);
            Thread a = new Thread(() => Function1(new PercentReady(whole, 1f / 11f)));
            Thread b = new Thread(() => Function2(new PercentReady(whole, 10f / 11f)));
            a.Start();
            b.Start();
            Console.WriteLine("wait a.");
            a.Join();
            Console.WriteLine("a ready, wait b.");
            b.Join();
            Console.WriteLine("b ready");
        }

        private void Function1(PercentReady pr)
        {
            pr.Ready = 0.0f;
            for (int i = 0; i < 10000; i++)
                pr.Ready = i / 9999.0f;
        }

        private void Function2(PercentReady pr)
        {
            pr.Ready = 0.0f;
            for (int i = 0; i < 100000; i++)
                pr.Ready = i / 99999.0f;
        }

        [Fact]
        public void TestSimpleFail()
        {
            Assert.Throws<ArgumentException>(() => new PercentReady().Ready = 2);

            Assert.Throws<ArgumentException>(() => new PercentReady(null, 2.0f));

            PercentReady big = new PercentReady();
            new PercentReady(big, 0.5f);
            Assert.Throws<ArgumentException>(() => new PercentReady(big, 0.6f));

            big = new PercentReady();
            new PercentReady(big, 0.5f);
            Assert.Throws<ArgumentException>(() => big.Ready = 1.0f);
        }
    }
}
