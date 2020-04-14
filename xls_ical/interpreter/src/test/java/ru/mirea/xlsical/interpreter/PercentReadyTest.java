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

package ru.mirea.xlsical.interpreter;

import org.junit.Test;

import static org.junit.Assert.*;

public class PercentReadyTest {

    @Test
    public void startSimplePercent() {
        PercentReady pr = new PercentReady();
        assertEquals(0.0f, pr.getReady(), 0f);
        pr.setReady(0.5f);
        assertEquals(0.5f, pr.getReady(), 0f);
        try {
            pr.setReady(2f);
            fail();
        } catch (IllegalArgumentException e) {
            assertEquals(0.5f, pr.getReady(), 0f);
        }
        pr.setReady(1.0f);
        assertEquals(1.0f, pr.getReady(), 0.0f);
    }

    @Test
    public void startWholePercent() throws InterruptedException {
        PercentReady whole = new PercentReady(new SampleConsoleTransferPercentReady());
        Thread a = new Thread(() -> function1(new PercentReady(whole, 1f/11f)));
        Thread b = new Thread(() -> function2(new PercentReady(whole, 10f/11f)));
        a.start();
        b.start();
        System.out.println("wait a.");
        a.join();
        System.out.println("a ready, wait b.");
        b.join();
        System.out.println("b ready");
    }

    private void function1(PercentReady pr) {
        pr.setReady(0.0f);
        for(int i = 0; i < 10000; i++) {
            pr.setReady((float)i/9999.0f);
        }
    }

    private void function2(PercentReady pr) {
        pr.setReady(0.0f);
        for(int i = 0; i < 100000; i++) {
            pr.setReady((float)i/99999.0f);
        }
    }

    @Test
    public void testSimpleFail() {
        try {
            new PercentReady().setReady(2);
            fail();
        } catch (IllegalArgumentException e) {/*good!*/}
        try {
            new PercentReady(null, 2.0f);
            fail();
        } catch (IllegalArgumentException e) {/*good!*/}
        try {
            PercentReady big = new PercentReady();
            new PercentReady(big, 0.5f);
            new PercentReady(big, 0.6f);
            fail();
        } catch (IllegalArgumentException e) {/*good!*/}

        try {
            PercentReady big = new PercentReady();
            new PercentReady(big, 0.5f);
            big.setReady(1.0f);
            fail();
        } catch (IllegalAccessError e) {/*good!*/}
    }
}
