package ru.mirea.xlsical.Server;

import org.apache.poi.openxml4j.exceptions.InvalidFormatException;
import ru.mirea.xlsical.CouplesDetective.CoupleHistorian;
import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.ExportCouplesToICal;
import ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples.*;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;
import ru.mirea.xlsical.CouplesDetective.xl.OpenFile;
import ru.mirea.xlsical.interpreter.PackageToClient;
import ru.mirea.xlsical.interpreter.PackageToServer;

import java.io.Closeable;
import java.io.File;
import java.io.IOException;
import java.time.ZoneId;
import java.time.ZonedDateTime;
import java.util.*;
import java.util.concurrent.BlockingQueue;
import java.util.concurrent.LinkedBlockingQueue;
import java.util.regex.Pattern;
import java.util.regex.PatternSyntaxException;

/**
 * Класс, который выступает в роле исполнителя обработчика.
 * Используйте {@code add} для добавления задания.
 * Используйте {@code take} для получения ответа.
 *
 * @see #add(PackageToServer)
 * @see #take()
 */
public class TaskExecutor implements Runnable {

    private final BlockingQueue<PackageToServer> qIn;
    private final BlockingQueue<PackageToClient> qOut;
    private final CoupleHistorian coupleHistorian;

    public TaskExecutor() {
        this.qIn = new LinkedBlockingQueue<>();
        this.qOut = new LinkedBlockingQueue<>();
        this.coupleHistorian = new CoupleHistorian();
    }

    /**
     * Получает готовый элемент из очереди и удаляет его из очереди.
     * Если выходная очередь пуста, то ждёт появления элемента.
     * В случае, если ожидание прервать, то сработает исключение {@link java.lang.InterruptedException}.
     * @return Пакет от обработчика.
     */
    public PackageToClient take() throws InterruptedException {
        return qOut.take();
    }

    /**
     * Добавляет элемент в очередь задач.
     * @param pack Пакет с требованиями к решению задачи.
     */
    public void add(PackageToServer pack) {
        qIn.add(pack);
    }

    /**
     * Запускает выполнение задач до тех пор, пока не вызовется {@code interrupt} потока.
     * По факту - циклический вызов {@link #step()}.
     */
    @Override
    public void run() {
        while(!Thread.currentThread().isInterrupted())
            try {
                step();
            } catch (InterruptedException e){
                return;
            }
    }

    /**
     * Берёт из входной очереди (see how to {@link #add(PackageToServer) add} to input queue) входной элемент,
     * и отправляет его в выходную очередь (see how to {@link #take() take} from queue).
     * @throws InterruptedException Срабатывает исключение в случае прерывания ожидания из входной очереди.
     */
    public void step() throws InterruptedException {
        qOut.add(monoStep(qIn.take()));
    }

    /**
     * Выполняет обработку пакета без использования очередей. <p/>
     * <u>Не рекомендуется использовать</u> данный метод,
     * так как он заставляет ждать выполнения работы входной поток.
     * Если входящих пакетов из интернета будет приходить быстрее,
     * чем обрабатываться, то нет гарантий на сохранность входных пакетов.
     * @param pkg Пакет с требованиями к решению задачи.
     * @return Пакет от обработчика.
     */
    public PackageToClient monoStep(PackageToServer pkg) {
        if(pkg == null)
            return new PackageToClient(null, null, 0, "Ошибка: была предпринята попытка обработать пустой пакет.");
        if(pkg.queryCriteria == null)
            return new PackageToClient(pkg.ctx, null, 0, "Ошибка: отстствуют критерии поиска.");
        if(pkg.excelsFiles != null && pkg.excelsFiles.length > 0) {
            System.out.println("Попытка использовать forceStep из monoStep!");
            return forceStep(pkg);
        }
        List<CoupleInCalendar> couples = coupleHistorian.getCouples(pkg.queryCriteria);
        return new PackageToClient(
                pkg.ctx,
                ExportCouplesToICal.start(couples),
                couples.size(),
                "ok.");
    }

