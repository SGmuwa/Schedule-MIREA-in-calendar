package ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;

import ru.mirea.xlsical.CouplesDetective.Couple;
import ru.mirea.xlsical.CouplesDetective.CoupleInCalendar;
import ru.mirea.xlsical.CouplesDetective.xl.ExcelFileInterface;

import java.awt.*;
import java.io.IOException;
import java.time.*;
import java.time.temporal.ChronoField;
import java.time.temporal.ChronoUnit;
import java.util.*;
import java.util.List;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

/**
 * Данный класс отвечает за получение календарных пар из Excel расписания.
 * Умеет читать только семестровое расписание.
 * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>.
 */
public class DetectiveSemester extends Detective {

    public DetectiveSemester(ExcelFileInterface file, DetectiveDate dateSettings) {
        super(file, dateSettings);
    }

    /**
     * @deprecated
     * @see #DetectiveSemester(ExcelFileInterface, DetectiveDate)
     */
    public DetectiveSemester(ExcelFileInterface file) {
        super(file);
    }

    /**
     * Функция ищет занятия для seeker в файле File.
     *
     * @param start  Дата и время начала составления расписания.
     * @param finish Дата и время конца составления раписания.
     * @param startWeek Номер недели в день {@code start}.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
     * @throws IOException        Во время работы с Excel file - файл стал недоступен.
     */
    public LinkedList<CoupleInCalendar> startAnInvestigation(ZonedDateTime start, ZonedDateTime finish, int startWeek) throws DetectiveException, IOException {
        LinkedList<CoupleInExcel> couplesInExcel = startViewer();
        LinkedList<CoupleInCalendar> out = new LinkedList<>();
        for (CoupleInExcel line : couplesInExcel) {
            out.addAll(SetterCouplesInCalendar.getCouplesByPeriod(
                    start.toLocalDate(),
                    finish.toLocalDate(),
                    start.getZone(),
                    startWeek,
                    line.start,
                    line.finish,
                    line.dayOfWeek,
                    line.isOdd,
                    line.itemTitle,
                    line.typeOfLesson,
                    line.nameOfGroup,
                    line.nameOfTeacher,
                    line.audience,
                    line.address
            ));
        }
        return out;
    }

        /**
         * Функция ищет занятия для seeker в файле File.
         *
         * @param start  Дата и время начала составления расписания.
         *               Стоит отметить, что если указать день начала с воскресенья,
         *               то в понедельник будет номер недели равный двум.
         * @param finish Дата и время конца составления раписания.
         * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
         * @throws IOException        Во время работы с Excel file - файл стал недоступен.
         * @see #startAnInvestigation(ZonedDateTime, ZonedDateTime, int) 
         */
    @Override
    public LinkedList<CoupleInCalendar> startAnInvestigation(ZonedDateTime start, ZonedDateTime finish) throws DetectiveException, IOException {
        return startAnInvestigation(start, finish, 1);
    }

    /**
     * Функция расчитывает рекомендуемое время начала построения текущего расписания.
     *
     * @param now Момент времени, который считается настоящим.
     * @return Время начала занятий.
     * @see #getFinishTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getStartTime(ZonedDateTime now) {
        DetectiveDate.TwoZonedDateTime search;
        if (Month.JANUARY.getValue() <= now.getMonth().getValue()
                && now.getMonth().getValue() <= Month.JUNE.getValue()
        ) { // У нас загружано расписание для весны. Ищем начало.
            search = dateSettings.searchBeforeAfter(
                    ZonedDateTime.of(
                            LocalDate.of(now.getYear(), Month.FEBRUARY, 25),
                            LocalTime.NOON,
                            now.getZone()
                    ),
                    Duration.of(35, ChronoUnit.DAYS)
            );
        }
        else { // У нас загружано расписание для осени. Ищем начало.
            search = dateSettings.searchBeforeAfter(
                    ZonedDateTime.of(
                            LocalDate.of(now.getYear(), Month.SEPTEMBER, 25),
                            LocalTime.NOON,
                            now.getZone()
                    ),
                    Duration.of(35, ChronoUnit.DAYS)
            );
        }
        if(search.getLeft() == null)
            return guessStartTime(now); // Не нашёл? Давай гадать!

        return search.getLeft();
    }

    /**
     * Метод отвечает за то, чтобы догадаться, в какой день начнётся семестр.
     * @param now Любая дата требуемого семестра.
     * @return Предполагаемая дата начала семестра.
     */
    protected static ZonedDateTime guessStartTime(ZonedDateTime now) {
        if (Month.JANUARY.getValue() <= now.getMonth().getValue()
                && now.getMonth().getValue() <= Month.JUNE.getValue()
        ) { // У нас загружано расписание для весны
            // Установить на начало января
            ZonedDateTime current = ZonedDateTime.of(
                    LocalDate.of(now.getYear(), 1, 1),
                    LocalTime.of(0, 0, 0),
                    now.getZone()
            );
            // Прибавить 32 будни+суббота дней
            current = addBusinessDaysToDate(current, 32);
            switch (current.getDayOfWeek()) {
                //case MONDAY:
                //    break;
                case TUESDAY:
                    current = current.minus(1, ChronoUnit.DAYS);
                    break;
                case WEDNESDAY:
                    current = current.minus(2, ChronoUnit.DAYS);
                    break;
                //case THURSDAY:
                //    break;
                //case FRIDAY:
                //    break;
                case SATURDAY:
                    current = current.plus(2, ChronoUnit.DAYS);
                    break;
                case SUNDAY:
                    current = current.plus(1, ChronoUnit.DAYS);
                    break;
            }
            return current;
        }
        else { // У нас загружано расписание для осени.
            ZonedDateTime current = ZonedDateTime.of(
                    LocalDate.of(now.getYear(), 9, 1),
                    LocalTime.of(0, 0, 0),
                    now.getZone()
            );
            switch (current.getDayOfWeek()) {
                case SATURDAY:
                    current = current.plus(2, ChronoUnit.DAYS);
                    break;
                case SUNDAY:
                    current = current.plus(1, ChronoUnit.DAYS);
                    break;
            }
            return current;
        }
    }

