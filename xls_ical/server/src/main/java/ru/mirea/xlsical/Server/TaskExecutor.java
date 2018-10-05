package ru.mirea.xlsical.Server;

import ru.mirea.xlsical.CouplesDetective.ExportCouplesToICal;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.CouplesDetective.Couple;
import ru.mirea.xlsical.CouplesDetective.Detective;
import ru.mirea.xlsical.CouplesDetective.DetectiveException;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

import java.io.*;
import java.util.*;
import java.util.concurrent.ConcurrentLinkedQueue;

public class TaskExecutor implements Runnable {

    private final ConcurrentLinkedQueue<PackageToServer> qIn;
    private final ConcurrentLinkedQueue<PackageToClient> qOut;

    public TaskExecutor() {
        this.qIn = new ConcurrentLinkedQueue<>();
        this.qOut = new ConcurrentLinkedQueue<>();
    }

    /**
     * Запускает выполнение задач.
     */
    @Override
    public void run() {
        while(!Thread.currentThread().isInterrupted())
            step();
    }

    public void step() {
        List<Couple> couples;
        PackageToServer inputP;
        do inputP = qIn.poll(); while (inputP == null);
        if(inputP.excelsFiles == null) {
            qOut.add(new PackageToClient(inputP.ctx, null, 0, "Ошибка внутри сервера."));
            return;
        }
        Collection<ExcelFileInterface> fs = null;
        try {
            fs = openExcelFiles(inputP.excelsFiles);
            couples = Detective.startAnInvestigations(inputP.queryCriteria, fs);
        } catch (IOException error) {
            qOut.add(new PackageToClient(inputP.ctx, null, 0, "Ошибка внутри сервера."));
            error.printStackTrace();
            return;
        } catch (DetectiveException error) {
            qOut.add(new PackageToClient(inputP.ctx, null, 0, error.getMessage()));
            return;
        } finally {
            if(fs != null)
            for(Closeable file : fs)
                if(file != null)
                    try {
                        file.close();
                    }
                    catch (IOException err) {
                        err.printStackTrace();
                        System.out.println("Can't close file into error.");
                    }
        }
        qOut.add(new PackageToClient(
                inputP.ctx,
                ExportCouplesToICal.start(couples),
                couples.size(),
                "ok."));
    }

    /**
     * Вытаскивает элемент из очереди и удаляет его. Возвращает null, если очередь пуста.
     * @return Пакет от обработчика.
     */
    public PackageToClient poll() {
        return qOut.poll();
    }

    /**
     * Добавляет элемент в очередь задач.
     * @param pack Пакет с требованиями к решению задачи.
     * @return true (as specified by Collection.add).
     */
    public boolean add(PackageToServer pack) {
        return qIn.add(pack);
    }

    private ArrayList<ExcelFileInterface> openExcelFiles(String[] filesStr) {
        ArrayList<ExcelFileInterface> output = new ArrayList<>(filesStr.length);
        for(int index = filesStr.length - 1; index >= 0; index--) {
            output.add(openExcelFiles(filesStr[index]));
        }
        return output;
    }

    private ArrayList<ExcelFileInterface> openExcelFiles(Collection<? extends String> filesStr) {
        int size = filesStr.size();
        ArrayList<ExcelFileInterface> output = new ArrayList<>(size);
        for(String str : filesStr) {
            output.add(openExcelFiles(str));
        }
        return output;
    }

    private LinkedList<ExcelFileInterface> openExcelFiles(Iterable<? extends String> filesStr) {
        LinkedList<ExcelFileInterface> output = new LinkedList<>();
        for(String str : filesStr) {
            output.add(openExcelFiles(str));
        }
        return output;
    }

    private ExcelFileInterface openExcelFiles(String fileStr) {
        File a = new File(fileStr);
        if(!a.canRead()) {
            System.out.println("TaskExecutor::openExcelFiles(String fileStr) - can't reed file " + fileStr);
            return null;
        }
        try {
            return new OpenFile(a.getAbsolutePath());
        } catch (IOException error) {
            error.printStackTrace();
            return null;
        }
    }
}
