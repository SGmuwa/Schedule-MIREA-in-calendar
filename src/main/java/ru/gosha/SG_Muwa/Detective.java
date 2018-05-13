
/*
План детектива:
0. Узнаю список времён: Начала и окончания пар.

Цикл:
1. Получаю список за день.
2. Анализирую, является ли день днём самостоятельных занятий.
3. Узнаю, по какому адресу занимаются в этот день.
4. Добавляю в исключения строки с адресами.
5. Разбиваю по двум строкам в GetCouplesByPeriod. Исключающие строки отправляются как "".

 */

import java.awt.*;

// Класс, который занимается поиском расписания.
public class Detective {

    /**
     * Функция ищет занятия для seeker в файлах files.
     * @param seeker критерий поиска.
     * @param files список файлов, в которых требуется искать пары занятий.
     */
    public static void StartAnInvestigations(Seeker seeker, Iterable<ExcelFileInterface> files) throws DetectiveException {
        for(ExcelFileInterface f : files) StartAnInvestigation(seeker, f);
    }


    /**
     * Функция ищет занятия для seeker в файле File.
     * @param seeker критерий поиска.
     * @param file файл, в котором требуется искать пары занятий.
     */
    public static void StartAnInvestigation(Seeker seeker, ExcelFileInterface file) throws DetectiveException {
        Point WeekPosition = SeekEverythingInLeftUp("Неделя", file);
        int[] Times = GetTimes(WeekPosition, file);
        int CountCouples = GetCountCoupleInDay(WeekPosition, file);
        Point basePos;
        try {
            basePos = SeekFirstCouple(file); // Позиция первой записи "Предмет".
            // Обычно на R3C6.
        } catch (DetectiveException e) {
            System.out.println(e.getMessage());
            System.err.println(e.getMessage());
            return; // Нет пар в этом листе.
        }
        // Ура! Мы нашли базовую позицию!
        for(
                int lastEC = 10, // Last entry count - Счётчик последней записи.
                posEntryX = basePos.x; // Это позиция записи. Указывает столбец, где есть запись "Предмет".

                lastEC != 0;

                lastEC--, posEntryX--
                )
        {
            if(file.getCellData(posEntryX, basePos.y).equals("Предмет"))
                for(int DayOfTheWeek = 1; DayOfTheWeek <= 7; DayOfTheWeek++) {
                    // Выставляем курсор на название первой пары дня.
                    Point cursor = new Point(posEntryX, basePos.y + 1 + DayOfTheWeek * CountCoples);
                    if(IsDayFree(cursor, CountCouples, file)) continue; // Если день свободен, то ничего не добавляем.
                    for (Couple couple : GetCoupleFromAnchor(posEntryX, basePos.y, file)) {
                        // Хорошо! Мы получили список занятий у группы. Если это группа - то просто добавить, если это преподаватель - то отфильтровать.

                    }
                }
        }
    }

    /**
     * Отвечает на вопрос, является ли день свободным.
     * @param titleOfDay Позиция названия первой пары дня.
     * @param CountCouples Количество пар в дне.
     * @param file Файл, откуда надо считывать данные.
     * @return Истина, если данный день является днём самостоятельных работ, или же если в дне нет записей. Иначе: False.
     */
    private static boolean IsDayFree(Point titleOfDay, int CountCouples, ExcelFileInterface file) {
        for(int i = titleOfDay.y; i < titleOfDay.y + CountCouples*2; i++) {
            if(file.getCellData(titleOfDay.x, i).isEmpty() == false)
            {
                if(file.getCellData(titleOfDay.x, i).equals("День")) // Мы уже на второй строке встретим слово "День". Даю эту отпимизацию на то, что формат обозначения День самтостятельных занятий не изменится.
                    return true;
                else return false; // Если встретили что-то другое, то это что-то не то! Вряд ли день самостоятельных занятий!
            }
        } // Опа! Все пары - пусты!
        return true;
    }

    /**
     * Функция узнаёт, по какому адресу занимаются.
     * @param titleOfDay Позиция названия первой пары дня.
     * @param CountCouples Количество пар в дне.
     * @param DefaultAddress Адрес по-умолчанию за день.
     * @param file Файл, откуда надо считывать данные.
     * @return Адрес местоположения пары.
     */
    private static String GetAddressOfDay(Point titleOfDay, int CountCouples, String DefaultAddress, ExcelFileInterface file) {
        for(int y = titleOfDay.y; y < titleOfDay.y + CountCouples*2; y++)
            if(file.getCellData(titleOfDay.x, y).equals("Занятия по адресу:"))
    }

