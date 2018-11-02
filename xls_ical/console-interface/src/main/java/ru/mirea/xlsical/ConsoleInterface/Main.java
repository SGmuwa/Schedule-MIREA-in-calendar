package ru.mirea.xlsical.ConsoleInterface;

import ru.mirea.xlsical.interpreter.Seeker;
import ru.mirea.xlsical.interpreter.PackageToServer;
import ru.mirea.xlsical.Server.TaskExecutor;
import ru.mirea.xlsical.interpreter.SeekerType;

import java.io.PrintStream;
import java.time.LocalDate;
import java.time.ZoneId;
import java.util.Arrays;
import java.util.Scanner;

public class Main {
    public static void main(String[] args) {
        Scanner sc = new Scanner(System.in);
        System.out.println(TaskExecutor.monoStep(
                new PackageToServer(
                        null,
                        getFiles(sc, System.out),
                        getSeeker(sc, System.out)
                )
        ).toString());
        sc.next();
    }

    private static String[] getFiles(Scanner sc, PrintStream ps) {
        ps.println("Добавьте Excel файлы через запятую.");
        return sc.nextLine().split(", ?");
    }

    private static Seeker getSeeker(Scanner sc, PrintStream ps) {
        String nameOfSeeker;
        ps.println("Введите имя искателя (номер группы или имя преподавателя):");
        nameOfSeeker = sc.nextLine();
        SeekerType seekerType = getter(sc, ps, Arrays.toString(SeekerType.values()) + "\nВведите тип искателя:", SeekerType::valueOf);
        LocalDate dateStart = getter(sc, ps, "Дата начала составления ics, например, \"" + LocalDate.now().toString() + "\":", LocalDate::parse);
        LocalDate dateFinish = getter(sc, ps, "Дата конца составления ics, например, \"" + LocalDate.now().toString() + "\":", LocalDate::parse);
        ZoneId timezoneStart = getter(sc, ps, "Часовой пояс, например, \"Europe/Minsk\":", ZoneId::of);
        ps.println("Введите адрес по-умолчанию:");
        String defaultAddress = sc.nextLine();
        int startWeek = getter(sc, ps, "Дата начала составления ics указывает на X номер недели, например 1 или 0, где X = ", Integer::parseInt);
        return new Seeker(
                nameOfSeeker,
                seekerType,
                dateStart,
                dateFinish,
                timezoneStart,
                defaultAddress,
                startWeek
        );
    }

    private static <T> T getter(Scanner sc, PrintStream ps, String msg, CanParseInterface<T> parse) {
        T out;
        ps.println(msg);
        do {
            try {
                out = parse.parse(sc.nextLine());
            } catch (Exception e) {
                ps.println(e.getLocalizedMessage() + "\nОшибка чтения. Попробуйте ещё раз.");
                out = null;
            }
        } while (out == null);
        return out;
    }
}