    public static PackageToClient forceStep(PackageToServer pkg) {
        if(pkg.excelsFiles == null || pkg.excelsFiles.length == 0) {
            return new PackageToClient(pkg.ctx, null, 0, "Error: use Step! I did not find any excel files!");
        }
        DetectiveDate detectiveDate = new DetectiveDate();
        ZonedDateTime now = ZonedDateTime.now();
        List<CoupleInCalendar> couples = new LinkedList<>();
        ArrayList<IDetective> fs = new ArrayList<>();
        try {
                for(ExcelFileInterface file : openExcelFiles(pkg.excelsFiles)) {
                    fs.add(Detective.chooseDetective(file, detectiveDate));
                }
                for(IDetective detective : fs) {
                    try {
                        couples.addAll(detective.startAnInvestigation(detective.getStartTime(now), detective.getFinishTime(now)));
                    } catch (DetectiveException exD) {
                        // В случае, если один из файлов не правильно оформлен, то его игнорируем.
                        System.out.println("DetectiveException");
                        fs.remove(detective);
                    }
                }

        } catch (IOException error) {
            error.printStackTrace();
            pkg.percentReady.setReady(1);
            return new PackageToClient(pkg.ctx, null, 0, "Ошибка внутри сервера.");
        } finally {
            if (fs != null)
                for (Closeable file : fs)
                    if (file != null)
                        try {
                            file.close();
                        } catch (IOException err) {
                            //err.printStackTrace();
                            System.out.println("Can't close file into error.");
                        }
        }
        try {
            Pattern pattern = Pattern.compile(pkg.queryCriteria.nameOfSeeker);
            couples.removeIf((elm) -> !pattern.matcher(elm.nameOfGroup).find()
                    && !pattern.matcher(elm.nameOfTeacher).find());
        } catch (PatternSyntaxException e) {
            couples.removeIf((elm) -> !pkg.queryCriteria.nameOfSeeker.equals(elm.nameOfTeacher)
            && !pkg.queryCriteria.nameOfSeeker.equals(elm.nameOfGroup));
        }
        return new PackageToClient(
                pkg.ctx,
                ExportCouplesToICal.start(couples),
                couples.size(),
                "ok.");
    }

    /**
     * Открывает все excel файлы, которые были переданы из массива путей до файлов.
     * @param filesStr Массив строк, который символизируют путь до файлов Excel.
     * @return Массив-список открытых Excel файлов.
     */
    private static ArrayList<ExcelFileInterface> openExcelFiles(String[] filesStr) {
        ArrayList<ExcelFileInterface> output = new ArrayList<>(filesStr.length);
        for(int index = filesStr.length - 1; index >= 0; index--) {
            output.addAll(openExcelFiles(filesStr[index]));
        }
        return output;
    }

    /**
     * Открывает все Excel файлы, которые были переданы в колекции путей до файлов.
     * @param filesStr Коллекция строк, который символизируют путь до файлов Excel.
     * @return Массив-список открытых Excel файлов.
     */
    private static ArrayList<ExcelFileInterface> openExcelFiles(Collection<? extends String> filesStr) {
        int size = filesStr.size();
        ArrayList<ExcelFileInterface> output = new ArrayList<>(size);
        for(String str : filesStr) {
            output.addAll(openExcelFiles(str));
        }
        return output;
    }

    /**
     * Открывает все Excel файлы, которые были переданы из получателя путей до файлов.
     * @param filesStr Перечеслитель строк, который символизируют путь до файлов Excel.
     * @return Связный-список открытых Excel файлов.
     */
    private static LinkedList<ExcelFileInterface> openExcelFiles(Iterable<? extends String> filesStr) {
        LinkedList<ExcelFileInterface> output = new LinkedList<>();
        for(String str : filesStr) {
            output.addAll(openExcelFiles(str));
        }
        return output;
    }

    /**
     * Открывает Excel файл, который был передан по fileStr.
     * @param fileStr Путь до файла Excel.
     * @return Список открытых Excel файлов.
     */
    private static ArrayList<? extends ExcelFileInterface> openExcelFiles(String fileStr) {
        File a = new File(fileStr);
        if(!a.canRead()) {
            System.out.println("TaskExecutor::openExcelFiles(String fileStr) - can't reed file " + fileStr);
            return new ArrayList<>();
        }
        try {
            return OpenFile.newInstances(a.getAbsolutePath());
        } catch (IOException | InvalidFormatException error) {
            error.printStackTrace();
            return new ArrayList<>();
        }
    }
}