    /**
     * Функция расчитывает рекомендуемое время конца построения текущего расписания.
     *
     * @param now Момент времени, который считается настоящим.
     * @return Время
     * @see #getStartTime(ZonedDateTime)
     */
    @Override
    public ZonedDateTime getFinishTime(ZonedDateTime now) {
        /*
         * Обращаемся к зачётным неделям.
         * Детектив зачётной недели говорит, когда у него начало построения.
         * Это время гарантированно 00:00:00.
         * Мы отнимаем секунду и получаем последнюю секунду, когда могут проводится
         * семестровые занятия.
         */
        ZonedDateTime current = DetectiveLastWeekS.static_getStartTime(this.dateSettings, now).minus(1, ChronoUnit.SECONDS);
        if(current.getDayOfWeek().equals(DayOfWeek.SUNDAY))
            current = current.minus(1, ChronoUnit.DAYS);
        return current;
    }


    /**
     * Функция ищет занятия для seeker в файле File.
     * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла
     * @throws IOException Во время работы с Excel file - файл стал недоступен.
     */
    protected LinkedList<CoupleInExcel> startViewer() throws DetectiveException, IOException {
        Point WeekPositionFirst = SeekInLeftUp("[Нн]еделя", 5*2, 3*2);
        LinkedList<Point> IgnoresCoupleTitle = new LinkedList<>();
        int[] Times = GetTimes(WeekPositionFirst); // Узнать время начала и конца пар.
        int CountCouples = Times.length / 2; // Узнать количество пар за день.
        Point basePos = SeekFirstCouple(); // Позиция первой записи "Предмет". Обычно на R3C6.
        // Ура! Мы нашли базовую позицию! Это basePos.
        LinkedList<CoupleInExcel> out = new LinkedList<>();
        for(
                int lastEC = 15, // Last entry count - Счётчик последней записи. Если долго не находим, то выходим из цикла.
                posEntryX = basePos.x; // Это позиция записи. Указывает столбец, где есть запись "Предмет".

                lastEC != 0;

                lastEC--, posEntryX++
                )
        {
            if(
                    "Предмет".equals(file.getCellData(posEntryX, basePos.y))
                            && file.getCellData(posEntryX, basePos.y - 1).length() > 0
            ) {
                lastEC = 15;
                //System.out.print(" R" + basePos.y + "C" + posEntryX);
                // Выставляем курсор на название первой пары дня.
                out.addAll(
                        GetCouplesFromAnchor(
                                posEntryX,
                                basePos.y,
                                Times,
                                IgnoresCoupleTitle,
                                getDefaultAddressesPoints()
                        )
                        /* Хорошо! Мы получили список занятий у группы.
                        Если это группа - то просто добавить,
                        если это преподаватель - то отфильтровать. */
                );
            }
        }
        //System.out.println();
        return out;
    }

    /**
     * Функция ищет список адресов, которые используются для негласного поля
     * {@code группа.адрес_по_умолчанию}. Это поле находится чуть правее от
     * названия группы и обозначается цветом.
     * @return Возвращает таблицу типа ключ-значение. Ключом является точка в
     * excel таблице, где находится ячейка с адресом. Это может быть
     * {@code R79C3}. Если обратиться к этому ключу, данная таблица вернёт
     * нормализованный адрес из этой ячейки.
     * Пример: "пр-т Вернадского, д.78".
     * @throws IOException Файл excel не доступен.
     */
    private HashMap<Point, String> getDefaultAddressesPoints() throws IOException {
        ArrayList<Point> points = seekInLeftUpAll("занятия в кампусе по адресу", 3*2, 82*2);
        HashMap<Point, String> out = new HashMap<>();
        for(Point p : points)
            out.put(p, getNormalAddressFromCell(p));
        return out;
    }

