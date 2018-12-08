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
        PercentReady whole = new PercentReady(new ICanUsePercentReady() {
            String old = "";

            @Override
            public void transferValue(PercentReady pr) {
                String newStr = pr.toString();
                if(!newStr.equals(old)) {
                    old = newStr;
                    System.out.println(newStr);
                }
            }
        });
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
