package ru.mirea.xlsical.CouplesDetective;

import ru.mirea.xlsical.interpreter.PercentReady;
import ru.mirea.xlsical.interpreter.SampleConsoleTransferPercentReady;

public class GlobalPercentReady {
    public static final PercentReady percentReady =
            new PercentReady(
                    new SampleConsoleTransferPercentReady("Test: ")
            );
}