    /**
     * Отвечает на вопрос, является ли день свободным.
     * @param c Позиция названия первой пары дня. Столбец.
     * @param r Позиция названия первой пары дня. Строка.
     * @param CountCouples Количество пар в дне.
     * @param IgnoresCoupleTitle Список адресов заголовков, которые следует игнорировать. Идёт только добавление элементов в список.
     * @param file Файл, откуда надо считывать данные.
     * @return Истина, если данный день является днём самостоятельных работ, или же если в дне нет записей. Иначе: False.
     */
    private static boolean IsDayFree(int c, int r, int CountCouples, List<Point> IgnoresCoupleTitle, ExcelFileInterface file) throws IOException {
        for(int i = r; i < r + CountCouples*2; i++) {
            if(!file.getCellData(c, i).isEmpty())
            {
                if(file.getCellData(c, i).equals("День")) // Мы уже на второй строке встретим слово "День". Даю эту отпимизацию на то, что формат обозначения День самтостятельных занятий не изменится.
                {
                    for(; i < r + CountCouples*2; i++)
                        IgnoresCoupleTitle.add(new Point(c, i));
                    return true;
                }
                else return false; // Если встретили что-то другое, то это что-то не то! Вряд ли день самостоятельных занятий!
            }
            IgnoresCoupleTitle.add(new Point(c, i));
        } // Опа! Все пары - пусты!
        return true;
    }

    /**
     * Функция узнаёт, по какому адресу занимаются.
     * @param titleOfDay Позиция названия первой пары дня.
     * @param pointToGroupName Позиция, которая указывает на название группы.
     *                         Требуется для того, чтобы правее узнать адрес по-умолчанию.
     * @param addresses Местоположение адресов филиалов.
     * @param countCouples Количество пар в дне.
     * @param IgnoresCoupleTitle Список адресов заголовков, которые следует игнорировать.
     *                           Идёт только добавление элементов в список.
     * @return Адрес местоположения пары.
     */
    private String GetAddressOfDay(Point titleOfDay, Point pointToGroupName,
                                          HashMap<Point, String> addresses, int countCouples,
                                          Collection<Point> IgnoresCoupleTitle
    ) throws IOException {
        for (int y = titleOfDay.y; y < titleOfDay.y + countCouples * 2; y++)
            if (file.getCellData(titleOfDay.x, y).trim().equals("Занятия по адресу:")) {
                IgnoresCoupleTitle.add(new Point(titleOfDay.x, y));
                IgnoresCoupleTitle.add(new Point(titleOfDay.x, y + 1));
                return file.getCellData(titleOfDay.x, y + 1);
            }
        // Если никакой адресс не найден, надо искать defaultAddress группы. Чаще всего java попадает в эту ветку кода.
        pointToGroupName = new Point(pointToGroupName.x + 3, pointToGroupName.y);
        for (Point c : addresses.keySet())
            if (file.isBackgroundColorsEquals(pointToGroupName.x, pointToGroupName.y, c.x, c.y))
                return addresses.get(c);
        // Не получилось что-то найти... Влепим тогда хоть какой-нибудь. Сюда лучше не заходить java.
        //System.out.println("DetectiveSemester.java: Warning: address not found. file: " + file.toString());
        Iterator<String> it = addresses.values().iterator();
        if(it.hasNext())
            return it.next() + "?";
        else
            return "";
    }

    /**
     * Ищет в регионе 20x10 слово "Предмет" и возвращает его координаты.
     * Слово "Предмет" символизирует единицу расписания для группы.
     * Если слово "Предмет" не найдено, то фунуция вернёт {@code null}.
     * @return Координаты первого найденного слова "Предмет".
     */
    private Point SeekFirstCouple() throws DetectiveException, IOException {
        try {
            return SeekInLeftUp("Предмет", 6*2, 3*2);
        }
        catch (DetectiveException e){
            throw new DetectiveException(e.getMessage() + "\nНевозможно найти хотя бы один предмет в таблице Excel.", file);
        }
    }

