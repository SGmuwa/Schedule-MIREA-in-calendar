package ru.mirea.xlsical;

import ru.mirea.xlsical.Server.TaskExecutor;

import java.util.ArrayDeque;
import java.util.Queue;
import java.util.concurrent.LinkedBlockingQueue;

public class Main {
    public static int threadNumber = 10;

    public static void main(String[] args) throws Exception
    {
        Queue<ID_Pack> qIn = new LinkedBlockingQueue<ID_Pack>();
        Queue<ID_Pack> qOut = new ArrayDeque<ID_Pack>();

        TaskExecutor taskExecutor = new TaskExecutor(qIn, qOut);
        Thread[] threadExecutorArr = new Thread[threadNumber];
        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i] = new Thread(taskExecutor);

        for (int i=0; i<threadNumber;++i)
            threadExecutorArr[i].start();

    }
}


