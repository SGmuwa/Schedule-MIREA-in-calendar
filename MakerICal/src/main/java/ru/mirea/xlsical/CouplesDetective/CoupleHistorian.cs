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
using System.IO;
using ru.mirea.xlsical.interpreter;
using NodaTime;
using ru.mirea.xlsical.CouplesDetective.ViewerExcelCouples;
using System.Linq;
using System.Text.RegularExpressions;

namespace ru.mirea.xlsical.CouplesDetective
{
    /// <summary>
    /// Класс реализует защиту от записи прошедших пар.
    /// Также предсказывает, какие пары будут в будущем.
    /// </summary>
    public class CoupleHistorian
    {
        private readonly ExternalDataUpdater edUpdater;
        private LinkedList<CoupleInCalendar> cache;
        private DetectiveDate settingDates;
        public readonly FileInfo pathToCache;

        public CoupleHistorian(ZonedDateTime now, ExternalDataUpdater edUpdater, DetectiveDate detectiveDate, PercentReady pr, FileInfo pathToCache)
        {
            PercentReady PR_loadCache = new PercentReady(pr, 0.1f);
            PercentReady PR_updateCache = new PercentReady(pr, 0.9f);
            this.pathToCache = pathToCache;
            this.edUpdater = edUpdater;
            this.edUpdater.setNeedUpdate(this.updateCache);
            this.settingDates = detectiveDate;
            if (this.settingDates == null)
                this.settingDates = new DetectiveDate();
            PR_loadCache.Ready = 0f;
            if (this.pathToCache.Exists)
            {
                try
                {
                    loadCache();
                }
                catch (IOException)
                {
                    cache = null;
                }
            }
            PR_loadCache.Ready = 1f;
            this.now = now;
            updateCache(PR_updateCache);
        }

        public CoupleHistorian(ZonedDateTime now, PercentReady pr = null, FileInfo pathToCache = null)
        {
            if (pathToCache == null)
                pathToCache = new FileInfo("ArrayListOfCouplesInCalendar.dat");
            if (pr == null)
                pr = new PercentReady();
            PercentReady PR_constructor = new PercentReady(pr, 0.000025f);
            PercentReady PR_external = new PercentReady(pr, 0.0025f - 0.000025f);
            this.pathToCache = pathToCache;
            this.now = now;

            //
            // Создание ссылки на внешние данные.
            //

            this.settingDates = new DetectiveDate();
            PR_constructor.Ready = 0.2f;
            this.edUpdater = new ExternalDataUpdater(PR_external);
            this.edUpdater.setNeedUpdate(this.updateCache);
            PR_constructor.Ready = 0.4f;


            if (this.pathToCache != null && this.pathToCache.Exists)
                try
                {
                    loadCache();
                }
                catch (IOException)
                {
                    cache = null;
                }
            PR_constructor.Ready = 0.75f;
            // Всегда нужно обновить кэш после простоя.
            updateCache(new PercentReady(pr, 1f - 0.0025f));
        }

        protected LinkedList<CoupleInCalendar> getCache()
        {
            return cache;
        }

        private ZonedDateTime now;

        public void setNow(ZonedDateTime now)
        {
            this.now = now;
        }