    /**
     * Ищет в заданном регионе заданную фразу и возвращает координаты.
     * @param regex Регулярное выражение того того, что надо искать.
     * @param sizeX Количество колонок, в которых будет вестись поиск.
     * @param sizeY Количество строк, в которых будет вестись поиск.
     * @return Координаты первого найденного слова.
     * @throws DetectiveException Упс! Не нашёл!
     */
    private Point SeekInLeftUp(String regex, int sizeX, int sizeY) throws DetectiveException, IOException {
        if (sizeX < 0 || sizeY < 0)
            throw new IllegalArgumentException("sizeX and sizeY must be equals or more 0!\nX = " + sizeX + ", Y = " + sizeY + '.');
        if (regex != null) {
            Pattern p = Pattern.compile(regex);
            for (int y = 1; y <= sizeY; y++)
                for (int x = 1; x <= sizeX; x++)
                    if (p.matcher(file.getCellData(x, y)).find())
                        return new Point(x, y);
        }
        throw new DetectiveException("DetectiveSemester.java: Невозможно найти заданное слово Word. Word = '" + regex + "', sX = " + sizeX + ", sY = " + sizeY, file);
    }

    /**
     * Ищет в заданном регионе заданную фразу и возвращает лист координат на ячейки с подходящим текстом.
     * @param regex Регулярное выражение того того, что надо искать.
     * @param sizeX Количество колонок, в которых будет вестись поиск.
     * @param sizeY Количество строк, в которых будет вестись поиск.
     * @return Координаты первого найденного слова.
     */
    private ArrayList<Point> seekInLeftUpAll(String regex, int sizeX, int sizeY) throws IOException {
        ArrayList<Point> out = new ArrayList<>();
        if (sizeX < 0 || sizeY < 0)
            throw new IllegalArgumentException("sizeX and sizeY must be equals or more 0!\nX = " + sizeX + ", Y = " + sizeY + '.');
        if (regex != null) {
            Pattern p = Pattern.compile(regex);
            for (int y = 1; y <= sizeY; y++)
                for (int x = 1; x <= sizeX; x++)
                    if (p.matcher(file.getCellData(x, y)).find())
                        out.add(new Point(x, y));
        }
        return out;
    }

    /**
     * Извлекает адрес из ячейки. В ячейке может быть и другой текст, но забирается только адрес.
     * @param target Координаты ячейки, в которой находится адрес.
     *               Пример текста: "В-78 - занятия в кампусе по адресу пр-т Вернадского, д.78"
     * @return Текстовое представление адреса. Пример: "пр-т Вернадского, д.78"
     */
    private String getNormalAddressFromCell(Point target) throws IOException {
        String text = file.getCellData(target.x, target.y);
        int needIndex = text.lastIndexOf("адресу ") + 1;
        return text.substring(needIndex);
    }

    /**
     * Идёт считывание данных о предметах с определённого положения "Предмет".
     * @param column Столбец, где находится "Якорь" то есть ячейка с записью "Предмет".
     * @param row Строка, где находится "Якорь" то есть ячейка с записью "Предмет".
     * @param times Отсюда берётся расписание времени в формате началок - конец. На все пары.
     * @param ignoresCoupleTitle Лист занятий, который надо игнорировать.
     * @param addresses Точки, на которых распологается адреса филиалов.
     * @return Множество занятий у группы.
     */
    private LinkedList<CoupleInExcel> GetCouplesFromAnchor(int column, int row, int[] times, List<Point> ignoresCoupleTitle, HashMap<Point, String> addresses) throws IOException {
        LinkedList<CoupleInExcel> coupleOfWeek = new LinkedList<>();
        int countOfCouples = times.length / 2;
        Point pointToNameOfGroup = new Point(column, row - 1);
        String nameOfGroup = file.getCellData(pointToNameOfGroup.x, pointToNameOfGroup.y).trim();
        // TODO: С чего решил, что дней в неделе семь? И что сначала идёт понедельник?
        for(byte dayOfWeek = 1; dayOfWeek <= 7; dayOfWeek++)
        {
            //int c = column;
            int r = (row + 1) + (dayOfWeek - 1) * countOfCouples * 2;
            if (IsDayFree(column, r, countOfCouples, ignoresCoupleTitle, file))
                continue; // Если день свободен, то ничего не добавляем.
            coupleOfWeek.addAll(
                    GetCouplesFromDay
                            (column, r, nameOfGroup, DayOfWeek.of(dayOfWeek), ignoresCoupleTitle, times,
                                    GetAddressOfDay
                                            (
                                                    new Point(column, r),
                                                    pointToNameOfGroup,
                                                    addresses,
                                                    countOfCouples,
                                                    ignoresCoupleTitle
                                            )
                            )
            );
        }
        return coupleOfWeek;
    }

