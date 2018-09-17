package ru.mirea.xlsical.Server;

import io.netty.channel.ChannelHandlerContext;
import ru.mirea.xlsical.CouplesDetective.ExportCouplesToICal;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.CouplesDetective.Couple;
import ru.mirea.xlsical.CouplesDetective.Detective;
import ru.mirea.xlsical.CouplesDetective.DetectiveException;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

import java.io.*;
import java.util.ArrayList;
import java.util.List;
import java.util.Queue;
import java.util.Random;

public class TaskExecutor implements Runnable {

    private static final File pathToTemp;
    private static final Random ran = new Random();
    private Queue<ID_Pack> qIn;
    private Queue<ID_Pack> qOut;

    static {
        pathToTemp = new File(System.getProperty("java.io.tmpdir") + File.separator + "gosha");
        if (!pathToTemp.exists()) {
            pathToTemp.mkdir();
        } else {
            deleteDir(pathToTemp);
            pathToTemp.mkdir();
        }
    }

    private static void deleteDir(File file) {
        File[] contents = file.listFiles();
        if (contents != null) {
            for (File f : contents) {
                deleteDir(f);
            }
        }
        file.delete();
    }

    public TaskExecutor(Queue<ID_Pack> qIn, Queue<ID_Pack> qOut) {
        this.qIn = qIn;
        this.qOut = qOut;
    }

    @Override
    public void run() {
        while(!Thread.currentThread().isInterrupted()) step();
    }

    public void step() {
        ID_Pack message;
        List<Couple> couples;
        message = poll();
        PackageToServer a;
        Object ctx = message.id;
        if(message.data instanceof PackageToServer)
            a = (PackageToServer) message.data;
        else {
            System.out.println("Error to convert to PackageToServer.");
            return;
        }
        try {
            couples = Detective.startAnInvestigations(a.QueryCriteria, extractsBytes(a.ExcelsFiles));
        } catch (IOException error) {
            pull(new ID_Pack(ctx, new PackageToClient(new byte[0], 0, "Ошибка внутри сервера.")));
            System.out.println("[ERROR] Почему папка temp не доступна??");
            return;
        } catch (DetectiveException error) {
            pull(new ID_Pack(ctx, new PackageToClient(new byte[0], 0, error.getMessage())));
            return;
        }
        String out = ExportCouplesToICal.start(couples);
        pull(new ID_Pack(ctx, new PackageToClient(out.getBytes(), couples.size(), "ok.")));
    }

    private ID_Pack poll() {
        synchronized (qIn) {
            ID_Pack out;
            do {
                out = qIn.poll();
            } while (out == null);
            return out;
        }
    }

    private void pull(ID_Pack pack) {
        synchronized (qOut) {qOut.add(pack);}
    }

    private ArrayList<ExcelFileInterface> extractsBytes(byte[][] files) {
        ArrayList<ExcelFileInterface> output = new ArrayList<>(files.length);
        for(int index = files.length - 1; index >= 0; index--) {
            output.add(extractsBytes(files[index]));
        }
        return output;
    }

    private ExcelFileInterface extractsBytes(byte[] file) {
        File a = new File(pathToTemp, ran.nextInt() + "" + ran.nextInt() + ".xlsxlsx.bin");
        try {
            a.createNewFile();
        } catch (IOException error) {
            return null;
        }
        try(FileOutputStream out = new FileOutputStream(a)) {
            out.write(file);
            return new OpenFile(a.getAbsolutePath());
        } catch (IOException error) {
            return null;
        }
    }
}
