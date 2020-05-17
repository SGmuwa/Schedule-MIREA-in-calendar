package ru.mirea.xlsical;

import ru.mirea.xlsical.Server.TaskExecutor;

import java.util.ArrayDeque;
import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;

public class Main {
    public static int threadNumber = 10;

    public static void main(String[] args) throws Exception
    {
        TaskExecutor taskExecutor = new TaskExecutor();
        Thread[] threadExecutorArr = new Thread[threadNumber];
        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i] = new Thread(taskExecutor);

        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i].start();

    }
}