    /**
     * Идёт считывание данных в список о предметах с определённого положения: первая пара в дне.
     * @param column Столбец, который указывает на первую пару дня.
     * @param row Строка, которая указывает на перву пару дня.
     * @param nameOfGroup Имя группы.
     * @param dayOfWeek День недели. 1 - понедельник. 7 - воскресенье.
     * @param ignoresCoupleTitle Лист занятий, который надо игнорировать.
     * @param times Лист с началом и окончанием пары. {Начало1пары, конец1пары, начало2пары, конец2пары, ...}. Указывается в минутах.
     * @param address Адрес, где находятся пары
     * @return Множество занятий у группы в конкретный день.
     */
    public LinkedList<CoupleInExcel> GetCouplesFromDay(int column, int row, String nameOfGroup, DayOfWeek dayOfWeek, List<Point> ignoresCoupleTitle, int[] times, String address) throws IOException {
        LinkedList<CoupleInExcel> coupleOfDay = new LinkedList<>();
        int countOfCouples = times.length / 2;
        for(Point cursor = new Point(column, row); cursor.y < row + countOfCouples*2; cursor.y++) { // Считываем каждую строчку
            if (IsEqualsInList(ignoresCoupleTitle, cursor))
                continue; // Если такая запись под игнором, то игнорируем, ничего не делаем.
            String[] titles = file.getCellData(cursor.x, cursor.y).trim().split("(\r\n|)\n"); // Регулярное выражение. Делать новую строку либо от \r\n либо от \n. Универсально!
            titles = deleteEmptyElementsInArray(titles);
            String[] typeOfLessons = file.getCellData(cursor.x + 1, cursor.y).trim().split("(\r\n)|\n");
            String[] teachers = file.getCellData(cursor.x + 2, cursor.y).trim().split(("(\r\n)|\n"));
            String[] audiences = file.getCellData(cursor.x + 3, cursor.y).trim().split(("(\r\n)|\n"));
            for (int indexInLine = 0; indexInLine < titles.length; indexInLine++) {
                coupleOfDay.add(new CoupleInExcel(
                        titles[indexInLine],
                        indexInLine < typeOfLessons.length ? typeOfLessons[indexInLine] : typeOfLessons[0],
                        nameOfGroup,
                        indexInLine < teachers.length ? teachers[indexInLine] : teachers[0],
                        indexInLine < audiences.length ? audiences[indexInLine] : audiences[0],
                        address,
                        LocalTime.of(times[(cursor.y - row)/2*2] / 60, times[(cursor.y - row)/2*2] % 60),
                        LocalTime.of(times[(cursor.y - row)/2*2 + 1] / 60, times[(cursor.y - row)/2*2 + 1] % 60),
                        dayOfWeek,
                        (cursor.y - row) % 2 == 0
                ));
            }
        }
        return coupleOfDay;
    }

    private static String[] deleteEmptyElementsInArray(String [] input) {
        List<String> out = new ArrayList<>(Arrays.asList(input));
        out.removeIf((str) -> str == null || str.isEmpty() || " ".equals(str));
        return out.toArray(new String[0]);
    }


    /**
     * Отвечает на вопрос, содержится ли элемент в списке. Сравнивается с помощью equals.
     * @param list Список, в котором требуется проверить наличие этого объекта.
     * @param reference Объект, который является эталоном для поиска.
     * @param <T> Произвольный тип данных. Желаетльно, чтобы equals был определён.
     * @return {@code True}, если в {@code list} есть содержание элемента {@code reference}. Иначе - {@code False}.
     */
    public static <T> boolean IsEqualsInList(Iterable<T> list, T reference){
        for(T v : list)
            if(v.equals(reference)) return true;
        return false;
    }


    /**
     * Говорит, сколько максимально пар может поместиться в одном дне учёбы.
     * @param CR Координаты (столбец (x) и строка (y)), где находится фраза "Неделя".
     * @param file Файл, в котором надо подсчитать количество пар.
     * @return Максимальное количество пар в одном дне недели.
     * @throws DetectiveException Ошибка при поиске порядковых номеров пар.
     * @throws IOException Файл excel стал недоступным.
     */
    private static int GetCountCoupleInDay(Point CR, ExcelFileInterface file) throws DetectiveException, IOException {
        int OldNumber = Integer.MIN_VALUE; // Последнее число, которое было прочитано.
        int x = CR.x - 3; // Остаёмся на одном и том же столбце!
        for (int y = CR.y + 1; y < CR.y + 100; y++) { // Движемся вниз по строкам!
            if (file.getCellData(x, y).isEmpty()) continue;
            if(IsStringNumber(file.getCellData(x, y))) {
                int NewNumber = Integer.parseInt(file.getCellData(x, y), 10);
                if (NewNumber <= OldNumber) // Хоба! В резуьтате движения вних мы нашли тот момент, когда было 5... 6... и вот! 1!!!
                    return OldNumber; // Тут надо записывать по сути КОЛИЧЕСТВО_ПАР. Судя по расписанию, номер пары соответсвует количеству пройденых пар, и если мы нашли номер последней пары, то знаем и количество пар. Это NewNumber.
                OldNumber = NewNumber;
            }
            // Иначе продолжаем. continue;
        }
        return 0;
    }

