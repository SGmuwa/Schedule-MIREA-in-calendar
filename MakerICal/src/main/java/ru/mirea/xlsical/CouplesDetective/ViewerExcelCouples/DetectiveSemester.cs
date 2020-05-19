/*
    Schedule MIREA in calendar.
    Copyright (C) 2020  Mikhail Pavlovich Sidorenko (motherlode.muwa@gmail.com)

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.xl;

namespace ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples
{

    /**
     * Данный класс отвечает за получение календарных пар из Excel расписания.
     * Умеет читать только семестровое расписание.
     * @author <a href="https://github.com/SGmuwa/">[SG]Muwa</a>.
     */
    public class DetectiveSemester : Detective
    {

        public DetectiveSemester(ExcelFileInterface file, DetectiveDate dateSettings) : base(file, dateSettings) { }

        /// <summary>
        /// Функция ищет занятия для seeker в файле File.
        /// </summary>
        /// <param name="start">Дата и время начала составления расписания.</param>
        /// <param name="finish">Дата и время конца составления расписания.</param>
        /// <param name="startWeek">Номер недели в день <code>start</code>.</param>
        /// <returns></returns>
        /// <exception cref="DetectiveException">Появилась проблема, связанная с обработкой Excel файла.</exception>
        /// <exception cref="System.IO.IOException">Во время работы с Excel file — файл стал недоступен.</exception>
        public IEnumerable<CoupleInCalendar> StartAnInvestigation(ZonedDateTime start, ZonedDateTime finish, int startWeek)
        {
            LinkedList<CoupleInExcel> couplesInExcel = startViewer();
            LinkedList<CoupleInCalendar> @out = new LinkedList<CoupleInCalendar>();
            foreach (CoupleInExcel line in couplesInExcel)
            {
                @out.AddLastRange(SetterCouplesInCalendar.getCouplesByPeriod(
                    start,
                    finish,
                    startWeek,
                    line.start,
                    line.finish,
                    #warning Волшебная константа!
                    DateTimeZoneProviders.Tzdb["Europe/Moscow"],
                    line.dayOfWeek,
                    line.isOdd,
                    line.ItemTitle,
                    line.TypeOfLesson,
                    line.NameOfGroup,
                    line.NameOfTeacher,
                    line.Audience,
                    line.Address
            ));
            }
            return @out;
        }

        /**
         * Функция ищет занятия для seeker в файле File.
         *
         * @param start  Дата и время начала составления расписания.
         *               Стоит отметить, что если указать день начала с воскресенья,
         *               то в понедельник будет номер недели равный двум.
         * @param finish Дата и время конца составления расписания.
         * @throws DetectiveException Появилась проблема, связанная с обработкой Excel файла.
         * @throws IOException        Во время работы с Excel file - файл стал недоступен.
         * @see #startAnInvestigation(ZonedDateTime, ZonedDateTime, int) 
         */
        public override IEnumerable<CoupleInCalendar> StartAnInvestigation(ZonedDateTime start, ZonedDateTime finish)
            => StartAnInvestigation(start, finish, 1);

        /// <summary>
        /// Функция расчитывает рекомендуемое время начала построения текущего расписания.
        /// </summary>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Время начала занятий.</returns>
        /// <seealso cref="GetFinishTime(ZonedDateTime)"/>
        public override ZonedDateTime GetStartTime(ZonedDateTime now)
        {
            (ZonedDateTime? start, ZonedDateTime? finish) search;
            if (IsoMonth.January <= (IsoMonth)now.Month
                    && (IsoMonth)now.Month <= IsoMonth.June
            )
            { // У нас загружено расписание для весны. Ищем начало.
                search = dateSettings.searchBeforeAfter(
                    now.Zone.AtLeniently(new LocalDateTime(now.Year, (int)IsoMonth.February, 25, 12, 00)),
                    Duration.FromDays(35)
                );
            }
            else
            { // У нас загружено расписание для осени. Ищем начало.
                search = dateSettings.searchBeforeAfter(
                    now.Zone.AtLeniently(new LocalDateTime(now.Year, (int)IsoMonth.September, 25, 12, 00)),
                    Duration.FromDays(35)
                );
            }
            return search.start.HasValue ? search.start.Value : guessStartTime(now);
        }

        /// <summary>
        /// Метод отвечает за то, чтобы догадаться, в какой день начнётся семестр.
        /// </summary>
        /// <param name="now">Любая дата требуемого семестра.</param>
        /// <returns>Предполагаемая дата начала семестра.</returns>
        protected static ZonedDateTime guessStartTime(ZonedDateTime now)
        {
            if (IsoMonth.January <= (IsoMonth)now.Month
                    && (IsoMonth)now.Month <= IsoMonth.June
            )
            { // У нас загружено расписание для весны
              // Установить на начало января
                ZonedDateTime current = now.Zone.AtStartOfDay(new LocalDate(now.Year, (int)IsoMonth.January, 1));
                // Прибавить 32 будни+суббота дней
                current = AddBusinessDaysToDate(current, 32);
                switch (current.DayOfWeek)
                {
                    //case MONDAY:
                    //    break;
                    case IsoDayOfWeek.Tuesday:
                        current = current.PlusDays(-1);
                        break;
                    case IsoDayOfWeek.Wednesday:
                        current = current.PlusDays(-2);
                        break;
                    //case THURSDAY:
                    //    break;
                    //case FRIDAY:
                    //    break;
                    case IsoDayOfWeek.Saturday:
                        current = current.PlusDays(2);
                        break;
                    case IsoDayOfWeek.Sunday:
                        current = current.PlusDays(1);
                        break;
                }
                return current;
            }
            else
            { // У нас загружено расписание для осени.
                ZonedDateTime current = now.Zone.AtStartOfDay(new LocalDate(now.Year, (int)IsoMonth.September, 1));
                switch (current.DayOfWeek)
                {
                    case IsoDayOfWeek.Saturday:
                        current = current.PlusDays(2);
                        break;
                    case IsoDayOfWeek.Sunday:
                        current = current.PlusDays(1);
                        break;
                }
                return current;
            }
        }

        /// <summary>
        /// Функция расчитывает рекомендуемое время конца построения текущего расписания.
        /// </summary>
        /// <param name="now">Момент времени, который считается настоящим.</param>
        /// <returns>Время</returns>
        /// <seealso cref="GetStartTime(ZonedDateTime)"/>
        public override ZonedDateTime GetFinishTime(ZonedDateTime now)
        {
            /*
             * Обращаемся к зачётным неделям.
             * Детектив зачётной недели говорит, когда у него начало построения.
             * Это время гарантированно 00:00:00.
             * Мы отнимаем секунду и получаем последнюю секунду, когда могут проводится
             * семестровые занятия.
             */
            ZonedDateTime current = DetectiveLastWeekS.GetStartTime(this.dateSettings, now).PlusMinutes(-1);
            if (current.DayOfWeek == IsoDayOfWeek.Sunday)
                current = current.PlusDays(-1);
            return current;
        }

        /// <summary>
        /// Функция ищет занятия в файле File.
        /// </summary>
        /// <returns>Список Excel занятий.</returns>
        /// <exception cref="DetectiveException">Появилась проблема, связанная с обработкой Excel файла</exception>
        /// <exception cref="System.IO.IOException">Во время работы с Excel файлом он стал недоступен.</exception>
        protected LinkedList<CoupleInExcel> startViewer()
        {
            Point WeekPositionFirst = SeekInLeftUp("[Нн]еделя", 5 * 2, 3 * 2);
            List<Point> IgnoresCoupleTitle = new List<Point>();
            int[] Times = GetTimes(WeekPositionFirst); // Узнать время начала и конца пар.
            int CountCouples = Times.Length / 2; // Узнать количество пар за день.
            Point basePos = SeekFirstCouple(); // Позиция первой записи "Предмет". Обычно на R3C6.
                                               // Ура! Мы нашли базовую позицию! Это basePos.
            LinkedList<CoupleInExcel> @out = new LinkedList<CoupleInExcel>();
            for (
                    int lastEC = 15, // Last entry count - Счётчик последней записи. Если долго не находим, то выходим из цикла.
                    posEntryX = basePos.X; // Это позиция записи. Указывает столбец, где есть запись "Предмет".

                    lastEC != 0;

                    lastEC--, posEntryX++
                    )
            {
                if (
                        "Предмет".Equals(file.GetCellData(posEntryX, basePos.Y))
                                && file.GetCellData(posEntryX, basePos.Y - 1).Any()
                )
                {
                    lastEC = 15;
                    //System.out.print(" R" + basePos.y + "C" + posEntryX);
                    // Выставляем курсор на название первой пары дня.
                    @out.AddLastRange(
                            GetCouplesFromAnchor(
                                    posEntryX,
                                    basePos.Y,
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
            return @out;
        }

        /// <summary>
        /// Функция ищет список адресов, которые используются для негласного поля
        /// <c>группа.адрес_по_умолчанию</c>. Это поле находится чуть правее от
        /// названия группы и обозначается цветом.
        /// </summary>
        /// <returns>Возвращает таблицу типа ключ-значение. Ключом является точка в
        /// excel таблице, где находится ячейка с адресом. Это может быть
        /// <c>R79C3</c>. Если обратиться к этому ключу, данная таблица вернёт
        /// нормализованный адрес из этой ячейки.
        /// Пример: "пр-т Вернадского, д.78".</returns>
        /// <exception>Файл excel не доступен.</exception>
        private Dictionary<Point, string> getDefaultAddressesPoints()
        {
            List<Point> points = seekInLeftUpAll("занятия в кампусе по адресу", 3 * 2, 82 * 2);
            Dictionary<Point, string> @out = new Dictionary<Point, string>();
            foreach (Point p in points)
                @out[p] = getNormalAddressFromCell(p);
            return @out;
        }

        /// <summary>
        /// Отвечает на вопрос, является ли день свободным.
        /// </summary>
        /// <param name="c">Позиция названия первой пары дня. Столбец.</param>
        /// <param name="r">Позиция названия первой пары дня. Строка.</param>
        /// <param name="CountCouples">Количество пар в дне.</param>
        /// <param name="IgnoresCoupleTitle">Список адресов заголовков, которые следует игнорировать. Идёт только добавление элементов в список.</param>
        /// <param name="file">Файл, откуда надо считывать данные.</param>
        /// <returns><c>true</c>, если данный день является днём самостоятельных работ, или же если в дне нет записей. Иначе — <c>false</c>.</returns>
        private static bool IsDayFree(int c, int r, int CountCouples, List<Point> IgnoresCoupleTitle, ExcelFileInterface file)
        {
            for (int i = r; i < r + CountCouples * 2; i++)
            {
                if (file.GetCellData(c, i).Any())
                {
                    if (file.GetCellData(c, i).Equals("День")) // Мы уже на второй строке встретим слово "День". Даю эту отпимизацию на то, что формат обозначения День самтостятельных занятий не изменится.
                    {
                        for (; i < r + CountCouples * 2; i++)
                            IgnoresCoupleTitle.Add(new Point(c, i));
                        return true;
                    }
                    else return false; // Если встретили что-то другое, то это что-то не то! Вряд ли день самостоятельных занятий!
                }
                IgnoresCoupleTitle.Add(new Point(c, i));
            } // Опа! Все пары - пусты!
            return true;
        }

        /// <summary>
        /// Функция узнаёт, по какому адресу занимаются.
        /// </summary>
        /// <param name="titleOfDay">Позиция названия первой пары дня.</param>
        /// <param name="pointToGroupName">Позиция, которая указывает на название группы.
        /// Требуется для того, чтобы правее узнать адрес по-умолчанию.</param>
        /// <param name="addresses">Местоположение адресов филиалов.</param>
        /// <param name="countCouples">Количество пар в дне.</param>
        /// <param name="IgnoresCoupleTitle"> Список адресов заголовков, которые следует игнорировать.
        /// Идёт только добавление элементов в список.</param>
        /// <returns>Адрес местоположения пары.</returns>
        private string GetAddressOfDay(Point titleOfDay, Point pointToGroupName,
            Dictionary<Point, string> addresses, int countCouples,
            ICollection<Point> IgnoresCoupleTitle
        )
        {
            for (int y = titleOfDay.Y; y < titleOfDay.Y + countCouples * 2; y++)
                if (file.GetCellData(titleOfDay.X, y).Trim().Equals("Занятия по адресу:"))
                {
                    IgnoresCoupleTitle.Add(new Point(titleOfDay.X, y));
                    IgnoresCoupleTitle.Add(new Point(titleOfDay.X, y + 1));
                    return file.GetCellData(titleOfDay.X, y + 1);
                }
            // Если никакой адрес не найден, надо искать defaultAddress группы. Чаще всего java попадает в эту ветку кода.
            pointToGroupName = new Point(pointToGroupName.X + 3, pointToGroupName.Y);
            foreach (Point c in addresses.Keys)
                if (file.IsBackgroundColorsEquals(pointToGroupName.X, pointToGroupName.Y, c.X, c.Y))
                    return addresses[c];
            // Не получилось что-то найти... Влепим тогда хоть какой-нибудь. Сюда лучше не заходить java.
            return addresses.Values.FirstOrDefault();
        }

        /// <summary>
        /// Ищет в регионе 20x10 слово "Предмет" и возвращает его координаты.
        /// Слово "Предмет" символизирует единицу расписания для группы.
        /// Если слово "Предмет" не найдено, то функция вернёт <c>null</c>.
        /// </summary>
        /// <returns>Координаты первого найденного слова "Предмет".</returns>
        private Point SeekFirstCouple()
        {
            try
            {
                return SeekInLeftUp("Предмет", 6 * 2, 3 * 2);
            }
            catch (DetectiveException e)
            {
                throw new DetectiveException(e.Message + "\nНевозможно найти хотя бы один предмет в таблице Excel.", file);
            }
        }

        /// <summary>
        /// Ищет в заданном регионе заданную фразу и возвращает координаты.
        /// </summary>
        /// <param name="regex">Регулярное выражение того того, что надо искать.</param>
        /// <param name="sizeX">Количество колонок, в которых будет вестись поиск.</param>
        /// <param name="sizeY">Количество строк, в которых будет вестись поиск.</param>
        /// <returns>Координаты первого найденного слова.</returns>
        /// <exception cref="DetectiveException">Нет соответствующей ячейки.</exception>
        private Point SeekInLeftUp(string regex, int sizeX, int sizeY)
        {
            if (sizeX < 0 || sizeY < 0)
                throw new System.ArgumentException("sizeX and sizeY must be equals or more 0!\nX = " + sizeX + ", Y = " + sizeY + '.');
            if (regex != null)
            {
                Regex p = new Regex(regex);
                for (int y = 1; y <= sizeY; y++)
                    for (int x = 1; x <= sizeX; x++)
                        if (p.IsMatch(file.GetCellData(x, y)))
                            return new Point(x, y);
            }
            throw new DetectiveException("DetectiveSemester.java: Невозможно найти заданное слово Word. Word = '" + regex + "', sX = " + sizeX + ", sY = " + sizeY, file);
        }

        /// <summary>
        /// Ищет в заданном регионе заданную фразу и возвращает лист координат на ячейки с подходящим текстом.
        /// </summary>
        /// <param name="regex">Регулярное выражение того того, что надо искать.</param>
        /// <param name="sizeX">Количество колонок, в которых будет вестись поиск.</param>
        /// <param name="sizeY">Количество строк, в которых будет вестись поиск.</param>
        /// <returns>Координаты первого найденного слова.</returns>
        private List<Point> seekInLeftUpAll(string regex, int sizeX, int sizeY)
        {
            List<Point> @out = new List<Point>();
            if (sizeX < 0 || sizeY < 0)
                throw new System.ArgumentException("sizeX and sizeY must be equals or more 0!\nX = " + sizeX + ", Y = " + sizeY + '.');
            if (regex != null)
            {
                Regex p = new Regex(regex);
                for (int y = 1; y <= sizeY; y++)
                    for (int x = 1; x <= sizeX; x++)
                        if (p.IsMatch(file.GetCellData(x, y)))
                            @out.Add(new Point(x, y));
            }
            return @out;
        }

        /// <summary>
        /// Извлекает адрес из ячейки. В ячейке может быть и другой текст, но забирается только адрес.
        /// <para>Пример текста: "В-78 - занятия в кампусе по адресу пр-т Вернадского, д.78"</para>
        /// </summary>
        /// <param name="target">Координаты ячейки, в которой находится адрес.</param>
        /// <returns>
        /// Текстовое представление адреса. Пример: "пр-т Вернадского, д.78"
        /// </returns>
        private string getNormalAddressFromCell(Point target)
        {
            string text = file.GetCellData(target.X, target.Y);
            int needIndex = text.LastIndexOf("адресу ") + 1;
            return text.Substring(needIndex);
        }

        /// <summary>
        /// Идёт считывание данных о предметах с определённого положения "Предмет".
        /// </summary>
        /// <param name="column">Столбец, где находится "Якорь" то есть ячейка с записью "Предмет".</param>
        /// <param name="row">Строка, где находится "Якорь" то есть ячейка с записью "Предмет".</param>
        /// <param name="times">Отсюда берётся расписание времени в формате началок - конец. На все пары.</param>
        /// <param name="ignoresCoupleTitle">Лист занятий, который надо игнорировать.</param>
        /// <param name="addresses">Точки, на которых располагается адреса филиалов.</param>
        /// <returns>Множество занятий у группы.</returns>
        private LinkedList<CoupleInExcel> GetCouplesFromAnchor(
            int column, int row, int[] times,
            List<Point> ignoresCoupleTitle, Dictionary<Point, string> addresses)
        {
            LinkedList<CoupleInExcel> coupleOfWeek = new LinkedList<CoupleInExcel>();
            int countOfCouples = times.Length / 2;
            Point pointToNameOfGroup = new Point(column, row - 1);
            string nameOfGroup = file.GetCellData(pointToNameOfGroup.X, pointToNameOfGroup.Y).Trim();
#warning С чего решил, что дней в неделе семь? И что сначала идёт понедельник?
            for (IsoDayOfWeek dayOfWeek = IsoDayOfWeek.Monday; dayOfWeek <= IsoDayOfWeek.Sunday; dayOfWeek++)
            {
                //int c = column;
                int r = (row + 1) + (dayOfWeek - IsoDayOfWeek.Monday) * countOfCouples * 2;
                if (IsDayFree(column, r, countOfCouples, ignoresCoupleTitle, file))
                    continue; // Если день свободен, то ничего не добавляем.
                coupleOfWeek.AddLastRange(
                        GetCouplesFromDay
                                (column, r, nameOfGroup, dayOfWeek, ignoresCoupleTitle, times,
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

        /// <summary>
        /// Идёт считывание данных в список о предметах с определённого положения: первая пара в дне.
        /// </summary>
        /// <param name="column">Столбец, который указывает на первую пару дня.</param>
        /// <param name="row">Строка, которая указывает на первую пару дня.</param>
        /// <param name="nameOfGroup">Имя группы.</param>
        /// <param name="dayOfWeek">День недели.</param>
        /// <param name="ignoresCoupleTitle">Лист занятий, который надо игнорировать.</param>
        /// <param name="times">Лист с началом и окончанием пары. {Начало1пары, конец1пары, начало2пары, конец2пары, ...}. Указывается в минутах.</param>
        /// <param name="address">Адрес, где находятся пары</param>
        /// <returns>Множество занятий у группы в конкретный день.</returns>
        public LinkedList<CoupleInExcel> GetCouplesFromDay(
            int column, int row, string nameOfGroup,
            IsoDayOfWeek dayOfWeek, List<Point> ignoresCoupleTitle, int[] times,
            string address)
        {
            LinkedList<CoupleInExcel> coupleOfDay = new LinkedList<CoupleInExcel>();
            int countOfCouples = times.Length / 2;
            for (Point cursor = new Point(column, row); cursor.Y < row + countOfCouples * 2; cursor.Y++)
            { // Считываем каждую строчку
                if (IsEqualsInList(ignoresCoupleTitle, cursor))
                    continue; // Такую запись надо проигнорировать.
                string[] titles = file.GetCellData(cursor.X, cursor.Y).Trim().Split("(\r\n|)\n"); // Регулярное выражение. Делать новую строку либо от \r\n либо от \n. Универсально!
                titles = deleteEmptyElementsInArray(titles);
                string[] typeOfLessons = file.GetCellData(cursor.X + 1, cursor.Y).Trim().Split("(\r\n)|\n");
                string[] teachers = file.GetCellData(cursor.X + 2, cursor.Y).Trim().Split(("(\r\n)|\n"));
                string[] audiences = file.GetCellData(cursor.X + 3, cursor.Y).Trim().Split(("(\r\n)|\n"));
                for (int indexInLine = 0; indexInLine < titles.Length; indexInLine++)
                {
                    coupleOfDay.AddLast(new CoupleInExcel(
                            titles[indexInLine],
                            indexInLine < typeOfLessons.Length ? typeOfLessons[indexInLine] : typeOfLessons[0],
                            nameOfGroup,
                            indexInLine < teachers.Length ? teachers[indexInLine] : teachers[0],
                            indexInLine < audiences.Length ? audiences[indexInLine] : audiences[0],
                            address,
                            new LocalTime(times[(cursor.Y - row) / 2 * 2] / 60, times[(cursor.Y - row) / 2 * 2] % 60),
                            new LocalTime(times[(cursor.Y - row) / 2 * 2 + 1] / 60, times[(cursor.Y - row) / 2 * 2 + 1] % 60),
                            dayOfWeek,
                            (cursor.Y - row) % 2 == 0
                    ));
                }
            }
            return coupleOfDay;
        }

        private static string[] deleteEmptyElementsInArray(string[] input)
        {
            List<string> @out = new List<string>(input);
            @out.RemoveAll((str) => str == null || !str.Any() || " ".Equals(str));
            return @out.ToArray();
        }

        /// <summary>
        /// Отвечает на вопрос, содержится ли элемент в списке. Сравнивается с помощью equals.
        /// </summary>
        /// <param name="list">Список, в котором требуется проверить наличие этого объекта.</param>
        /// <param name="reference">Объект, который является эталоном для поиска.</param>
        /// <typeparam name="T">Произвольный тип данных. Желательно, чтобы equals был определён.</typeparam>
        /// <returns><c>true</c>, если в <paramref name="list"/> есть содержание элемента <paramref name="reference"/>. Иначе — <c>false</c>.</returns>
        public static bool IsEqualsInList<T>(IEnumerable<T> list, T reference)
        {
            foreach (T v in list)
                if (v.Equals(reference)) return true;
            return false;
        }

        /// <summary>
        /// Говорит, сколько максимально пар может поместиться в одном дне учёбы.
        /// </summary>
        /// <param name="CR">Координаты (столбец (x) и строка (y)), где находится фраза "Неделя".</param>
        /// <param name="file">Файл, в котором надо подсчитать количество пар.</param>
        /// <returns>Максимальное количество пар в одном дне недели.</returns>
        /// <exception cref="DetectiveException">Ошибка при поиске порядковых номеров пар.</exception>
        /// <exception cref="System.IO.IOException">Файл excel стал недоступным.</exception>
        private static int GetCountCoupleInDay(Point CR, ExcelFileInterface file)
        {
            int OldNumber = int.MinValue; // Последнее число, которое было прочитано.
            int x = CR.X - 3; // Остаёмся на одном и том же столбце!
            for (int y = CR.Y + 1; y < CR.Y + 100; y++)
            { // Движемся вниз по строкам!
                if (!file.GetCellData(x, y).Any()) continue;
                if (IsStringNumber(file.GetCellData(x, y)))
                {
                    int NewNumber = int.Parse(file.GetCellData(x, y));
                    if (NewNumber <= OldNumber) // Хоба! В результате движения вниз мы нашли тот момент, когда было 5... 6... и вот! 1!!!
                        return OldNumber; // Тут надо записывать по сути КОЛИЧЕСТВО_ПАР. Судя по расписанию, номер пары соответствует количеству пройденных пар, и если мы нашли номер последней пары, то знаем и количество пар. Это NewNumber.
                    OldNumber = NewNumber;
                }
                // Иначе продолжаем. continue;
            }
            return 0;
        }

        /// <summary>
        /// Функция, которая извлекает из указанного участка Excel файла времена начала и конца пар в минутах.
        /// </summary>
        /// <param name="CR">Столбец и строка, где находится якорь. Якорём является самая первая запись "Неделя".</param>
        /// <returns>Возвращает список времён в формате минут: {начало пары, конец пары}.</returns>
        public int[] GetTimes(Point CR)
        {
            int[] output = new int[2 * GetCountCoupleInDay(CR, file)];
            if (output.Length == 0)
                throw new DetectiveException("Ошибка при поиске время начала и конца пар -> Пока программа спускалась вниз по строкам, считая, сколько пар в одном дне, она прошла около 100 строк и сказала идти вы все, я столько не хочу обрабатывать.", file);
            // Ура, мы знаем количество. Это output.length. Теперь можно считывать времена.
            int indexArray = 0;
            for (int y = CR.Y + 1; y < CR.Y + 1 + output.Length; y += 2)
                for (int x = CR.X - 2; x <= CR.X - 1; x++)
                    // Заполняем массив временем.
                    output[indexArray++] = GetMinutesFromTimeString(file.GetCellData(x, y));
            return output;
        }

        /// <summary>
        /// Конвектирует время в формате HH-MM или HH:MM в минуты.
        /// </summary>
        /// <param name="inputT">Входящее время в строковом представлении.</param>
        /// <returns>Возвращает время в минутах.</returns>
        public int GetMinutesFromTimeString(string inputT)
        {
            if (inputT == null || !inputT.Any()) return 0;
            string[] HHMM = inputT.Trim().Split("-");
            if (HHMM.Length == 1) HHMM = inputT.Trim().Split(":");
            if (HHMM.Length == 2)
                return int.Parse(HHMM[0]) * 60 + int.Parse(HHMM[1]);
            throw new DetectiveException("Не получилось узнать время по записи: " + inputT, file);
        }

        private static readonly Regex reg10 = new Regex(@"^-?\d+$");

        /// <summary>
        /// Возвращает ответ, может ли являться текст записью целого десятичного числа.
        /// </summary>
        /// <param name="input">Входной текст, который следует проверить.</param>
        /// <returns><c>true</c>, если возможно корректно выполнить <see cref="int.Parse(string)"/>, иначе — <c>false</c>.</returns>
        public static bool IsStringNumber(string input) => reg10.IsMatch(input);

        public class CoupleInExcel : Couple
        {
            public readonly LocalTime start;
            public readonly LocalTime finish;
            public readonly IsoDayOfWeek dayOfWeek;
            public readonly bool isOdd;

            public CoupleInExcel(string itemTitle, string typeOfLesson, string nameOfGroup, string nameOfTeacher, string audience, string address, LocalTime start, LocalTime finish, IsoDayOfWeek dayOfWeek, bool isOdd)
            : base(itemTitle, typeOfLesson, nameOfGroup, nameOfTeacher, audience, address)
            {
                this.start = start;
                this.finish = finish;
                this.dayOfWeek = dayOfWeek;
                this.isOdd = isOdd;
            }
        }

        public static class SetterCouplesInCalendar
        {
            /// <summary>
            /// Получает на входе данные про одну строку. Принимает решение, в какие дни будут пары. Не делает выборку данных.
            /// </summary>
            /// <param name="start">Дата и время начала семестра. Расписание будет составлено с этого дня и времени.</param>
            /// <param name="finish">Дата и время окончания семестра. Расписание будет составлено до этого дня и времени.</param>
            /// <param name="startWeek">С какого номера недели начать построение расписания?</param>
            /// <param name="timeStartOfCouple">Время начала пары.</param>
            /// <param name="timeFinishOfCouple">Время окончания пары.</param>
            /// <param name="timezoneCouple">Часовой пояс пар</param>
            /// <param name="dayOfWeek">Рассматриваемый день недели. Использование: Напрмер, Calendar.MUNDAY.</param>
            /// <param name="isOdd"><c>true</c>, если это для нечётной недели. <c>false</c>, если эта строка для чётной недели.</param>
            /// <param name="itemTitle">Первая строка данных названия предмета. Сюда может входить и номера недель.</param>
            /// <param name="typeOfLesson">Первая строка типа занятия.</param>
            /// <param name="nameOfGroup">Рассматриваемая группа.</param>
            /// <param name="nameOfTeacher">Первая строка данных преподавателя.</param>
            /// <param name="audience">Первая строка аудитории.</param>
            /// <param name="address">Адрес корпуса.</param>
            /// <returns>Возвращает, в какие дни будут пары.</returns>
            public static List<CoupleInCalendar> getCouplesByPeriod(
                ZonedDateTime start, ZonedDateTime finish, int startWeek,
                LocalTime timeStartOfCouple, LocalTime timeFinishOfCouple, DateTimeZone timezoneCouple,
                IsoDayOfWeek dayOfWeek, bool isOdd, string itemTitle,
                string typeOfLesson, string nameOfGroup, string nameOfTeacher,
                string audience, string address)
            {
                List<CoupleInCalendar> @out;
                start = start.WithZone(timezoneCouple);
                finish = finish.WithZone(timezoneCouple);
                ZonedDateTime current = start;
                Duration durationBetweenStartAndFinish = (timeFinishOfCouple - timeStartOfCouple).ToDuration();

                itemTitle = NormalizeString(itemTitle);
                typeOfLesson = NormalizeString(typeOfLesson);
                nameOfGroup = NormalizeString(nameOfGroup);
                nameOfTeacher = NormalizeString(nameOfTeacher);
                audience = NormalizeString(audience);
                address = NormalizeString(address);

                List<int> weeks = GetWeeks(itemTitle, startWeek, (int)(((finish - start).Days / 7L) + 2L), isOdd);
                itemTitle = clearFromWeeks(itemTitle);
                @out = new List<CoupleInCalendar>(weeks.Count + 1);
                foreach (int numberOfWeek /*Номер недели*/ in weeks)
                {
                    // Передвигаемся на неделю.
                    current = start.PlusDays((numberOfWeek - startWeek) * 7);
                    current = current.Zone.AtStartOfDay(current.Date);
                    int needAddDayOfWeek = (int)dayOfWeek - (int)current.DayOfWeek;
                    current = current.PlusDays(needAddDayOfWeek);
                    current = current.Zone.AtStrictly(current.Date.At(timeStartOfCouple));
                    if (
                        ZonedDateTime.Comparer.Instant.Compare(current, start) >= 0
                        && ZonedDateTime.Comparer.Instant.Compare(current, finish) < 0
                    )
                        @out.Add(new CoupleInCalendar(
                            itemTitle,
                            typeOfLesson,
                            nameOfGroup,
                            nameOfTeacher,
                            audience,
                            address,
                            current,
                            current.Plus(durationBetweenStartAndFinish)
                        ));
                }
                return @out;
            }

            /// <summary>
            /// Функция убирает из названия предмета заметки о неделях.
            /// </summary>
            /// <param name="itemTitle">Первая строка данных названия предмета. Сюда может входить и номера недель.</param>
            /// <returns>Строка без цифр, "н.", "недель".</returns>
            private static string clearFromWeeks(string itemTitle)
            {
#warning Данная функция ещё не реализована.
                return itemTitle;
            }

            /// <summary>
            /// Данная функция вернёт, на каких неделях пара есть.
            /// </summary>
            /// <param name="itemTitle">Заголовок названия предмета из таблицы расписания.</param>
            /// <param name="startWeek">С какой недели начинается учёба?</param>
            /// <param name="limitWeek">Максимальный доступный номер недели.</param>
            /// <param name="isOdd"><c>true</c>, если это для нечётной недели. <c>false</c>, если эта строка для чётной недели.</param>
            /// <returns></returns>
            private static List<int> GetWeeks(string itemTitle, int startWeek, int limitWeek, bool isOdd)
            {
                if (limitWeek < startWeek) return new List<int>(1);
                HashSet<int> goodWeeks = new HashSet<int>(limitWeek / 2 + 1); // Контейнер с хорошими неделями
                // Изменение входных параметров в зависимости от itemTitle.

                int? startWeekFromString = GetFromStringStartWeek(itemTitle); // Получаем, с какой недели идут пары.
                if (startWeekFromString.HasValue)
                {
                    if (startWeekFromString > startWeek)
                        startWeek = startWeekFromString.Value;
                }
                int? finishWeekFromString = GetFromStringFinishWeek(itemTitle); // Получаем, с какой недели идут пары.
                if (finishWeekFromString.HasValue)
                {
                    if (finishWeekFromString < limitWeek)
                        limitWeek = finishWeekFromString.Value;
                }

                List<int> exc = new List<int>(GetAllExceptionWeeks(itemTitle));
                List<int> onlyWeeks = new List<int>(GetAllOnlyWeeks(itemTitle));
                if (startWeekFromString.HasValue)
                {
                    exc.Remove(startWeekFromString.Value);
                    onlyWeeks.Remove(startWeekFromString.Value);
                }
                if (finishWeekFromString.HasValue)
                {
                    exc.Remove(finishWeekFromString.Value);
                    onlyWeeks.Remove(finishWeekFromString.Value);
                }
                goodWeeks.AddRange(onlyWeeks);
                int sw = startWeek, lw = limitWeek;
                goodWeeks.RemoveWhere((week) => week < sw || week > lw);
                if (goodWeeks.Count == 0)
                {
                    for (int i = startWeek % 2 == 0 ? isOdd ? startWeek + 1 : startWeek : isOdd ? startWeek : startWeek + 1;
                         i < limitWeek + 1; i += 2)
                    {
                        if (!exc.Contains(i)) // Если это не исключение
                            goodWeeks.Add(i); // Пусть все недели - хорошие.
                    }
                }
                List<int> @out = new List<int>(goodWeeks);
                @out.Sort();
                return @out;
            }

            private static readonly Regex regFinishWeek = new Regex(@"(^| )[Дд]о ?+\d+");

            /// <summary>
            /// Функция ищет, до какой недели идут пары. Ищет "До %d", где %d - целое число.
            /// </summary>
            /// <param name="itemTitle">Заголовок названия предмета из таблицы расписания.</param>
            /// <returns>Возвращает %d. В случае, если не найдено — возвращается <c>null</c>.</returns>
            private static int? GetFromStringFinishWeek(string itemTitle)
            {
                Match m = regFinishWeek.Match(itemTitle);
                if (m.Success)
                    return int.Parse(m.Value.Substring(3));
                else
                    return null;
            }

            private static readonly Regex regStartWeek = new Regex(@"(^| )[СсCc] ?+\d+");

            /// <summary>
            /// Функция ищет, с какой недели идут пары. Ищет "C %d", где %d - целое число.
            /// </summary>
            /// <param name="itemTitle">Заголовок названия предмета из таблицы расписания.</param>
            /// <returns>Возвращает %d. В случае, если не найдено - возвращается <c>null</c>.</returns>
            private static int? GetFromStringStartWeek(string itemTitle)
            {
                Match m = regStartWeek.Match(itemTitle);
                if (m.Success)
                    return int.Parse(m.Value.Substring(2).Trim());
                else
                    return null;
            }

            private static readonly Regex regWeeksExcept = new Regex(@"[Кк]р(\.|(оме))?.((((\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\.?|н( |\.|$)))|((нед([а-яА-Я]+)?\.?|н( |\.|$)) ?((\d(, ?| и | )?)+)))");

            /// <summary>
            /// Функция ищет, в каких неделях есть исключения. Ищет "Кроме %d" или "кр. %d", где %d - целое число.
            /// </summary>
            /// <param name="itemTitle">Заголовок названия предмета из таблицы расписания.</param>
            /// <returns>Возвращает %d. В случае, если не найдено — возвращается пустой список.</returns>
            public static IEnumerable<int> GetAllExceptionWeeks(string itemTitle)
            {
                Match m = regWeeksExcept.Match(itemTitle);
                if (m.Success)
                    return GetAllIntsFromString(m.Value);
                else
                    return Enumerable.Empty<int>();
            }

            private static readonly Regex regWeeks = new Regex(@"(((\d(, ?| и | )?)+) ?(нед([а-яА-Я]+)?\.?|н( |\.|$)))|((нед([а-яА-Я]+)?\.?|н( |\.)) ?((\d(, ?| и | )?)+))");

            /// <summary>
            /// Функция пытается найти такие выражения, как "%d, %d и %d н." и их вариации. Возвращает список недель.
            /// </summary>
            /// <param name="itemTitle">Заголовок названия предмета из таблицы расписания.</param>
            /// <returns>Возвращает все найденные недели (%d). В случае, если не найдено — возвращается пустой список.</returns>
            public static IEnumerable<int> GetAllOnlyWeeks(string itemTitle)
            {
                Match m = regWeeks.Match(itemTitle);
                if (m.Success)
                    return GetAllIntsFromString(m.Value);
                else
                    return Enumerable.Empty<int>();
            }

            private static readonly Regex reg10any = new Regex(@"-?\d+");

            /// <summary>
            /// Находит отрицательные и положительные целые числа в строке и выдаёт их список.
            /// </summary>
            /// <param name="input">Входная строка, где следует проводить поиск целых чисел.</param>
            /// <returns>Перечень найденных целых чисел. Если не найдено — пустой список.</returns>
            private static IEnumerable<int> GetAllIntsFromString(string input)
            => from s in reg10any.Matches(input) select int.Parse(s.Value);

            /// <summary>
            /// Удаляет символы 0..0x1F, заменяя на пробелы. Удаляет пробелы слева и справа.
            /// </summary>
            /// <param name="input">Входная строка, из которой необходимо удалить управляющие символы.</param>
            /// <returns>Новый экземпляр строки без управляющих символов и левых-правых пробелов.</returns>
            private static string NormalizeString(string input)
            {
                if (input == null)
                    return null;
                StringBuilder @in = new StringBuilder(input);
                Parallel.For(0, @in.Length, i =>
                {
                    if (@in[i] < 0x20)
                        @in[i] = ' ';
                });
                return @in.ToString().Trim();
            }
        }
    }
}
