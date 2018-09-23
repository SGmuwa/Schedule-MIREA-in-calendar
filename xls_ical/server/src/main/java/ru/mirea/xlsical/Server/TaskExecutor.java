package ru.mirea.xlsical.Server;

import ru.mirea.xlsical.CouplesDetective.ExportCouplesToICal;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.CouplesDetective.Couple;
import ru.mirea.xlsical.CouplesDetective.Detective;
import ru.mirea.xlsical.CouplesDetective.DetectiveException;
import ru.mirea.xlsical.interpreter.Package;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

import java.io.*;
import java.util.*;
import java.util.concurrent.SynchronousQueue;

public class TaskExecutor implements Runnable {

    private final SynchronousQueue<PackageToServer> qIn;
    private final SynchronousQueue<PackageToClient> qOut;

    public TaskExecutor() {
        this.qIn = new SynchronousQueue<>();
        this.qOut = new SynchronousQueue<>();
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

    public PackageToClient poll() {
        return qOut.poll();
    }

    public void pull(PackageToServer pack) {
        qIn.add(pack);
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