    /**
     * Функция, которая извлекает из указанного участка Excel файла времена начала и конца пар в минутах.
     * @param CR Столбец и строка, где находится якорь. Якорём является самая первая запись "Неделя".
     * @return Возвращает список времён в формате минут: {начало пары, конец пары}.
     */
    public int[] GetTimes(Point CR) throws DetectiveException, IOException {
        int[] output = new int[2 * GetCountCoupleInDay(CR, file)];
        if(output.length == 0)
            throw new DetectiveException("Ошибка при поиске время начала и конца пар -> Пока программа спускалась вниз по строкам, считая, сколько пар в одном дне, она прошла окола 100 строк и сказала идити вы все, я столько не хочу обрабатывать.", file);
        // Ура, мы знаем количество. Это output.length. Теперь можно считывать времена.
        int indexArray = 0;
        for(int y = CR.y + 1; y < CR.y + 1 + output.length; y+=2)
            for(int x = CR.x - 2; x <= CR.x - 1; x++)
                // Заполняем масив временем.
                output[indexArray++] = GetMinutesFromTimeString(file.getCellData(x, y));
        return output;
    }

    /**
     * Конвектирует время в формате HH-MM или HH:MM в минуты.
     * @param inputT Входящее время в строковом представлении.
     * @return Возвращает время в минутах.
     */
    public int GetMinutesFromTimeString(String inputT) throws DetectiveException {
        if(inputT == null || inputT.isEmpty()) return 0;
        String[] HHMM = inputT.trim().split("-");
        if(HHMM.length == 1) HHMM = inputT.trim().split(":");
        if(HHMM.length == 2)
            return Integer.parseInt(HHMM[0]) * 60 + Integer.parseInt(HHMM[1]);
        throw new DetectiveException("Не получилось узнать время по записи: " + inputT, file);
    }

    /**
     * Возвращает ответ, может ли являться текст записью целого десятичного числа.
     * @param input Входной текст, который следует проверить.
     * @return {@code True}, если возможно корректно выполнить {@code Integer.parseInt(input)}, иначе - {@code False}.
     */
    public static boolean IsStringNumber(String input) {
        Pattern p = Pattern.compile("-?\\d+");
        Matcher m = p.matcher(input);
        return m.matches();
    }

    public class CoupleInExcel extends Couple {
        public final LocalTime start;
        public final LocalTime finish;
        public final DayOfWeek dayOfWeek;
        public final boolean isOdd;

        public CoupleInExcel(String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address, LocalTime start, LocalTime finish, DayOfWeek dayOfWeek, boolean isOdd) {
            super(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address);
            this.start = start;
            this.finish = finish;
            this.dayOfWeek = dayOfWeek;
            this.isOdd = isOdd;
        }
    }

    public static class SetterCouplesInCalendar {

