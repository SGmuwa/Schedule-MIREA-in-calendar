using System.Collections.Generic;
using System;

namespace ru.mirea.xlsical.CouplesDetective
{
    public class BinarySearch
    {
        private static int BinarySearch_Iter<T>(IReadOnlyList<T> array, int size, bool descendingOrder, T key, IComparer<T> comparator)
        {
            int left = 0;
            int right = size;
            int mid = 0;

            while (!(left >= right))
            {
                mid = left + (right - left) / 2;

                if (array[mid].Equals(key))
                    return mid;

                if ((comparator.Compare(array[mid], key) > 0) ^ descendingOrder)
                    right = mid;
                else
                    left = mid + 1;
            }

            return ~left;
        }

        /**
         * Бинарный поиск индекса элемента. Укажет индекс самого ближнего к индексу 0 соответствующего элемента. Если не нашёл - показывает ближайший индекс под оператором ~.
         * @param array Входящий массив, в котором надо искать. Массив должен быть отсортирован.
         * @param key То, что надо найти
         * @param <E> Тим должен уметь сравниваться.
         * @return Индекс элемента. Если не найден, то ищется ближайший элемент, который меньше {@code key},
         * а затем результат инвертируется с помощью оператора {@code ~}. Если все элементы массива больше {@code key},
         * то возвращается {@link Integer#MIN_VALUE}.
         */
        public static int BinarySearch_Iter_Wrapper<E>(List<E> array, E key) where E : IComparable<E>
        {
            if (array == null)
                throw new ArgumentNullException("array must be not null!");
            if (array.Count == 0 || key == null)
                return int.MinValue;
            int left = BinarySearch_Iter(array, array.Count, array[0].CompareTo(array[array.Count - 1]) > 0, key, (a, b)->a.compareTo(b));
            if (left < 0)
                left = ~left;
            if (left >= array.Count)
                left = array.Count - 1;
            while (left >= 0 && array[left].CompareTo(key) >= 0)
                left--;
            if (left + 1 < array.Count && array[left + 1].CompareTo(key) == 0)
                left++;
            if (left == -1)
            {
                if (array[0].compareTo(key) == 0)
                    return 0;
                return int.MinValue;
            }
            return array.get(left).compareTo(key) == 0 ? left : ~left;
        }

        public static int BinarySearch_Iter_Wrapper<E>(E key, ICanGetValueByIndex<E> array, int size, IComparer<E> comparator)
        {
            if (array == null || comparator == null)
                throw new ArgumentNullException("array must be not null!");
            if (size == 0 || key == null)
                return Integer.MIN_VALUE;
            int left = BinarySearch_Iter(array::get, size, comparator.compare(array.get(0), array.get(size - 1)) > 0, key, comparator);
            if (left < 0)
                left = ~left;
            if (left >= size)
                left = size - 1;
            while (left >= 0 && comparator.compare(array.get(left), key) >= 0)
                left--;
            if (left + 1 < size && comparator.compare(array.get(left + 1), key) == 0)
                left++;
            if (left == -1)
            {
                if (comparator.compare(array.get(0), key) == 0)
                    return 0;
                return Integer.MIN_VALUE;
            }
            return comparator.compare(array.get(left), key) == 0 ? left : ~left;
        }
    }

    interface ICanGetValueByIndex<E>
    {
        E this[int index] { get; }
    }
}
