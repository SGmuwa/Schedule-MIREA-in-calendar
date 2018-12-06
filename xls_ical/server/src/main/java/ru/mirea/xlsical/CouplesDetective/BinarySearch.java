package ru.mirea.xlsical.CouplesDetective;


import java.util.ArrayList;

public class BinarySearch {
    private static <T extends Comparable<? super T>> int BinarySearch_Iter(ArrayList<T> array, boolean descendingOrder, T key) {
        int left = 0;
        int right = array.size();
        int mid = 0;

        while (!(left >= right))
        {
            mid = left + (right - left) / 2;

            if (array.get(mid) == key)
                return mid;

            if ((array.get(mid).compareTo(key) > 0) ^ descendingOrder)
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
     * @param <E> Тим должен уметь стравниваться.
     * @return Индекс элемента. Если не найден, то ищется ближайший элемент, который меньше {@code key},
     * а затем результат ивенсируется с помощью оператора {@code ~}. Если все элементы массива больше {@code key},
     * то возвращается {@link Integer#MIN_VALUE}.
     */
    public static <E extends Comparable<? super E>> int BinarySearch_Iter_Wrapper(ArrayList<E> array, E key)
    {
        if(array == null)
            throw new NullPointerException("array must be not null!");
        if (array.size() == 0 || key == null)
            return Integer.MIN_VALUE;
        int left = BinarySearch_Iter(array, array.get(0).compareTo(array.get(array.size() - 1)) > 0, key);
        if(left < 0)
            left = ~left;
        if(left >= array.size())
            left = array.size() - 1;
        while(left >= 0 && array.get(left).compareTo(key) >= 0)
            left--;
        if(left + 1 < array.size() && array.get(left + 1).compareTo(key) == 0)
            left++;
        if(left == -1) {
            if (array.get(0).compareTo(key) == 0)
                return 0;
            return Integer.MIN_VALUE;
        }
        return array.get(left).compareTo(key) == 0 ? left : ~left;
    }
}