        /**
         * Получает на входе данные про одну строку. Принимает решение, в какие дни будут пары. Не делает выборку данных.
         *
         * @param start              Дата и время начала сессии. Расписание будет составлено с этого дня и времени.
         * @param finish             Дата и время окончания сессии. Расписание будет составлено до этого дня и времени.
         * @param startZoneId        Часовой пояс, в котором начинается учебный план.
         * @param startWeek          С какого номера недели начать построение расписания?
         * @param timeStartOfCouple  Время начала пары.
         * @param timeFinishOfCouple Время окончания пары.
         * @param dayOfWeek          Рассматриваемый день недели. Использование: Напрмер, Calendar.MUNDAY.
         * @param isOdd              True, если это для нечётной недели. False, если эта строка для чётной недели.
         * @param itemTitle          Первая строка данных названия предмета. Сюда может входить и номера недель.
         * @param typeOfLesson       Первая строка типа занятия.
         * @param nameOfGroup        Рассматриваемая группа.
         * @param nameOfTeacher      Первая строка данных преподавателя.
         * @param audience           Первая строка аудитории.
         * @param address            Адрес корпуса.
         * @return Возвращает, в какие дни будут пары.
         */
        public static List<CoupleInCalendar> getCouplesByPeriod(LocalDate start, LocalDate finish, ZoneId startZoneId, int startWeek, LocalTime timeStartOfCouple, LocalTime timeFinishOfCouple, DayOfWeek dayOfWeek, boolean isOdd, String itemTitle, String typeOfLesson, String nameOfGroup, String nameOfTeacher, String audience, String address) {
            List<CoupleInCalendar> out;
            ZonedDateTime startT = ZonedDateTime.of(LocalDateTime.of(start, LocalTime.of(0, 0)), startZoneId);
            ZonedDateTime finishT = ZonedDateTime.of(LocalDateTime.of(finish, LocalTime.of(23, 50)), startZoneId);
            ZonedDateTime current = startT;
            long durationBetweenStartAndFinish = Duration.between(timeStartOfCouple, timeFinishOfCouple).toNanos();

            itemTitle = normalizeString(itemTitle);
            typeOfLesson = normalizeString(typeOfLesson);
            nameOfGroup = normalizeString(nameOfGroup);
            nameOfTeacher = normalizeString(nameOfTeacher);
            audience = normalizeString(audience);
            address = normalizeString(address);

            List<Integer> weeks = getWeeks(itemTitle, startWeek, (int) ((Duration.between(startT, finishT).toDays() / 7L) + 2L), isOdd);
            itemTitle = clearFromWeeks(itemTitle);
            out = new ArrayList<>(weeks.size() + 1);
            for (Integer numberOfWeek /*Номер недели*/ : weeks) {
                // Передвигаемся на неделю.
                current = startT.plus(numberOfWeek - startWeek, ChronoUnit.WEEKS);
                // Двигаемся к 00:00 dayOfWeek.
                current = current.minusNanos(current.getNano()).minusSeconds(current.getSecond()).minusMinutes(current.getMinute()).minusHours(current.getHour());
                int needAddDayOfWeek = dayOfWeek.getValue() - current.getDayOfWeek().getValue();
                current = current.plusDays(needAddDayOfWeek);
                current = current.plusNanos(timeStartOfCouple.getNano()).plusSeconds(timeStartOfCouple.getSecond()).plusMinutes(timeStartOfCouple.getMinute()).plusHours(timeStartOfCouple.getHour());
                if (
                        current.getLong(ChronoField.INSTANT_SECONDS) < startT.getLong(ChronoField.INSTANT_SECONDS) ||  // Использование LocalTime.MAX не безопасно: в дне может и не быть максимального локального времени. Использовано вместо этого прибавление одного дня и время 00:00.
                                current.getLong(ChronoField.INSTANT_SECONDS) >= finishT.getLong(ChronoField.INSTANT_SECONDS)) {
                    continue;
                }
                out.add(new CoupleInCalendar(
                        itemTitle,
                        typeOfLesson,
                        nameOfGroup,
                        nameOfTeacher,
                        audience,
                        address,
                        current,
                        current.plusNanos(durationBetweenStartAndFinish)
                ));
            }
            return out;
        }

        /**
         * Функция убирает из названия предмета заметки о неделях.
         * @param itemTitle Первая строка данных названия предмета. Сюда может входить и номера недель.
         * @return Строка без цифр, "н.", "недель".
         * @deprecated TODO: Данная функция ещё не реализована.
         */
        private static String clearFromWeeks(String itemTitle) {
            return itemTitle;
        }

        /**
         * Данная функция вернёт, на каких неделях пара есть.
         * @param itemTitle Заголовок названия предмета из таблицы расписания.
         * @param startWeek С какой недели начинается учёба?
         * @param limitWeek Максимальный доступный номер недели.
         * @param isOdd True, если это для нечётной недели. False, если эта строка для чётной недели.
         * @return Список необходимых недель.
         */
        private static List<Integer> getWeeks(String itemTitle, int startWeek, int limitWeek, boolean isOdd) {
            if (limitWeek < startWeek) return new ArrayList<>(1);
            Set<Integer> goodWeeks = new HashSet<>(limitWeek / 2 + 1); // Контейнер с хорошими неделями
            List<Integer> onlyWeeks = null;
            // Изменение входных параметров в зависимости от itemTitle.

            Integer startWeekFromString = getFromStringStartWeek(itemTitle); // Получаем, с какой недели идут пары.
            if (startWeekFromString != null) {
                if (startWeekFromString > startWeek)
                    startWeek = startWeekFromString;
            }
            Integer finishWeekFromString = getFromStringFinishWeek(itemTitle); // Получаем, с какой недели идут пары.
            if (finishWeekFromString != null) {
                if (finishWeekFromString < limitWeek)
                    limitWeek = finishWeekFromString;
            }

            List<Integer> exc = getAllExceptionWeeks(itemTitle);
            onlyWeeks = exc.size() == 0 ? getAllOnlyWeeks(itemTitle) : new ArrayList<>(0);
            if (startWeekFromString != null) {
                exc.remove(startWeekFromString);
                onlyWeeks.remove(startWeekFromString);
            }
            if (finishWeekFromString != null) {
                exc.remove(finishWeekFromString);
                onlyWeeks.remove(finishWeekFromString);
            }
            goodWeeks.addAll(onlyWeeks);
            int sw = startWeek, lw = limitWeek;
            goodWeeks.removeIf((week) -> week < sw || week > lw);
            if (goodWeeks.size() == 0) {
                for (int i = startWeek % 2 == 0 ? isOdd ? startWeek + 1 : startWeek : isOdd ? startWeek : startWeek + 1;
                     i < limitWeek + 1; i += 2) {
                    if (!exc.contains(i)) // Если это не исключение
                        goodWeeks.add(i); // Пусть все недели - хорошие.
                }
            }
            ArrayList<Integer> out = new ArrayList<>(goodWeeks);
            out.sort(Integer::compareTo);
            return out;
        }

