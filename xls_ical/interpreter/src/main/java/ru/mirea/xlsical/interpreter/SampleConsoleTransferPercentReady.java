package ru.mirea.xlsical.interpreter;

public class SampleConsoleTransferPercentReady implements ICanUsePercentReady {

    public SampleConsoleTransferPercentReady() {

    }
    public SampleConsoleTransferPercentReady(String message) {
        this.message = message;
    }

    private String oldValue = "";
    private String message = "";
    /**
     * Вызывается всегда, когда используется setValue.
     *
     * @param pr Объект, который был изменён.
     */
    @Override
    public void transferValue(PercentReady pr) {
        String newValue = pr.toString();
        if(!newValue.equals(oldValue)) {
            oldValue = newValue;
            System.out.println(message + newValue);
        }
    }
}