    /**
     * Ищет в регионе 20x10 слово "Предмет" и возвращает его координаты.
     * Слово "Предмет" символизирует единицу расписания для группы.
     * Если слово "Предмет" не найдено, то фунуция вернёт {@code null}.
     * @param file Файл, в котором следует искать.
     * @return Координаты первого найденного слова "Предмет".
     */
    private static Point SeekFirstCouple(ExcelFileInterface file) throws DetectiveException {
        try {
            return SeekEverythingInLeftUp("Предмет", file);
        }
        finally {
            throw new DetectiveException("Невозможно найти хотя бы один предмет в таблице Excel.");
        }
    }

    /**
     * Ищет в регионе 20x10 заданную фразу и возвращает координаты.
     * @param Word Слово, которое следует искать.
     * @param file Файл, в котором требуется искать.
     * @return Координаты первого найденного слова "Предмет".
     * @throws DetectiveException Упс! Не нашёл!
     */
    private static Point SeekEverythingInLeftUp(String Word, ExcelFileInterface file) throws DetectiveException {
        for(int y = 1; y <= 10; y++)
            for(int x = 1; x <= 20; x++)
                if(file.getCellData(x, y).equals(Word)) return new Point(x, y);
        throw new DetectiveException("Невозможно найти заданное слово Word. Word = " + Word);
    }

    /**
     * Идёт считывание данных о предметах с определённого положения "Предмет".
     * @param Column Столбец, где находится "Якорь".
     * @param Row Строка, где находится "Якорь".
     * @param file Файл, откуда надо производить чтение.
     * @return Множество занятий у группы.
     */
    private static Iterable<Couple> GetCoupleFromAnchor(int Column, int Row, ExcelFileInterface file){


    }

    /**
     * Говорит, сколько пар в одном дне.
     * @param CR Координаты фразы "Неделя".
     * @param file Файл, в котором надо подсчитать количество пар.
     * @return Количество пар в одном дне недели.
     * @throws DetectiveException
     */
    private static int GetCountCoupleInDay(Point CR, ExcelFileInterface file) throws DetectiveException {
        int OldNumber = Integer.MIN_VALUE; // Последнее число, которое было прочитано.
        int output = 0;
        int x = CR.x - 3; // Остаёмся на одном и том же столбце!
        for (int y = CR.y + 1; y < CR.y + 100; y++) { // Движемся вниз по строкам!
            if (IsStringNumber(file.getCellData(x, y)) == false) {// Если это не число
                throw new DetectiveException("Ошибка при поиске порядковых номеров пар. На ячейке R3C5 должно быть написано \"Неделя\" а в ячейке R4C2 требуется порядковый номер пары.");
            } else { // Если мы движемся правильно в сторону прочтения порядковых номеров (нам попадаются только числа), то
                if (file.getCellData(x, y).isEmpty()) continue;
                output++;
                int NewNumber = Integer.parseInt(file.getCellData(x, y), 10);
                if (NewNumber <= OldNumber) // Хоба! В резуьтате движения вних мы нашли тот момент, когда было 5... 6... и вот! 1!!!
                    return output; // Тут надо записывать по сути КОЛИЧЕСТВО_ПАР * 2. Судя по расписанию, номер пары соответсвует количеству пройденых пар, и если мы нашли номер последней пары, то знаем и количество пар. Это NewNumber.
                else continue; // Если не нашли, то ничего страшного! Продолжаем!
            }
        }
        return 0;
    }

    /**
     * Функция, которая извлекает из указанного участка Excel файла времена начала и конца пар в минутах.
     * @param CR Столбец и строка, где находится якорь. Якорём является самая первая запись "Неделя".
     * @param file Excel файл.
     * @return Возвращает список времён в формате минут: {начало пары, конец пары}.
     */
    private static int[] GetTimes(Point CR, ExcelFileInterface file) throws  DetectiveException {
        int[] output = new int[2 * GetCountCoupleInDay(CR, file)];
        if(output.length == 0)
            throw new DetectiveException("Ошибка при поиске время начала и конца пар -> Пока программа спускалась вниз по строкам, считая, сколько пар в одном дне, она прошла окола 100 строк и сказала идити вы все, я столько не хочу обрабатывать.");
        // Ура, мы знаем количество. Это output.length. Теперь можно считывать времена.
        int indexArray = 0;
        for(int y = CR.y + 1; y <= CR.y + 1 + output.length*2; y+=2)
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
    private static int GetMinutesFromTimeString(String inputT) {
        if(inputT == null || inputT.isEmpty()) return 0;
        String[] HHMM = inputT.trim().split("-");
        if(HHMM.length == 1) HHMM = inputT.trim().split(":");
        return Integer.parseInt(HHMM[0]) * 60 + Integer.parseInt(HHMM[1]);
    }

    /**
     * Возвращает ответ, может ли являться текст записью десятичного числа.
     * @param input Входной текст, который следует проверить.
     * @return {@code True}, если в input записано число или же строка пуста, иначе - {@code False}.
     */
    private static boolean IsStringNumber(String input) {
        char[] buffer = input.trim().toCharArray();
        for(char a : buffer)
            if(a > '9' || a < '0') return false;
        return true;
    }
}