        /**
         * Функция ищет, до какой недели идут пары. Ищет "До %d", где %d - целое число.
         * @param itemTitle Заголовок названия предмета из таблицы расписания.
         * @return Возвращает %d. В случае, если не найдено - возвращается {@code null}.
         */
        private static Integer getFromStringFinishWeek(String itemTitle) {
            Pattern p = Pattern.compile("(^| )[Дд]о ?+\\d+");
            Matcher m = p.matcher(itemTitle);
            if(m.find())
                return Integer.parseInt(m.group().substring(3));
            else
                return null;
        }

        /**
         * Функция ищет, с какой недели идут пары. Ищет "C %d", где %d - целое число.
         * @param itemTitle Заголовок названия предмета из таблицы расписания.
         * @return Возвращает %d. В случае, если не найдено - возвращается {@code null}.
         */
        private static Integer getFromStringStartWeek(String itemTitle) {
            Pattern p = Pattern.compile("(^| )[СсCc] ?+\\d+");
            Matcher m = p.matcher(itemTitle);
            if(m.find())
                return Integer.parseInt(m.group().substring(2).trim());
            else return null;
        }

        /**
         * Функция ищет, в каких неделях есть исключения. Ищет "Кроме %d" или "кр. %d", где %d - целое число.
         * @param itemTitle Заголовок названия предмета из таблицы расписания.
         * @return Возвращает %d. В случае, если не найдено - возвращается пустой список.
         */
        public static List<Integer> getAllExceptionWeeks(String itemTitle) {
            // [Кк]р(\.|(оме))?.((((\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\.?|н( |\.|$)))|((нед([а-яА-Я]+)?\.?|н( |\.|$)) ?((\d(, ?| и | )?)+)))

            Pattern p = Pattern.compile("[Кк]р(\\.|(оме))?.((((\\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\\.?|н( |\\.|$)))|((нед([а-яА-Я]+)?\\.?|н( |\\.|$)) ?((\\d(, ?| и | )?)+)))");
            Matcher m = p.matcher(itemTitle);
            if(m.find())
                return getAllIntsFromString(m.group());
            else
                return new LinkedList<>();
        }

        /**
         * Функция пытается найти такие выражения, как "%d, %d и %d н." и их вариации. Возвращает список недель.
         * Pattern: {@code (((\d(, ?| и | )?)+) ?(нед|н( |\.|$)))|((нед|н( |\.|$)) ?((\d(, ?| и | )?)+))}
         * @param itemTitle Заголовок названия предмета из таблицы расписания.
         * @return Возвращает %d. В случае, если не найдено - возвращается пустой список.
         */
        public static List<Integer> getAllOnlyWeeks(String itemTitle) {
            // ((\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\.?|н( |\.|$))
            // (((\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\.?|н( |\.|$)))|((нед([а-яА-Я]+)?\.?|н( |\.)) ?((\d(, ?| и | )?)+))
            Pattern p = Pattern.compile("(((\\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\\.?|н( |\\.|$)))|((нед([а-яА-Я]+)?\\.?|н( |\\.)) ?((\\d(, ?| и | )?)+))");
            Matcher m = p.matcher(itemTitle);
            if(m.find())
                return getAllIntsFromString(m.group());
            else
                return new LinkedList<>();
        }

        /**
         * Находит отрицательные и положительные целые числа в строке и выдаёт их список.
         * @param input Входная строка, где следует проводить поиск целых чисел.
         * @return Перечень найденных целых чисел. Если не найдено - пустой список.
         */
        private static List<Integer> getAllIntsFromString(String input) {
            LinkedList<Integer> numbers = new LinkedList<Integer>();
            Pattern p = Pattern.compile("-?\\d+");
            Matcher m = p.matcher(input);
            while (m.find()) {
                numbers.add(Integer.parseInt(m.group()));
            }
            return numbers;
        }

        /**
         * Удаляет символы 0..0x1F, заменяя на пробелы. Удаляет пробелы слева и справа.
         * @param input Входная строка, из которой необходимо удалить управляющие символы.
         * @return Новый экземпляр строки без управляющих символов и левых-правых пробелов.
         */
        private static String normalizeString(String input) {
            if(input == null)
                return null;
            StringBuilder in = new StringBuilder(input);
            for(int i = in.length() - 1; i >= 0; i--) {
                if(in.charAt(i) < 32) {
                    in.replace(i, i, " ");
                }
            }
            return in.toString().trim();
        }
    }
}