        /// <summary>
        /// Скачивает с сайта МИРЭА расписание.
        /// Сортирует пары по дате-времени.
        /// Анализирует будущие пары.
        /// Сохраняет на диск.
        /// </summary>
        /// <param name="pr">Доступ к управлению процентом загрузки.</param>
        private void updateCache(PercentReady pr)
        {
            PercentReady[]
            cycles = new PercentReady[] {
                new PercentReady(pr, 12f/16f),
                new PercentReady(pr, 1f/16f),
                new PercentReady(pr, 1f/16f) };
            PercentReady sort = new PercentReady(pr, 2f / 16f);
            LinkedList<CoupleInCalendar> outCache = new LinkedList<CoupleInCalendar>(); // Итоговый кэш
            LinkedList<CoupleInCalendar> newCache = new LinkedList<CoupleInCalendar>(); // То, что получили из МИРЭА
            if (this.cache == null)
                this.cache = new LinkedList<CoupleInCalendar>();
            ZonedDateTime now = this.now == null ? new ZonedDateTime(LocalDateTime.FromDateTime(System.DateTime.UtcNow), DateTimeZone.Utc, Offset.Zero) : this.now;

            {
                EnumeratorExcels it = edUpdater.OpenTablesFromExternal();
                float size = it.Count * 3;
                int i = 0;
                while (it.MoveNext())
                {
                    Detective detective = Detective.ChooseDetective(it.Current, settingDates);
                    try
                    {
                        newCache.AddLastRange(detective.StartAnInvestigation(
                                detective.GetStartTime(now),
                                detective.GetFinishTime(now)
                        ));
                    }
                    catch (IOException) { }
                    catch (DetectiveException) { }
                    try
                    {
                        it.Current.Dispose();
                    }
                    catch (IOException)
                    {
                        System.Console.WriteLine("can't close file: " + it.Current.ToString());
                    }
                    if (i + 1 < size)
                        cycles[0].Ready = ++i / size;
                }
                cycles[0].Ready = 1.0f;
            }
            // Всё, что позже этой метки - можно менять. Всё, что раньше - нельзя.
            ZonedDateTime deadLine = now.PlusDays(-4);
            // Добавим то, что было раньше.
            {
                float size = cache.Count;
                int i = 0;
                foreach (CoupleInCalendar couple in cache)
                {
                    if (ZonedDateTime.Comparer.Instant.Compare(couple.DateAndTimeOfCouple, deadLine) < 0)
                    { // Добавим то, что было до обновления.
                        outCache.AddLast(couple);
                    }
                    else break;
                    cycles[1].Ready = ++i / size;
                }
                cycles[1].Ready = 1.0f;
            }
            // Добавим то, что нового.

            {
                float size = newCache.Count;
                int i = 0;
                foreach (CoupleInCalendar couple in newCache)
                {
                    if (ZonedDateTime.Comparer.Instant.Compare(deadLine, couple.DateAndTimeOfCouple) <= 0)
                    { // Добавим новые данные
                        outCache.AddLast(couple);
                    }
                    cycles[2].Ready = ++i / size;
                }
                cycles[2].Ready = 1.0f;
            }
            sortByDateTime(outCache, sort);
            mergeCouples(outCache);
            cache = outCache;
            if (pathToCache != null)
                saveCache();
        }

        protected void loadCache()
        {
            LinkedList<CoupleInCalendar> outCache = static_loadCache(pathToCache);
            sortByDateTime(outCache, new PercentReady());
            mergeCouples(outCache);
            cache = outCache;
        }

        protected static LinkedList<CoupleInCalendar> static_loadCache(FileInfo pathToCache)
        => SaverLoaderClass.ReadFromBinaryFile<LinkedList<CoupleInCalendar>>(pathToCache);

        protected void saveCache() => SaverLoaderClass.WriteToBinaryFile(pathToCache, cache);

        /// <summary>
        /// Получение календарного расписания по заданным критериям.
        /// </summary>
        /// <param name="queryCriteria">Критерии запроса, по которым будет происходить выборка данных.</param>
        /// <param name="percentReady">Указатель, куда помещать % готовности.</param>
        /// <returns>Новый список с календарными парами определённой группы или определённого
        /// преподавателя. Начиная с даты начала и заканчивая датой конца.</returns>
        public List<CoupleInCalendar> getCouples(Seeker queryCriteria, PercentReady percentReady)
        {
            percentReady.Ready = 0.0f;
            List<CoupleInCalendar> @out = new List<CoupleInCalendar>(cache.Count);
            Regex p;
            try
            {
                p = new Regex(queryCriteria.NameOfSeeker);
            }
            catch (System.ArgumentException)
            {
                p = null;
            }
            int ready = 0;
            int size = cache.Count;
            foreach (CoupleInCalendar couple in cache)
            {
                /*

                -------------(A)-----------(T)-----------(B)-----------------(t)>

                A = queryCriteria.start
                B = queryCriteria.finish
                T = ...t... = couple
                out.add if A <= T && T <= B
                out.add if A раньше или равен T && T раньше или равен B

                if (time1.compareTo(time2) < 0) { // Если time1 раньше time2.

                 */
                if (
                        // queryCriteria.dateStart раньше или равно couple.dateAndTimeFinishOfCouple
                        ZonedDateTime.Comparer.Instant.Compare(queryCriteria.DateStart, couple.DateAndTimeFinishOfCouple) <= 0
                                // couple.dateAndTimeOfCouple раньше или равно queryCriteria.dateFinish
                                && ZonedDateTime.Comparer.Instant.Compare(couple.DateAndTimeOfCouple, queryCriteria.DateFinish) <= 0
                )
                {
                    if (p != null && (couple.NameOfGroup != null && p.IsMatch(couple.NameOfGroup)
                        || couple.NameOfTeacher != null && p.IsMatch(couple.NameOfTeacher)))
                    {
                        // if by regex.
                        @out.Add(couple);
                    }
                    else if (queryCriteria.NameOfSeeker.Equals(couple.NameOfGroup) || queryCriteria.NameOfSeeker.Equals(couple.NameOfTeacher))
                        // if by equals.
                        @out.Add(couple);
                }
                ready++;
                percentReady.Ready = (float)ready / (float)size;
            }
            percentReady.Ready = 1.0f;
            return @out;
        }

        /**
         * Объединяет повторяющиеся пары в {@link #sortByDateTime(List, PercentReady) отсортированном} массиве.
         * Повторяющимися парами являются такая пара учебных занятий, где
         * совпадает аудитория, время начала и конца пары, заголовок и тип пары.
         * @param listNeedMerge Список пар, в которых надо найти эквивалентный пары
         *                      и объединить между собой. Данный лист обязан быть отсортирован.
         * @see CoupleInCalendar#equals(Object) Подробнее об сравнении пар между собой.
         */
        private static void mergeCouples(LinkedList<CoupleInCalendar> listNeedMerge)
        {
            IEnumerator<CoupleInCalendar> listIterator = listNeedMerge.GetEnumerator();
            ICollection<CoupleInCalendar> toRemove = new HashSet<CoupleInCalendar>();
            if (!listIterator.MoveNext())
                return;
            ISet<CoupleInCalendar> previous = new HashSet<CoupleInCalendar>();
            previous.Add(listIterator.Current);
            while (listIterator.MoveNext())
            {
                int flag = 0; // 0 = nothing. 1 = clear. 2 = add.
                foreach (CoupleInCalendar p in previous)
                {
                    if (listIterator.Current.DateAndTimeOfCouple.Equals(p.DateAndTimeOfCouple)
                            && listIterator.Current.Address.Equals(p.Address)
                            && listIterator.Current.Audience.Equals(p.Audience)
                            && listIterator.Current.ItemTitle.Equals(p.ItemTitle)
                            && listIterator.Current.TypeOfLesson.Equals(p.TypeOfLesson)
                            && listIterator.Current.DateAndTimeFinishOfCouple.Equals(p.DateAndTimeFinishOfCouple))
                    {
                        // Если это одна и та же пара, но преподаватель или группа другие...
                        if (!listIterator.Current.NameOfGroup.Equals(p.NameOfGroup))
                        {
                            p.NameOfGroup += ", " + listIterator.Current.NameOfGroup;
                        }
                        if (!listIterator.Current.NameOfTeacher.Equals(p.NameOfTeacher))
                        {
                            p.NameOfTeacher += ", " + listIterator.Current.NameOfTeacher;
                        }
                        toRemove.Add(listIterator.Current);
                        flag = 0;
                    }
                    else if (!listIterator.Current.DateAndTimeOfCouple.Equals(p.DateAndTimeOfCouple))
                    {
                        flag = 1;
                    }
                    else
                    {
                        flag = 2;
                    }
                }
                switch (flag)
                {
                    case 0:
                        break;
                    case 1:
                        previous.Clear();
                        previous.Add(listIterator.Current);
                        break;
                    case 2:
                        previous.Add(listIterator.Current);
                        break;
                }
            }
            listNeedMerge.RemoveAll(e => toRemove.Contains(e));
            /*
            List<CoupleInCalendar> needDelete = new ArrayList<>();
            ArrayList<CoupleInCalendar> couplesInDay = new ArrayList<>();
            for(CoupleInCalendar couple : listNeedMerge) {*/
            /*
            Если в массиве ничего нет, то добавить пару в список пар дня.
            Если текущая пара есть в тот же день, что и все в списке пар конкретного дня,
            то добавить текущую пару в список пар дня.
             *//*
            if(couplesInDay.size() == 0 || couplesInDay.get(couplesInDay.size() - 1).dateAndTimeOfCouple.toLocalDate().equals(couple.dateAndTimeOfCouple.toLocalDate()))
                couplesInDay.add(couple);
            else {
                // Требуется
            }
        }*/
        }

        /// <summary>
        /// Данный метод сортирует входные данные по возрастанию даты.
        /// </summary>
        /// <param name="listNeedMerge">Входные данные, которые необходимо отсортировать.</param>
        /// <param name="pr">Ссылка, куда надо отправлять отчёт о готовности.</param>
        private static void sortByDateTime(LinkedList<CoupleInCalendar> listNeedMerge, PercentReady pr)
        {
            float max = listNeedMerge.Count * (float)System.Math.Log(listNeedMerge.Count) + 1;
            int i = 0;
            listNeedMerge = new LinkedList<CoupleInCalendar>(listNeedMerge.OrderBy(coupleInCalendar =>
            {
                if (i < max)
                    pr.Ready = i++ / max;
                return coupleInCalendar.DateAndTimeOfCouple;
            }, ZonedDateTime.Comparer.Instant));
            pr.Ready = 1.0f;
        }
    }
}
